using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering;
using static System.Random;
using Random = UnityEngine.Random;

namespace XRC.Students.SU2023.IN06.Yuan {
    /// <summary>
    /// Spawn interactables for testing purposes. 
    /// </summary>
public class SpawnInteractables : MonoBehaviour
{
    // Start is called before the first frame update
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
            GameObject newObject = Instantiate(objectToSpawn, new Vector3(-8f + i * 2f, Random.Range(0, 3), cameraZ + 16.0f - Random.Range(0, 4f)),Quaternion.identity, spawnParent.transform);
            cubes.Add(newObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        
    }
}
}
