using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using Unity.VisualScripting;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Find the closest object given a eye gaze raycast hit position. Make sure that one object is being hovered on at a time. 
    /// </summary>
    public class FindClosestNeighbour : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private XRGazeInteractor gazeInteractor;
        public KDTree kdtree = new KDTree();
        private XRInteractionManager manager;
        private List<Collider> closeColliders = new List<Collider>();
        private Collider closestCollider = null;
        private Collider prevCollider = null;
        private bool isHovering = false;
        private int hoverCount;
        private Vector3 prevHit = Vector3.zero;

        void Start()
        {
            manager = FindObjectOfType<XRInteractionManager>();
            gazeInteractor.allowHover = false;
            gazeInteractor.onHoverEntered.AddListener(OnHoverEntered);
            gazeInteractor.onHoverExited.AddListener(OnHoverExited);

        }

        // Update is called once per frame
        void Update()
        {
            if (closestCollider != null)
            {
                StartCoroutine(CheckClosest(closestCollider));
            }


            if (gazeInteractor && gazeInteractor.enabled &&
                gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return;
                }
                float smoothingFactor = 0.8f;
                Vector3 hitPosition = hit.point;
                if (prevHit == Vector3.zero)
                {
                    prevHit = hitPosition;
                }

                hitPosition = Vector3.Lerp(prevHit, hitPosition, smoothingFactor);
                prevHit = hitPosition;
                getClosest(hitPosition);
            }
        }

        /// <summary>
        /// Add one to the total number of interactables being hovered. 
        /// </summary>
        /// <param name="interactable"> The interactable hover enter on. </param>
        private void OnHoverEntered(XRBaseInteractable interactable)
        {
            hoverCount++;
        }

        /// <summary>
        /// Subtract one to the total number of interactables being hovered.
        /// </summary>
        /// <param name="interactable"> The interactable hover exit on. </param>
        private void OnHoverExited(XRBaseInteractable interactable)
        {
            hoverCount--;
        }

        /// <summary>
        /// Set the closest collider variable to the closest one to the hit position.
        /// </summary>
        /// <param name="hitPosition"> The position of eye gaze hit. </param>
        void getClosest(Vector3 hitPosition)
        {
            float searchRadius = 1f;
            closeColliders = Physics.OverlapSphere(hitPosition, searchRadius).ToList();
            if (closeColliders.Count > 0)
            {
                kdtree.Build(closeColliders);
                closestCollider = kdtree.FindNearestNeighbor(hitPosition);
            }
            else
            {
                closestCollider = null;
            }
        }

        /// <summary>
        ///  Check if the closest collider to the hit position has changed, if so update the closest collider variable.
        /// </summary>
        /// <param name="collider"> The closest collider. </param>
        /// <returns></returns>
        IEnumerator CheckClosest(Collider collider)
        {
            if (collider != null)
            {
                HoverClosest(collider);
                prevCollider = collider;
            }

            yield return new WaitUntil(() => closestCollider != prevCollider);
        }

        /// <summary>
        /// Hover the closest.
        /// </summary>
        /// <param name="collider"> The closest collider to be hovered. </param>
        void HoverClosest(Collider collider)
        {
            if (collider.gameObject.tag == "Environment")
            {
                return;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                return;
            }
            if (collider != null)
            {
                if (collider.gameObject.GetComponent<XRGrabInteractable>() != null && collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    manager.HoverEnter(gazeInteractor, collider.gameObject.GetComponent<XRGrabInteractable>());
                }
            }
        }
    }
}