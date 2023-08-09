using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Determine whether objects in the scene are too small, if so, scale proportionally. 
    /// </summary>
    public class DistantSelect : MonoBehaviour
    {
        // Start is called before the first frame update
        private HashSet<GameObject> scaledObjects;
        private XRGazeInteractor gazeInteractor;
        private Vector3 prevHit = Vector3.zero;
        private Camera mainCamera;
        private float baseLine;
        void Start()
        {
            scaledObjects = new HashSet<GameObject>();
            gazeInteractor = FindObjectOfType<XRGazeInteractor>();
            mainCamera = Camera.main;
            baseLine = 1.0f / 12f;
        }

        // Update is called once per frame
        void Update()
        {
            if (gazeInteractor && gazeInteractor.enabled &&
                gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return;
                }
                float smoothingFactor = 0.2f;
                Vector3 hitPosition = hit.point;
                hitPosition = Vector3.Lerp(prevHit, hitPosition, smoothingFactor);
                prevHit = hitPosition;
                float searchRadius = 1f;
                List<Collider> colliders = Physics.OverlapSphere(hitPosition, searchRadius).ToList();
                colliders.Add(hit.collider);
                HashSet<GameObject> newGameObjects = new HashSet<GameObject>();
                foreach (var col in colliders)
                {
                    newGameObjects.Add(col.gameObject);
                }
                
                if (newGameObjects != scaledObjects || scaledObjects.Count == 0)
                {
                    unscaleOldObjects(colliders);
                    foreach (var collider in colliders)
                    {
                        if (!scaledObjects.Contains(collider.gameObject))
                        {
                            ScaleSingleObject(collider);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scale all the gameObjects back to their original scales.
        /// </summary>
        public void UnscaleObjects()
        {
            foreach (var objectToBeScaled in scaledObjects)
            {
                objectToBeScaled.transform.localScale =
                    objectToBeScaled.GetComponent<InitialTransform>().InitScale;
            }
            scaledObjects.Clear();
        }

        /// <summary>
        /// Scale objects that are not being scaled next back to their original scales. 
        /// </summary>
        /// <param name="colliders"> Colliders that are about to be scaled. </param>
        void unscaleOldObjects(List<Collider> colliders)
        {
            foreach (var objectToBeScaled in scaledObjects)
            {
                if (!colliders.Contains(objectToBeScaled.GetComponent<Collider>()))
                {
                    objectToBeScaled.transform.localScale =
                        objectToBeScaled.GetComponent<InitialTransform>().InitScale;
                }
            }
            scaledObjects.Clear();
        }

        /// <summary>-
        /// Scale a single object.
        /// </summary>
        /// <param name="collider"> The collider attached to the object to be scaled. </param>
        void ScaleSingleObject(Collider collider)
        {
            if (collider.gameObject.tag == "Environment")
            {
                return;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                return;
            }
            GameObject objectToBeScaled = collider.gameObject;
            if (!scaledObjects.Contains(objectToBeScaled))
            {
                if (objectToBeScaled.GetComponent<InitialTransform>() == null)
                {
                    return;
                }
                
                Vector3 originalScale = objectToBeScaled.GetComponent<InitialTransform>().InitScale;
                float distance = Vector3.Distance(objectToBeScaled.transform.position, mainCamera.transform.position);
                float height = collider.bounds.extents.y * 2;
                if ((height / distance) < baseLine)
                {
                    float newHeight = baseLine * distance;
                    Vector3 newScale = new Vector3(originalScale.x * newHeight/originalScale.y, newHeight, originalScale.z * newHeight / originalScale.y) ;
                    objectToBeScaled.transform.localScale = newScale;
                    scaledObjects.Add(objectToBeScaled);
                }
            }
        }
    }
}
