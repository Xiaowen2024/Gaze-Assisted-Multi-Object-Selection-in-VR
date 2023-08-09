using System.Collections.Generic;
using UnityEngine;

public class LinearSpringForceGraphv2 : MonoBehaviour
{
    public GameObject cubePrefab; // Prefab for the cube objects
    public int numberOfCubes = 10; // Number of cubes to create
    public float cubeSize = 1f; // Size of each cube
    public float springConstant = 10f; // Spring constant (stiffness of the springs)
    public float dampingFactor = 0.5f; // Damping factor (to control oscillations)
    public float restingDistance = 2f; // Resting distance between cubes

    private List<GameObject> cubes;
    private List<Vector3> initialPositions;

    void Start()
    {
        cubes = new List<GameObject>();
        initialPositions = new List<Vector3>();

        // Create cubes and add them to the cubes list
        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 position = Random.insideUnitSphere * 5f; // Random positions in a 10-unit radius sphere
            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
            cube.transform.localScale = Vector3.one * cubeSize;
            cubes.Add(cube);
            initialPositions.Add(position); // Store the initial positions
        }
    }

    void Update()
    {
        // Apply spring forces between adjacent cubes
        for (int i = 0; i < cubes.Count; i++)
        {
            for (int j = i + 1; j < cubes.Count; j++)
            {
                Vector3 forceDirection = cubes[j].transform.position - cubes[i].transform.position;
                float distance = forceDirection.magnitude;
                float displacement = distance - restingDistance;

                // Calculate the spring force based on Hooke's Law
                float springForce = springConstant * displacement;

                // Apply damping to reduce oscillations
                Vector3 relativeVelocity = cubes[j].GetComponent<Rigidbody>().velocity - cubes[i].GetComponent<Rigidbody>().velocity;
                float dampingForce = dampingFactor * Vector3.Dot(relativeVelocity, forceDirection.normalized);

                // Calculate the total force
                Vector3 totalForce = (springForce + dampingForce) * forceDirection.normalized;

                // Apply forces to the cubes
                cubes[i].GetComponent<Rigidbody>().AddForce(totalForce);
                cubes[j].GetComponent<Rigidbody>().AddForce(-totalForce);
            }
        }

        // Apply restoring force to bring cubes back to their initial positions
        for (int i = 0; i < cubes.Count; i++)
        {
            Vector3 restoringForce = springConstant * (initialPositions[i] - cubes[i].transform.position);
            cubes[i].GetComponent<Rigidbody>().AddForce(restoringForce);
        }
    }
}
