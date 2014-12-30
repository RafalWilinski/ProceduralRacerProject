//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/FPS")]
        public class FPS : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            float lastInterval; 
            int frames = 0; 
            public static float fps;

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            void Start()
            {
                lastInterval = Time.realtimeSinceStartup;
                frames = 0;
            }

            void OnGUI()
            {
                GUI.Label(new Rect(10, 10, 100, 20), (Mathf.Round(fps * 100.0f) / 100.0f).ToString() + " fps");
            }

            void Update()
            {
                ++frames;
                float timeNow = Time.realtimeSinceStartup;
                if (timeNow > lastInterval + 0.5f)
                {
                    fps = frames / (timeNow - lastInterval);
                    frames = 0;
                    lastInterval = timeNow;
                }
            }
        }
    }
}