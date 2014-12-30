//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Agent")]
        public class Agent : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            static public float mainSpeed = 1;

            NavMeshAgent navMeshAgent;
            float speed = 1;
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            // Use this for initialization
            void Start()
            {
                transform.position = Vector3.zero;

                navMeshAgent = GetComponent<NavMeshAgent>();


                Vector3 destination = Random.insideUnitSphere * 35;
                destination.y = 0;
                navMeshAgent.SetDestination(destination);

                speed = Random.Range(0.75f, 1.25f);               

            }

            // Update is called once per frame
            void FixedUpdate()
            {
                navMeshAgent.speed = speed * mainSpeed;

                if (navMeshAgent.velocity.magnitude < 0.2f)
                {
                    Vector3 destination = Random.insideUnitSphere * 35;
                    destination.y = 0;
                    navMeshAgent.SetDestination(destination);

                    speed = Random.Range(0.75f, 1.25f);  
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
