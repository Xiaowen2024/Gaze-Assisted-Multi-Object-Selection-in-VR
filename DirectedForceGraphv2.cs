using System.Collections.Generic;
using UnityEngine;

public class DirectedForceGraphv2 : MonoBehaviour
{
    public GameObject cubePrefab; // Prefab for the cube objects
    public int numberOfCubes = 10; // Number of cubes to create
    public float cubeSize = 1f; // Size of each cube
    public float graphScale = 5f; // Scale to spread out the cubes for better visualization
    public float scaleForce = 100; 
    private List<GameObject> cubes;
    private List<List<int>> adjacencyList;

    void Start()
    {
        cubes = new List<GameObject>();
        adjacencyList = new List<List<int>>();

        // Create cubes and add them to the cubes list
        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 position = Random.insideUnitSphere * graphScale;
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.transform.localScale = Vector3.one * cubeSize;
            cubes.Add(cube);
        }

        // Identify adjacent cubes and build the adjacency list
        for (int i = 0; i < cubes.Count; i++)
        {
            adjacencyList.Add(new List<int>());
            for (int j = 0; j < cubes.Count; j++)
            {
                if (i == j) continue; // Skip checking against itself

                if (AreCubesAdjacent(cubes[i], cubes[j]))
                {
                    adjacencyList[i].Add(j);
                }
            }
        }
    }

    void Update()
    {
        // Apply forces to the cubes to move them based on the adjacency
        for (int i = 0; i < cubes.Count; i++)
        {
            Vector3 totalForce = Vector3.zero;
            for (int j = 0; j < adjacencyList[i].Count; j++)
            {
                int neighborIndex = adjacencyList[i][j];
                Vector3 direction = cubes[neighborIndex].transform.position - cubes[i].transform.position;
                totalForce += direction.normalized / direction.sqrMagnitude; // Inverse square law force
            }
            Debug.Log("Total force is " + totalForce);
            cubes[i].GetComponent<Rigidbody>().AddForce(totalForce * scaleForce);
        }
    }

    // Function to check if two cubes are adjacent
    bool AreCubesAdjacent(GameObject cube1, GameObject cube2)
    {
        float distance = Vector3.Distance(cube1.transform.position, cube2.transform.position);
        return Mathf.Approximately(distance, cubeSize);
    }
}
