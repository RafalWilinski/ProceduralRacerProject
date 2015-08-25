// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyMotion
{

public enum ObjectType
{
	None,
	Solid,
	Skinned,
	Cloth,
	Particle
}

[Serializable]
internal abstract class MotionState
{
	protected struct MaterialDesc
	{
		public Material material;
		public bool coverage;
		public bool cutoff;
	}

	// TEMPORARY
	//public string m_name = "";
	//public string Name { get { return m_name; } }

	public const int AsyncUpdateTimeout = 100;

	protected bool m_error;
	protected bool m_initialized;

	protected AmplifyMotionCamera m_owner;
	protected AmplifyMotionObjectBase m_obj;

	public AmplifyMotionCamera Owner { get { return m_owner; } }
	public bool Error { get { return m_error; } }

	public MotionState( AmplifyMotionCamera owner, AmplifyMotionObjectBase obj )
	{
		// TEMPORARY
		//m_name = obj.name;

		m_error = false;
		m_initialized = false;

		m_owner = owner;
		m_obj = obj;
	}

	internal virtual void Initialize() { m_initialized = true; }
	internal virtual void Shutdown() {}
	internal abstract void UpdateTransform( bool starting );
	internal virtual void AsyncUpdate() {}
	internal virtual void RenderVectors( Camera camera, float scale, AmplifyMotion.Quality quality ) {}

	protected MaterialDesc[] ProcessSharedMaterials( Material[] mats )
	{
		MaterialDesc[] matsDesc = new MaterialDesc [ mats.Length ];
		for ( int i = 0; i < mats.Length; i++ )
		{
			matsDesc[ i ].material = mats[ i ];
			bool legacyCoverage = ( mats[ i ].GetTag( "RenderType", false ) == "TransparentCutout" );
		#if UNITY_3 || UNITY_4
			matsDesc[ i ].coverage = legacyCoverage;
		#else
			matsDesc[ i ].coverage = legacyCoverage || mats[ i ].IsKeywordEnabled( "_ALPHATEST_ON" );
		#endif
			matsDesc[ i ].cutoff = mats[ i ].HasProperty( "_Cutoff" );
		}
		return matsDesc;
	}

	internal static bool VectorChanged( Vector3 a, Vector3 b )
	{
		return Vector3.SqrMagnitude( a - b ) > 0.0f;
	}

	internal static bool RotationChanged( Quaternion a, Quaternion b )
	{
		Vector4 diff = new Vector4( a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w );
		return Vector4.SqrMagnitude( diff ) > 0.0f;
	}

	internal static void MulPoint4x4_XYZW( ref Vector4 result, ref Matrix4x4 mat, Vector4 vec )
	{
		result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
		result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
		result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
		result.w = mat.m30 * vec.x + mat.m31 * vec.y + mat.m32 * vec.z + mat.m33 * vec.w;
	}

	internal static void MulPoint3x4_XYZ( ref Vector3 result, ref Matrix4x4 mat, Vector4 vec )
	{
		result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03;
		result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13;
		result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23;
	}

	internal static void MulPoint3x4_XYZW( ref Vector3 result, ref Matrix4x4 mat, Vector4 vec )
	{
		result.x = mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
		result.y = mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
		result.z = mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
	}

	internal static void MulAddPoint3x4_XYZW( ref Vector3 result, ref Matrix4x4 mat, Vector4 vec )
	{
		result.x += mat.m00 * vec.x + mat.m01 * vec.y + mat.m02 * vec.z + mat.m03 * vec.w;
		result.y += mat.m10 * vec.x + mat.m11 * vec.y + mat.m12 * vec.z + mat.m13 * vec.w;
		result.z += mat.m20 * vec.x + mat.m21 * vec.y + mat.m22 * vec.z + mat.m23 * vec.w;
	}
}
}

[AddComponentMenu( "" )]
public class AmplifyMotionObjectBase : MonoBehaviour
{
	[Serializable]
	public class ParticleSystemDescriptor
	{
		public bool sizeOverLifeTimeActive;
		public AnimationCurve curveSizeOverLifeTime;

		public bool sizeBySpeedActive;
		public AnimationCurve curveBySpeed;
		public float speedRangeMin;
		public float speedRangeMax;
	}

	internal static bool ApplyToChildren = true;
	[SerializeField] private bool m_applyToChildren = ApplyToChildren;
	[SerializeField,HideInInspector] private ParticleSystemDescriptor m_particleSystemDesc = new ParticleSystemDescriptor();

	private AmplifyMotion.ObjectType m_type = AmplifyMotion.ObjectType.None;
	private Dictionary<Camera, AmplifyMotion.MotionState> m_states = new Dictionary<Camera, AmplifyMotion.MotionState>();

	private bool m_initialized = false;
	private bool m_fixedStep = false;
	private int m_objectId = 0;

	internal bool FixedStep { get { return m_fixedStep; } }
	internal int ObjectId { get { return m_objectId; } }

