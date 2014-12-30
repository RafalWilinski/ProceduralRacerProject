//VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace VacuumShaders
{
    namespace CurvedWorld
    {
        [AddComponentMenu("VacuumShaders/Curved World/Example Scenes Scripts/Game Manager")]
        public class GameManager : MonoBehaviour
        {
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Variables                                                                 //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            static public GameManager get;

            public float speed = 1;
            public GameObject[] chunks;


            static public float chunkSize = 50;
            static public Vector3 moveVector = new Vector3(1, 0, 0);
            static public GameObject lastChunk;

            int id;

            List<Material> listMaterials;
            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Unity Functions                                                           //                
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            void Awake()
            { 
                get = this;

                id = 0;

                //Instantiate first 10 chunks
                for (int i = 0; i < 10; i++)
                {
                    GameObject obj = InstantiateChunk();
                    obj.transform.position = new Vector3(-i * chunkSize, 0, 0);

                    lastChunk = obj;
                }
            }

            // Use this for initialization
            void Start()
            {
                Renderer[] renderers = FindObjectsOfType(typeof(Renderer)) as Renderer[];

                listMaterials = new List<Material>();
                foreach (Renderer _renderer in renderers)
                {
                    listMaterials.AddRange(_renderer.sharedMaterials);
                }

                listMaterials = listMaterials.Distinct().ToList();
            }
            

            //////////////////////////////////////////////////////////////////////////////
            //                                                                          // 
            //Custom Functions                                                          //
            //                                                                          //               
            //////////////////////////////////////////////////////////////////////////////
            GameObject InstantiateChunk()
            {
                id += 1;
                if (id == 9)
                    id = 0;

                //return (GameObject)Instantiate(chunks[Random.Range(0, chunks.Length)]);
                return (GameObject)Instantiate(chunks[id]);
            }

            public void DestroyChunk(MoveElement_Chunk moveElement)
            {
                Vector3 newPos = lastChunk.transform.position;
                newPos.x -= chunkSize;

                Destroy(moveElement.gameObject);

                lastChunk = InstantiateChunk();
                lastChunk.transform.position = newPos;
            }
        }
    }
}