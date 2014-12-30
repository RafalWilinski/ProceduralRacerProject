//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Player Camera")]
        public class PlayerCamera : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                Vector3 newPos = transform.position;
                newPos.z = Player.get.transform.position.z;

                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 8);
            }

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
        }
    }
}