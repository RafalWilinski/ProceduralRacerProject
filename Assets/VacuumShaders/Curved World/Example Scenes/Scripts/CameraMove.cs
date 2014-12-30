//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;

namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Camera Move")]
        public class CameraMove : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////

            public float speed = 1;
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
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * speed, Space.World);

                    if (transform.position.z < -20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, -20);
                    }
                }

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed, Space.World);

                    if (transform.position.z > 20)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, 20);
                    }
                }

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed, Space.World);

                    if (transform.position.x < -20)
                    {
                        transform.position = new Vector3(-20, transform.position.y, transform.position.z);
                    }
                }

                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * speed, Space.World);

                    if (transform.position.x > 20)
                    {
                        transform.position = new Vector3(20, transform.position.y, transform.position.z);
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////

        }
    }
}