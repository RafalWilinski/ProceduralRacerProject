// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using System;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AmplifyMotion
{
internal class SkinnedState : AmplifyMotion.MotionState
{
	private SkinnedMeshRenderer m_renderer;

	private int m_boneCount;
	private Transform[] m_boneTransforms;
	private Matrix4x4[] m_bones;

	private int m_weightCount;
	private int[] m_boneIndices;
	private float[] m_boneWeights;

	private int m_vertexCount;
	private Vector4[] m_baseVertices;
	private Vector3[] m_prevVertices;
	private Vector3[] m_currVertices;

	private Mesh m_clonedMesh;
	private Matrix4x4 m_worldToLocalMatrix;

	private MaterialDesc[] m_sharedMaterials;

	private ManualResetEvent m_asyncUpdateSignal = null;
	private bool m_asyncUpdateTriggered = false;

	private bool m_mask;
	private bool m_starting;
	private bool m_wasVisible;

	public SkinnedState( AmplifyMotionCamera owner, AmplifyMotionObjectBase obj )
		: base( owner, obj )
	{
		m_renderer = m_obj.GetComponent<SkinnedMeshRenderer>();
	}

	internal override void Initialize()
	{
		Transform[] bones = m_renderer.bones;
		if ( bones == null || bones.Length == 0 )
		{
			Debug.LogWarning( "[AmplifyMotion] Bones not found on " + m_obj.name + ". Please note that 'Optimize Game Object' Rig import setting is not yet supported. Motion blur was disabled for this object." );
			m_error = true;
			return;
		}

		for ( int i = 0; i < bones.Length; i++ )
		{
			if ( bones[ i ] == null )
			{
				Debug.LogWarning( "[AmplifyMotion] Found invalid/null Bone on " + m_obj.name + ". Motion blur was disabled for this object." );
				m_error = true;
				return;
			}
		}

		base.Initialize();

		m_vertexCount = m_renderer.sharedMesh.vertexCount;
		if ( m_renderer.quality == SkinQuality.Auto )
			m_weightCount = ( int ) QualitySettings.blendWeights;
		else
			m_weightCount = ( int ) m_renderer.quality;

		m_boneTransforms = m_renderer.bones;
		m_boneCount = m_renderer.bones.Length;
		m_bones = new Matrix4x4[ m_boneCount ];

		Vector4[] baseVertices = new Vector4[ m_vertexCount * m_weightCount ];
		int[] boneIndices = new int[ m_vertexCount * m_weightCount ];
		float[] boneWeights = ( m_weightCount > 1 ) ? new float[ m_vertexCount * m_weightCount ] : null;

		if ( m_weightCount == 1 )
			InitializeBone1( baseVertices, boneIndices );
		else if ( m_weightCount == 2 )
			InitializeBone2( baseVertices, boneIndices, boneWeights );
		else
			InitializeBone4( baseVertices, boneIndices, boneWeights );

		m_prevVertices = new Vector3[ m_vertexCount ];
		m_currVertices = new Vector3[ m_vertexCount ];

		Mesh skinnedMesh = m_renderer.sharedMesh;
		m_clonedMesh = new Mesh();
		m_clonedMesh.vertices = skinnedMesh.vertices;
		m_clonedMesh.normals = skinnedMesh.vertices;
		m_clonedMesh.uv = skinnedMesh.uv;
		m_clonedMesh.subMeshCount = skinnedMesh.subMeshCount;
		for ( int i = 0; i < skinnedMesh.subMeshCount; i++ )
			m_clonedMesh.SetTriangles( skinnedMesh.GetTriangles( i ), i );

		m_sharedMaterials = ProcessSharedMaterials( m_renderer.sharedMaterials );

		m_asyncUpdateSignal = new ManualResetEvent( false );
		m_asyncUpdateTriggered = false;

		// these are discarded in native path; keep them in managed path
		m_baseVertices = baseVertices;
		m_boneIndices = boneIndices;
		m_boneWeights = boneWeights;

		m_wasVisible = false;
	}

	internal override void Shutdown()
	{
		WaitForAsyncUpdate();

		Mesh.Destroy( m_clonedMesh );
	}

	void UpdateBones()
	{
		for ( int i = 0; i < m_boneCount; i++ )
			m_bones[ i ] = m_boneTransforms[ i ].localToWorldMatrix;

		m_worldToLocalMatrix = m_obj.transform.worldToLocalMatrix;
	}

	void UpdateVertices( bool starting )
	{
		if ( !starting && m_wasVisible )
			Array.Copy( m_currVertices, m_prevVertices, m_vertexCount );

		for ( int i = 0; i < m_boneCount; i++ )
			m_bones[ i ] = m_worldToLocalMatrix * m_bones[ i ];

		if ( m_weightCount == 1 )
			UpdateVerticesBone1();
		else if ( m_weightCount == 2 )
			UpdateVerticesBone2();
		else
			UpdateVerticesBone4();

		if ( starting || !m_wasVisible )
			Array.Copy( m_currVertices, m_prevVertices, m_vertexCount );
	}

	void InitializeBone1( Vector4[] baseVertices, int[] boneIndices )
	{
		Vector3[] meshVertices = m_renderer.sharedMesh.vertices;
		Matrix4x4[] meshBindPoses = m_renderer.sharedMesh.bindposes;
		BoneWeight[] meshBoneWeights = m_renderer.sharedMesh.boneWeights;

		for ( int i = 0; i < m_vertexCount; i++ )
		{
			int o0 = i * m_weightCount;
			int b0 = boneIndices[ o0 ] = meshBoneWeights[ i ].boneIndex0;
			baseVertices[ o0 ] = meshBindPoses[ b0 ].MultiplyPoint3x4( meshVertices[ i ] );
		}
	}

	void InitializeBone2( Vector4[] baseVertices, int[] boneIndices, float[] boneWeights )
	{
		Vector3[] meshVertices = m_renderer.sharedMesh.vertices;
		Matrix4x4[] meshBindPoses = m_renderer.sharedMesh.bindposes;
		BoneWeight[] meshBoneWeights = m_renderer.sharedMesh.boneWeights;

		for ( int i = 0; i < m_vertexCount; i++ )
		{
			int o0 = i * m_weightCount;
			int o1 = o0 + 1;

			BoneWeight bw = meshBoneWeights[ i ];
			int b0 = boneIndices[ o0 ] = bw.boneIndex0;
			int b1 = boneIndices[ o1 ] = bw.boneIndex1;

			float w0 = bw.weight0;
			float w1 = bw.weight1;

			float rcpSum = 1.0f / ( w0 + w1 );
			boneWeights[ o0 ] = w0 = w0 * rcpSum;
			boneWeights[ o1 ] = w1 = w1 * rcpSum;

			Vector3 bv0 = w0 * meshBindPoses[ b0 ].MultiplyPoint3x4( meshVertices[ i ] );
			Vector3 bv1 = w1 * meshBindPoses[ b1 ].MultiplyPoint3x4( meshVertices[ i ] );

			baseVertices[ o0 ] = new Vector4( bv0.x, bv0.y, bv0.z, w0 );
			baseVertices[ o1 ] = new Vector4( bv1.x, bv1.y, bv1.z, w1 );
		}
	}

	void InitializeBone4( Vector4[] baseVertices, int[] boneIndices, float[] boneWeights )
	{
		Vector3[] meshVertices = m_renderer.sharedMesh.vertices;
		Matrix4x4[] meshBindPoses = m_renderer.sharedMesh.bindposes;
		BoneWeight[] meshBoneWeights = m_renderer.sharedMesh.boneWeights;

		for ( int i = 0; i < m_vertexCount; i++ )
		{
			int o0 = i * m_weightCount;
			int o1 = o0 + 1;
			int o2 = o0 + 2;
			int o3 = o0 + 3;

			BoneWeight bw = meshBoneWeights[ i ];
			int b0 = boneIndices[ o0 ] = bw.boneIndex0;
			int b1 = boneIndices[ o1 ] = bw.boneIndex1;
			int b2 = boneIndices[ o2 ] = bw.boneIndex2;
			int b3 = boneIndices[ o3 ] = bw.boneIndex3;

			float w0 = boneWeights[ o0 ] = bw.weight0;
			float w1 = boneWeights[ o1 ] = bw.weight1;
			float w2 = boneWeights[ o2 ] = bw.weight2;
			float w3 = boneWeights[ o3 ] = bw.weight3;

			Vector3 bv0 = w0 * meshBindPoses[ b0 ].MultiplyPoint3x4( meshVertices[ i ] );
			Vector3 bv1 = w1 * meshBindPoses[ b1 ].MultiplyPoint3x4( meshVertices[ i ] );
			Vector3 bv2 = w2 * meshBindPoses[ b2 ].MultiplyPoint3x4( meshVertices[ i ] );
			Vector3 bv3 = w3 * meshBindPoses[ b3 ].MultiplyPoint3x4( meshVertices[ i ] );

			baseVertices[ o0 ] = new Vector4( bv0.x, bv0.y, bv0.z, w0 );
			baseVertices[ o1 ] = new Vector4( bv1.x, bv1.y, bv1.z, w1 );
			baseVertices[ o2 ] = new Vector4( bv2.x, bv2.y, bv2.z, w2 );
			baseVertices[ o3 ] = new Vector4( bv3.x, bv3.y, bv3.z, w3 );
		}
	}

	void UpdateVerticesBone1()
	{
		for ( int i = 0; i < m_vertexCount; i++ )
			MulPoint3x4_XYZ( ref m_currVertices[ i ], ref m_bones[ m_boneIndices[ i ] ], m_baseVertices[ i ] );
	}

	void UpdateVerticesBone2()
	{
		Vector3 deformedVertex = Vector3.zero;
		for ( int i = 0; i < m_vertexCount; i++ )
		{
			int o0 = i * 2;
			int o1 = o0 + 1;

			int b0 = m_boneIndices[ o0 ];
			int b1 = m_boneIndices[ o1 ];
			float w1 = m_boneWeights[ o1 ];

			MulPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b0 ], m_baseVertices[ o0 ] );
			if ( w1 != 0 )
				MulAddPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b1 ], m_baseVertices[ o1 ] );

			m_currVertices[ i ] = deformedVertex;
		}
	}

	void UpdateVerticesBone4()
	{
		Vector3 deformedVertex = Vector3.zero;
		for ( int i = 0; i < m_vertexCount; i++ )
		{
			int o0 = i * 4;
			int o1 = o0 + 1;
			int o2 = o0 + 2;
			int o3 = o0 + 3;

			int b0 = m_boneIndices[ o0 ];
			int b1 = m_boneIndices[ o1 ];
			int b2 = m_boneIndices[ o2 ];
			int b3 = m_boneIndices[ o3 ];

			float w1 = m_boneWeights[ o1 ];
			float w2 = m_boneWeights[ o2 ];
			float w3 = m_boneWeights[ o3 ];

			MulPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b0 ], m_baseVertices[ o0 ] );
			if ( w1 != 0 )
				MulAddPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b1 ], m_baseVertices[ o1 ] );
			if ( w2 != 0 )
				MulAddPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b2 ], m_baseVertices[ o2 ] );
			if ( w3 != 0 )
				MulAddPoint3x4_XYZW( ref deformedVertex, ref m_bones[ b3 ], m_baseVertices[ o3 ] );

			m_currVertices[ i ] = deformedVertex;
		}
	}

	internal override void AsyncUpdate()
	{
		try
		{
			// managed path
			UpdateVertices( m_starting );
		}
		catch ( System.Exception e )
		{
			Debug.LogError( "[AmplifyMotion] Failed on SkinnedMeshRenderer data. Please contact support.\n" + e.Message );
		}
		finally
		{
			m_asyncUpdateSignal.Set();
		}
	}

	internal override void UpdateTransform( bool starting )
	{
		if ( !m_initialized )
		{
			Initialize();
			return;
		}

		Profiler.BeginSample( "Skinned.Update" );

		bool isVisible = m_renderer.isVisible;

		if ( !m_error && ( isVisible || starting ) )
		{
			UpdateBones();

			m_mask = ( m_owner.Instance.CullingMask & ( 1 << m_obj.gameObject.layer ) ) != 0;
			m_starting = !m_wasVisible || starting;

			m_asyncUpdateSignal.Reset();
			m_asyncUpdateTriggered = true;

			m_owner.Instance.WorkerPool.EnqueueAsyncUpdate( this );
		}

		m_wasVisible = isVisible;

		Profiler.EndSample();
	}

	void WaitForAsyncUpdate()
	{
		if ( m_asyncUpdateTriggered )
		{
			if ( !m_asyncUpdateSignal.WaitOne( MotionState.AsyncUpdateTimeout ) )
			{
				Debug.LogWarning( "[AmplifyMotion] Aborted abnormally long Async Skin deform operation. Not a critical error but might indicate a problem. Please contact support." );
				return;
			}
			m_asyncUpdateTriggered = false;
		}
	}

	internal override void RenderVectors( Camera camera, float scale, AmplifyMotion.Quality quality )
	{
		Profiler.BeginSample( "Skinned.Render" );

		if ( m_initialized && !m_error && m_renderer.isVisible )
		{
			WaitForAsyncUpdate();

			m_clonedMesh.vertices = m_currVertices;
			m_clonedMesh.normals = m_prevVertices;

			const float rcp255 = 1 / 255.0f;
			int objectId = m_mask ? m_owner.Instance.GenerateObjectId( m_obj.gameObject ) : 255;

			Shader.SetGlobalFloat( "_EFLOW_OBJECT_ID", objectId * rcp255 );
			Shader.SetGlobalFloat( "_EFLOW_MOTION_SCALE", m_mask ? scale : 0 );

			int qualityPass = ( quality == AmplifyMotion.Quality.Mobile ) ? 0 : 2;

			for ( int i = 0; i < m_sharedMaterials.Length; i++ )
			{
				MaterialDesc matDesc = m_sharedMaterials[ i ];
				int pass = qualityPass + ( matDesc.coverage ? 1 : 0 );

				if ( matDesc.coverage )
				{
					m_owner.Instance.SkinnedVectorsMaterial.mainTexture = matDesc.material.mainTexture;
					if ( matDesc.cutoff )
						m_owner.Instance.SkinnedVectorsMaterial.SetFloat( "_Cutoff", matDesc.material.GetFloat( "_Cutoff" ) );
				}

				if ( m_owner.Instance.SkinnedVectorsMaterial.SetPass( pass ) )
					Graphics.DrawMeshNow( m_clonedMesh, m_obj.transform.localToWorldMatrix, i );
			}
		}

		Profiler.EndSample();
	}
}
}
