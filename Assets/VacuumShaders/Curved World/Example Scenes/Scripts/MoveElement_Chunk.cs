//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Move Element")]
        public class MoveElement_Chunk : MonoBehaviour
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
                transform.Translate(GameManager.moveVector * GameManager.get.speed * Time.deltaTime);
            }

            void FixedUpdate()
            {
                if (transform.position.x > 110)
                    GameManager.get.DestroyChunk(this);
            }

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
        }
    }
}