using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Identify occluded objects in the scene and call functions to expand the objects. 
    /// </summary>
    public class OccludedObjectsSelectionAssistance : MonoBehaviour
    {
        XRGazeInteractor gazeInteractor;
        private Vector3 prevHit = Vector3.zero;
        private Camera mainCamera;
        public InputActionReference ExpandReference;
        private Dictionary<GameObject, Transform> initialTransforms;
        private LinearSpringForceGraph forceGraph;
        private RaycastHit closestHit;
        private GameObject closestGameObject;
        private List<GameObject> objectsToBeScattered;
        
        private void OnEnable()
        {
            ExpandReference.action.Enable();
        }

        private void OnDisable()
        {
            ExpandReference.action.Disable();
        }

        void Start()
        {
            mainCamera = Camera.main;
            gazeInteractor = FindObjectOfType<XRGazeInteractor>();
            ExpandReference.action.started += Expand;
            ExpandReference.action.canceled += cancelExpand;
            initialTransforms = new Dictionary<GameObject, Transform>();
            forceGraph = this.GetComponent<LinearSpringForceGraph>();
            forceGraph.enabled = true;
            objectsToBeScattered = new List<GameObject>();
        }
        
        void Update()
        {
        }

        /// <summary>
        /// Find all occluded objects in the area of my eye gaze.
        /// </summary>
        void FindOccludedGameobjects()
        {
            if (gazeInteractor && gazeInteractor.enabled &&
                gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                float smoothingFactor = 0.8f;
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return;

                }
                
                Vector3 hitPosition = hit.point;
                hitPosition = Vector3.Lerp(prevHit, hitPosition, smoothingFactor);
                closestHit = hit;
                Vector3 newRayDirection = hitPosition - mainCamera.transform.position;
                float sphereRadius = 2f;
                float castDistance = 10f;
                
                List<Collider> colliders =  Physics.OverlapSphere(hitPosition, sphereRadius).ToList();
                foreach (var col in  colliders)
                {
                    if (!objectsToBeScattered.Contains(col.gameObject) && !initialTransforms.ContainsKey(col.gameObject)
                        && col.gameObject.layer !=  LayerMask.NameToLayer("Ignore Raycast")
                       )
                    {
                        objectsToBeScattered.Add(col.gameObject);
                        initialTransforms.Add(col.gameObject, col.gameObject.transform);
                    }
                }
            }
        }
        /// <summary>
        /// Expand the occluded so that they are visible when the primary button is pressed. 
        /// </summary>
        /// <param name="context"></param>
        void Expand(InputAction.CallbackContext context)
        {
            FindOccludedGameobjects();
            if (objectsToBeScattered != null && objectsToBeScattered.Count > 1)
            {
                forceGraph.Closest = findClosestGameObject();
                forceGraph.ObjectsToBeExpanded = objectsToBeScattered;
                forceGraph.ExpandEnabled = true;
            }
            else if (objectsToBeScattered.Count == 1)
            {
                Debug.Log("No Occlusion Found");
                return;
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// Find the object closest to my eye gaze hit position. 
        /// </summary>
        /// <returns></returns>
        GameObject findClosestGameObject()
        {
            float closestDistance = Mathf.Infinity;
            foreach (var obj in objectsToBeScattered)
            {
                if (Vector3.Distance(obj.transform.position, closestHit.collider.gameObject.transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(obj.transform.position, closestHit.collider.gameObject.transform.position);
                    closestGameObject = obj;
                }
            }
            return closestGameObject;
        }
        /// <summary>
        /// Move the objects back to their original position once the primary button is released. 
        /// </summary>
        /// <param name="context"></param>
        void cancelExpand(InputAction.CallbackContext context)
        {
            forceGraph.ExpandEnabled = false;
            forceGraph.ObjectsToBeExpanded = new List<GameObject>();
            forceGraph.NewPositions = new Dictionary<GameObject, Vector3>();
            foreach (var obj in objectsToBeScattered)
            {
                // if (obj.GetComponent<InitialTransform>() == null)
                // {
                //     Debug.Log(obj.name);
                //     Debug.Log("no initial transform");
                //     return;
                // }
                obj.transform.position = obj.GetComponent<InitialTransform>().InitPosition;
            }
            objectsToBeScattered.Clear();
            initialTransforms.Clear();
        }
    }
}

