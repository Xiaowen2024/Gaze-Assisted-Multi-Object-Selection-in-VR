using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Students.SU2023.IN06.Yuan
{

    /// <summary>
    /// Spawn interactables that are close to each other for testing purposes. 
    /// </summary>
    public class SpawnCloseInteractables : MonoBehaviour
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
                GameObject newObject = Instantiate(objectToSpawn, new Vector3(-2 + i * 0.5f, 2f, cameraZ + 16f - Random.Range(0, 4f)),
                    Quaternion.identity, spawnParent.transform);
                newObject.name = "cube" + i.ToString();
                cubes.Add(newObject);
            }

        }
        
        void Update()
        {

        }
    }
}
