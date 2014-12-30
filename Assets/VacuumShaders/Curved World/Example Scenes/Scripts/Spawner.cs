//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Spawner")]
        public class Spawner : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////

            public GameObject bunny;
            public GameObject bear;
            public GameObject hellephant;

            public int spawnCount = 1;
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            // Use this for initialization
            void Start()
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    StartCoroutine(Spawn());
                }
            }
            
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            IEnumerator Spawn()
            {             
                yield return new WaitForSeconds(Random.Range(0.0f, 5.0f));

                float val = Random.value;

                if (val < 0.4f)
                    Instantiate(bunny);
                else if (val < 0.8f)
                    Instantiate(bear);
                else
                    Instantiate(hellephant);
            }
        }
    }

}