//
// CameraShakeExample.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections;
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
using Thinksquirrel.Utilities;

//!\cond PRIVATE
namespace Thinksquirrel.UtilitiesExample
{
#endif
    [AddComponentMenu("")]
    [RequireComponent(typeof(CameraShake))]
    public class CameraShakeExample : MonoBehaviour {

        #region Private variables
        CameraShake m_Shake;
        bool m_ShakeGUI;
        bool m_Shake1;
        bool m_Shake2;
        bool m_MultiShake;
        #endregion

        #region MonoBehaviour methods
        void Start()
        {
            m_Shake = GetComponent<CameraShake>();
        }
        void OnGUI()
        {
            if (m_Shake)
            {
                DrawGUIArea1();
                DrawGUIArea2();
            }
        }
        #endregion

        #region Cancel shake button
        void DrawGUIArea1()
        {
            GUI.enabled = (m_Shake.IsShaking() || m_MultiShake) && !m_Shake.IsCancelling();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Cancel Shake"))
            {
                StopAllCoroutines();
                m_Shake.CancelShake(0.5f);
                m_Shake1 = false;
                m_Shake2 = false;
                m_MultiShake = false;
            }
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }
        #endregion

        #region Main interface
        void DrawGUIArea2()
        {
            if (m_ShakeGUI)
            {
                m_Shake.BeginShakeGUILayout();
            }
            else
            {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            }

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Space(100);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = !m_Shake.IsShaking() && !m_MultiShake;

            m_Shake1 = GUILayout.Toggle(m_Shake1, "Shake (without GUI)", GUI.skin.button, GUILayout.Width(200), GUILayout.Height(50));
            m_Shake2 = GUILayout.Toggle(m_Shake2, "Shake (with GUI)", GUI.skin.button, GUILayout.Width(200), GUILayout.Height(50));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical(GUILayout.Width(300));

            GUILayout.Label("Number of Shakes: " + m_Shake.numberOfShakes);

            m_Shake.numberOfShakes = (int)GUILayout.HorizontalSlider(m_Shake.numberOfShakes, 1, 10);

            GUILayout.Label("Shake Amount: " + m_Shake.shakeAmount);

            float x, y, z;

            x = GUILayout.HorizontalSlider(m_Shake.shakeAmount.x, 0, 2);
            y = GUILayout.HorizontalSlider(m_Shake.shakeAmount.y, 0, 2);
            z = GUILayout.HorizontalSlider(m_Shake.shakeAmount.z, 0, 2);

            if (Mathf.Abs(x - m_Shake.shakeAmount.x) > Vector3.kEpsilon ||
                Mathf.Abs(y - m_Shake.shakeAmount.y) > Vector3.kEpsilon ||
                Mathf.Abs(z - m_Shake.shakeAmount.z) > Vector3.kEpsilon)
                m_Shake.shakeAmount = new Vector3(x, y, z);

            GUILayout.Label("Rotation Amount: " + m_Shake.rotationAmount);

            x = GUILayout.HorizontalSlider(m_Shake.rotationAmount.x, 0, 10);
            y = GUILayout.HorizontalSlider(m_Shake.rotationAmount.y, 0, 10);
            z = GUILayout.HorizontalSlider(m_Shake.rotationAmount.z, 0, 10);

            if (Mathf.Abs(x - m_Shake.rotationAmount.x) > Vector3.kEpsilon ||
                Mathf.Abs(y - m_Shake.rotationAmount.y) > Vector3.kEpsilon ||
                Mathf.Abs(z - m_Shake.rotationAmount.z) > Vector3.kEpsilon)
                m_Shake.rotationAmount = new Vector3(x, y, z);

            GUILayout.Label("Distance: " + m_Shake.distance.ToString("0.00"));

            m_Shake.distance = GUILayout.HorizontalSlider(m_Shake.distance, 0, .1f);

            GUILayout.Label("Speed: " + m_Shake.speed.ToString("0.00"));

            m_Shake.speed = GUILayout.HorizontalSlider(m_Shake.speed, 1, 100);

            GUILayout.Label("Decay: " + m_Shake.decay.ToString("0.00%"));

            m_Shake.decay = GUILayout.HorizontalSlider(m_Shake.decay, 0, 1);

            GUILayout.Space(5);

            GUILayout.Label("Presets:");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Explosion"))
                Explosion();
            if (GUILayout.Button("Footsteps"))
                Footsteps();
            if (GUILayout.Button("Bumpy"))
                Bumpy();

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset"))
                Application.LoadLevel(Application.loadedLevel);

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUI.enabled = true;

            if (m_ShakeGUI)
            {
                m_Shake.EndShakeGUILayout();
            }
            else
            {
                GUILayout.EndArea();
            }
        }
        #endregion