	public AmplifyMotion.ObjectType Type { get { return m_type; } }
	public ParticleSystemDescriptor ParticleSystemDesc { get { return m_particleSystemDesc; } }

	internal void RegisterCamera( AmplifyMotionCamera camera )
	{
		Camera actual = camera.GetComponent<Camera>();
		if ( ( actual.cullingMask & ( 1 << gameObject.layer ) ) != 0 && !m_states.ContainsKey( actual ) )
		{
			AmplifyMotion.MotionState state = null;
			switch ( m_type )
			{
				case AmplifyMotion.ObjectType.Solid:
					state = new AmplifyMotion.SolidState( camera, this ); break;
				case AmplifyMotion.ObjectType.Skinned:
					state = new AmplifyMotion.SkinnedState( camera, this );	break;
				case AmplifyMotion.ObjectType.Cloth:
					state = new AmplifyMotion.ClothState( camera, this ); break;
				case AmplifyMotion.ObjectType.Particle:
					state = new AmplifyMotion.ParticleState( camera, this ); break;
				default:
					throw new Exception( "[AmplifyMotion] Invalid object type." );
			}

			camera.RegisterObject( this );

			m_states.Add( actual, state );
		}
	}

	internal void UnregisterCamera( AmplifyMotionCamera camera )
	{
		AmplifyMotion.MotionState state;
		Camera actual = camera.GetComponent<Camera>();
		if ( m_states.TryGetValue( actual, out state ) )
		{
			camera.UnregisterObject( this );

			if ( m_states.TryGetValue( actual, out state ) )
				state.Shutdown();

			m_states.Remove( actual );
		}
	}

	bool InitializeType()
	{
		Renderer renderer = GetComponent<Renderer>();
		if ( AmplifyMotionEffectBase.CanRegister( gameObject ) )
		{
			ParticleSystem particleRenderer = GetComponent<ParticleSystem>();
			if ( particleRenderer != null )
			{
				m_type = AmplifyMotion.ObjectType.Particle;
				AmplifyMotionEffectBase.RegisterObject( this );
			}
			else if ( renderer != null )
			{
				// At this point, Renderer is guaranteed to be one of the following
				if ( renderer.GetType() == typeof( MeshRenderer ) )
					m_type = AmplifyMotion.ObjectType.Solid;
			#if UNITY_4
				else if ( renderer.GetType() == typeof( ClothRenderer ) )
					m_type = AmplifyMotion.ObjectType.Cloth;
			#endif
				else if ( renderer.GetType() == typeof( SkinnedMeshRenderer ) )
				{
				#if !UNITY_4
					if ( GetComponent<Cloth>() != null )
						m_type = AmplifyMotion.ObjectType.Cloth;
					else
				#endif
						m_type = AmplifyMotion.ObjectType.Skinned;
				}

				AmplifyMotionEffectBase.RegisterObject( this );
			}
		}

		// No renderer? disable it, it is here just for adding children
		return ( renderer != null );
	}

	void OnEnable()
	{
		m_initialized = false;

		bool valid = InitializeType();
		if ( valid )
		{
			if ( m_type == AmplifyMotion.ObjectType.Cloth )
			{
			#if UNITY_4
				m_fixedStep = true;
			#else
				m_fixedStep = false;
			#endif
			}
			else if ( m_type == AmplifyMotion.ObjectType.Solid )
			{
				Rigidbody rigidbody = GetComponentInParent<Rigidbody>();
				if ( rigidbody != null && rigidbody.interpolation == RigidbodyInterpolation.None )
					m_fixedStep = true;
			}
		}

		if ( m_applyToChildren )
		{
			foreach ( Transform child in gameObject.transform )
				AmplifyMotionEffectBase.RegisterRecursivelyS( child.gameObject );
		}

		if ( !valid )
			enabled = false;
	}

	void OnDisable()
	{
		AmplifyMotionEffectBase.UnregisterObject( this );
		m_initialized = false;
	}

	void TryInitialize()
	{
		foreach ( AmplifyMotion.MotionState state in m_states.Values )
			state.Initialize();

		m_initialized = true;
	}

	void Start()
	{
		if ( AmplifyMotionEffectBase.Instance != null && !m_initialized )
			TryInitialize();
	}

	void Update()
	{
		if ( AmplifyMotionEffectBase.Instance != null && !m_initialized )
			TryInitialize();
	}

	internal void OnUpdateTransform( AmplifyMotionCamera owner, bool starting )
	{
		AmplifyMotion.MotionState state;
		if ( m_states.TryGetValue( owner.GetComponent<Camera>(), out state ) )
		{
			if ( !state.Error )
				state.UpdateTransform( starting );
		}
	}

	internal void OnRenderVectors( Camera camera, float scale, AmplifyMotion.Quality quality )
	{
		AmplifyMotion.MotionState state = null;
		if ( m_states.TryGetValue( Camera.current, out state ) )
		{
			if ( !state.Error )
				state.RenderVectors( camera, scale, quality );
		}
	}
}
