//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Player")]
        public class Player : MonoBehaviour
        {
            public enum SIDE { Left, Center, Right }
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            static public Player get;

            Vector3 newPos;
            SIDE side;
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            void Awake()
            {
                get = this;
            }

            // Use this for initialization
            void Start()
            {
                transform.position = Vector3.zero;

                newPos = transform.position;

                side = SIDE.Center;
            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    if (side == SIDE.Center)
                    {
                        newPos = new Vector3(0, 0, -5);
                        side = SIDE.Left;
                    }
                    else if (side == SIDE.Right)
                    {
                        newPos = Vector3.zero;
                        side = SIDE.Center;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    if (side == SIDE.Left)
                    {
                        newPos = new Vector3(0, 0, 0);
                        side = SIDE.Center;
                    }
                    else if (side == SIDE.Center)
                    {
                        newPos = new Vector3(0, 0, 4);
                        side = SIDE.Right;
                    }
                }

                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 10);
            }

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
        }
    }
}