//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders
    
using UnityEngine;
using System.Collections;

namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Football")]

        public class Football : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////

            Rigidbody rb;
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            // Use this for initialization
            void Start()
            {
                rb = GetComponent<Rigidbody>();
            }

            void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.tag != "Player")
                    rb.AddForce(((transform.position - collision.transform.position).normalized + Vector3.up * 2).normalized * Random.Range(5.0f, 20.0f), ForceMode.Impulse);

            }

            // Update is called once per frame
            void FixedUpdate()
            {
                if (transform.position.y < -20)
                    transform.position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(3.0f, 7.0f), Random.Range(-5.0f, 5.0f));
            }
        }
    }
}