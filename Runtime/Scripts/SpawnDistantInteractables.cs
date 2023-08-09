using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Spawn interactables that are distant from the camera for testing purposes. 
    /// </summary>
    public class SpawnDistantInteractables : MonoBehaviour
    {
        public GameObject objectToSpawn;
        public GameObject spawnParent;
        public int number = 10;
        private List<GameObject> cubes = new List<GameObject>();
        private Camera main;

        void Start()
        {
            main = Camera.main;
            float cameraZ = main.transform.position.z;
            for (int i = 0; i < number; i++)
            {
                GameObject newObject = Instantiate(objectToSpawn, new Vector3(-2 + i * 1f, Random.Range(0, 4f), cameraZ + 16.0f - Random.Range(0, 4f)),
                    Quaternion.identity, spawnParent.transform);
                cubes.Add(newObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
