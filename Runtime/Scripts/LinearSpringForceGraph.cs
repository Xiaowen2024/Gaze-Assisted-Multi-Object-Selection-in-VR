using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Constructor for a force graph along with functions to apply forces to occluded objects so that they expand and avoid occlusion. 
    /// </summary>
    public class LinearSpringForceGraph : MonoBehaviour
    {
        private int numberOfObjects;
        public float springConstant = 10f;
        public float dampingFactor = 0.7f;
        public float restingDistance = 2f;

        private List<GameObject> objects = new List<GameObject>();

        public List<GameObject> ObjectsToBeExpanded
        {
            get => objects;
            set => objects = value;
        }

        private GameObject closest;

        public GameObject Closest
        {
            get => closest;
            set => closest = value;
        }

        private Camera MainCamera;
        private Dictionary<GameObject, Vector3> newPositions;

        public Dictionary<GameObject, Vector3> NewPositions
        {
            get => newPositions;
            set => newPositions = value;
        }

        private bool expandEnabled = false; 
        public bool ExpandEnabled
        {
            get => expandEnabled;
            set => expandEnabled = value;
        }
        
        void Start()
        {
            newPositions = new Dictionary<GameObject, Vector3>();
            MainCamera = Camera.main;
        }

        void FixedUpdate()
        {
            if (expandEnabled)
            {
                applyForces();
            }
        }

        /// <summary>
        /// Apply forces to the occluded objects so that they are not occluded any more. 
        /// </summary>
        void applyForces()
        {
            if (closest == null) return;
            int closestIndex = objects.IndexOf(closest);
            Vector3 directionVector = MainCamera.transform.position - objects[closestIndex].transform.position;
            numberOfObjects = objects.Count;
            float angle = 180f / (numberOfObjects - 2);
            float offset = 1f;
            Vector3 objectDirection = Vector3.zero;
            for (int i = 0; i < objects.Count; i++)
            {
                if (closestIndex == i) continue;
                if (newPositions.ContainsKey(objects[i]))
                {
                    continue;
                }
                float minDistance = (objects[i].GetComponent<Renderer>().bounds.extents.magnitude/Mathf.Tan(angle))/2 +
                                    objects[closestIndex].GetComponent<Renderer>().bounds.extents.magnitude/2 + offset;
                float objectAngle = 0f;
                Vector3 upDirection = transform.up;
                if (i < closestIndex)
                {
                    objectAngle = angle * i - 90f;
                }
                else
                {
                    objectAngle = angle * (i - 1) - 90f;
                }
                if (Mathf.Max(Math.Abs(directionVector.x), Math.Abs(directionVector.y), Math.Abs(directionVector.z)) == Math.Abs(directionVector.x))
                {
                    objectDirection = Quaternion.Euler(objectAngle, 0f, 0f) * upDirection * minDistance;
                }
                else if (Mathf.Max(Math.Abs(directionVector.x), Math.Abs(directionVector.y), Math.Abs(directionVector.z)) == Math.Abs(directionVector.z))
                {
                    objectDirection = Quaternion.Euler( 0f, 0f,objectAngle) * upDirection * minDistance;
                }
                else
                {
                    objectDirection = Quaternion.Euler (0f,objectAngle, 0f) * upDirection * minDistance;
                }

                if (objects[i].GetComponent<InitialTransform>() == null)
                {
                    return;
                }
                objects[i].GetComponent<InitialTransform>().InitPosition = objects[i].transform.position;
                objects[i].transform.position = objects[closestIndex].transform.position + objectDirection;
                newPositions.Add(objects[i], objects[i].transform.position);
                float distance = objectDirection.magnitude;
                float displacement = distance - restingDistance;
                float springForce = springConstant * displacement;
                Vector3 relativeVelocity = objects[closestIndex].GetComponent<Rigidbody>().velocity -
                                           objects[i].GetComponent<Rigidbody>().velocity;
                float dampingForce = dampingFactor * Vector3.Dot(relativeVelocity, objectDirection.normalized);
                Vector3 totalForce = (springForce + dampingForce) * objectDirection.normalized;
                objects[i].GetComponent<Rigidbody>().AddForce(totalForce);
            }
        }
    }
}