        #region Manual shakes
        void Update()
        {
        if (m_Shake.shakeType == CameraShake.ShakeType.CameraMatrix)
        {
            transform.parent.position = new Vector3(
                transform.parent.position.x,
                Mathf.Cos(Time.time) * .35f,
                transform.parent.position.z);

            transform.localPosition = new Vector3(
                Mathf.Sin(Time.time) * .5f,
                transform.localPosition.y,
                transform.localPosition.z);
        }
        else
        {
            transform.parent.position = new Vector3(
                Mathf.Sin(Time.time) * .5f,
                Mathf.Cos(Time.time) * .35f,
                transform.parent.position.z);

        }
            if (m_Shake1)
            {
                m_ShakeGUI = false;
                m_Shake1 = false;
                m_Shake.Shake();
            }
            else if (m_Shake2)
            {
                m_ShakeGUI = true;
                m_Shake2 = false;
                m_Shake.Shake();
            }
        }
        #endregion

        #region Preset shakes
        void Explosion()
        {
            m_MultiShake = true;
            m_ShakeGUI = true;

            // Single shake
            var rot = new Vector3(2, .5f, 10);
            m_Shake.Shake(m_Shake.shakeType, 5, Vector3.one, rot, 0.25f, 50.0f, 0.20f, 1.0f, true, () => m_MultiShake = false);
        }

        void Footsteps()
        {
            m_ShakeGUI = true;
            m_MultiShake = true;

            // Sequential shakes

            StartCoroutine(DoFootsteps());
        }
        IEnumerator DoFootsteps()
        {
            var rot = new Vector3(2, .5f, 1);
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot, 0.02f, 50.0f, 0.50f, 1.0f, true, null);
            yield return new WaitForSeconds(1.0f);
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot, 0.03f, 50.0f, 0.50f, 1.0f, true, null);
            yield return new WaitForSeconds(1.0f);
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot * 1.5f, 0.04f, 50.0f, 0.50f, 1.0f, true, null);
            yield return new WaitForSeconds(1.0f);
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot * 2f, 0.05f, 50.0f, 0.50f, 1.0f, true, null);
            yield return new WaitForSeconds(1.0f);
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot * 2.5f, 0.06f, 50.0f, 0.50f, 1.0f, true, () => m_MultiShake = false);
        }

        void Bumpy()
        {
            m_ShakeGUI = true;
            m_MultiShake = true;

            // Multiple sequential shakes at once

            StartCoroutine(DoBumpy());
            StartCoroutine(DoBumpy2());
        }
        IEnumerator DoBumpy()
        {
            var rot = new Vector3(2, 2, 2);
            for(int i = 0; i < 50; i++)
            {
                m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, rot, 0.02f, 50.0f, 0.00f, 1.0f, true, null);
                yield return new WaitForSeconds(0.1f);
            }
            m_Shake.Shake(m_Shake.shakeType, 3, Vector3.one, Vector3.one, 0.02f, 50.0f, 0.00f, 1.0f, true, () => m_MultiShake = false);
        }
        IEnumerator DoBumpy2()
        {
            var rot = new Vector3(8, 1, 4);
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(1.0f);
                m_Shake.Shake(m_Shake.shakeType, 10, Vector3.up, rot, 0.50f, 50.0f, 0.20f, 1.0f, true, null);
            }
        }
        #endregion
    }
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
}
#endif
//!\endcond
