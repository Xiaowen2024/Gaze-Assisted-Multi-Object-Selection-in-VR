using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Define and manage functions for object selection and manipulation. 
    /// </summary>

    [RequireComponent(typeof(Transform))]
    public class SelectManager : MonoBehaviour
    {
        public InputActionReference gripReference = null;
        public InputActionReference triggerReference = null;
        public XRInteractionManager manager;
        private List<XRGrabInteractable> grabInteractables = new List<XRGrabInteractable>();
        public XRGazeInteractor gazeInteractor;
        private XRGrabInteractable interactable;
        public GameObject objectToSpawn;
        private Vector3 TargetRelativePosition;
        public GameObject centroidGameObject;

        private Dictionary<XRBaseInteractable, Transform> interactablesInitTransform =
            new Dictionary<XRBaseInteractable, Transform>();

        public Dictionary<XRBaseInteractable, GameObject> interactorsUsed =
            new Dictionary<XRBaseInteractable, GameObject>();

        [SerializeField] private bool isGriping = false;
        private Quaternion thisRotation;
        private Quaternion ReferenceRotation;
        private Quaternion diffVector;
        private Transform attachTransformCustomized;
        private Transform markerTransform;
        UnityEvent gazeHover = new UnityEvent();
        private SelectFeedback feedbackManager = new SelectFeedback();
        public GameObject marker; 
        private GameObject DistantSelectGameObject;
        [SerializeField] private GameObject rightControllerGameObject;

        private void OnEnable()
        {
            triggerReference.action.Enable();
            gripReference.action.Enable();
        }
        
        private void OnDisable()
        {
            triggerReference.action.Disable();
            gripReference.action.Disable();
        }

        void Start()
        {
            rightControllerGameObject.SetActive(true);
            gripReference.action.started += Grip;
            gripReference.action.canceled += deselectAll;
            triggerReference.action.started += Trigger;
            markerTransform = rightControllerGameObject.transform;
            DistantSelectGameObject = new GameObject("DistantSelect");
            DistantSelectGameObject.AddComponent<DistantSelect>();
        }

        // Update is called once per frame
        void Update()
        {
            if (gazeInteractor.interactablesHovered.Count != 0)
            {
                if (gazeInteractor.interactablesHovered[0] != null &&
                    gazeInteractor.interactablesHovered[0] is XRGrabInteractable)
                {
                    interactable = (XRGrabInteractable)gazeInteractor.interactablesHovered[0];
                    if (interactable.isSelected)
                    {
                        gazeHover.Invoke();
                    }
                }
            }
            else
            {
                interactable = null;
            }

            if (TargetRelativePosition != Vector3.zero)
            {
                thisRotation = rightControllerGameObject.transform.rotation;
                Quaternion q1Conjugate = Quaternion.Inverse(ReferenceRotation);
                diffVector = thisRotation * q1Conjugate;
                markerTransform.rotation = Quaternion.identity;
                markerTransform.rotation = markerTransform.rotation * diffVector;
                centroidGameObject.transform.position = rightControllerGameObject.transform.position + TargetRelativePosition;
                centroidGameObject.transform.rotation = rightControllerGameObject.transform.rotation;
                markerTransform.position = rightControllerGameObject.transform.position + TargetRelativePosition;
            }

            if (marker.activeSelf)
            {
                feedbackManager.UpdateFeedback(markerTransform, marker);
            }
        }

        /// <summary>
        /// Start select or deselect option when the trigger is pressed, depending on whether the object is selected right now.
        /// </summary>
        /// <param name="context"></param>
        private void Trigger(InputAction.CallbackContext context)
        {
            if (interactable != null && isGriping)
            {
                if (!interactable.isSelected)
                {
                    selectSingleObject(interactable);
                }
                else
                {
                    deselectSingleObject(interactable);
                }
            }
        }

        /// <summary>
        /// Deselect a single object and destroy its corresponding interactor.
        /// </summary>
        /// <param name="interactableToBeDeselected"> Interactable to be deselected. </param>
        private void deselectSingleObject(XRGrabInteractable interactableToBeDeselected)
        {
            if (interactableToBeDeselected != null && interactableToBeDeselected.isSelected &&
                interactableToBeDeselected.firstInteractorSelecting != null)
            {
                if (interactorsUsed.ContainsKey(interactableToBeDeselected))
                {
                    GameObject objectWithInteractor = interactorsUsed[interactableToBeDeselected];
                    XRDirectInteractor interactorToBeDestroyed =
                        objectWithInteractor.GetComponent<XRDirectInteractor>();
                    manager.SelectExit(interactorToBeDestroyed, interactableToBeDeselected);
                    Destroy(objectWithInteractor);
                    interactablesInitTransform.Remove(interactableToBeDeselected);
                    interactorsUsed.Remove(interactableToBeDeselected);
                    grabInteractables.Remove(interactableToBeDeselected);
                    if (isGriping)
                    {
                        calculateCentroid();
                    }
                }
            }
        }

        /// <summary>
        /// Deselect all the objects.
        /// </summary>
        /// <param name="context">  </param>
        private void deselectAll(InputAction.CallbackContext context)
        {
            isGriping = false;
            centroidGameObject.transform.DetachChildren();
            feedbackManager.DeactivateFeedback(marker);
            foreach (XRGrabInteractable grabInteractable in grabInteractables)
            {
                if (grabInteractable.isSelected)
                {
                    manager.SelectExit(grabInteractable.firstInteractorSelecting, grabInteractable);
                }
            }

            interactablesInitTransform.Clear();
            interactorsUsed.Clear();
            grabInteractables.Clear();
            //ds.UnscaleObjects();
            DistantSelectGameObject.GetComponent<DistantSelect>().UnscaleObjects();
        }

        /// <summary>
        /// Select a single object.
        /// </summary>
        /// <param name="interactableToBeSelected"> The interactable to be selected. </param>
        private void selectSingleObject(XRGrabInteractable interactableToBeSelected)
        {
            if (interactableToBeSelected != null)
            {
                if (interactorsUsed.ContainsKey(interactableToBeSelected))
                {
                    XRDirectInteractor interactor =
                        interactorsUsed[interactableToBeSelected].GetComponent<XRDirectInteractor>();
                    if (interactablesInitTransform.ContainsKey(interactableToBeSelected))
                    {
                        interactor.attachTransform.transform.position = interactablesInitTransform[interactableToBeSelected].position;
                        interactor.attachTransform.transform.rotation = interactablesInitTransform[interactableToBeSelected].rotation;
                    }
                    else
                    {
                        interactablesInitTransform.Add(interactableToBeSelected,
                            interactableToBeSelected.transform);
                        interactor.attachTransform = interactableToBeSelected.transform;
                    }

                    manager.SelectEnter(interactor, interactableToBeSelected);
                    grabInteractables.Add(interactableToBeSelected);
                    calculateCentroid();
                }
                else
                {
                    GameObject newObject = Instantiate(objectToSpawn);
                    Transform newAttachTransform = interactableToBeSelected.transform;
                    XRDirectInteractor interactor = newObject.GetComponent<XRDirectInteractor>();
                    interactorsUsed.Add(interactableToBeSelected, newObject);
                    if (interactablesInitTransform.ContainsKey(interactableToBeSelected))
                    {
                        interactor.attachTransform.position = interactablesInitTransform[interactableToBeSelected].position;
                        interactor.attachTransform.rotation = interactablesInitTransform[interactableToBeSelected].rotation;
                    }
                    else
                    {
                        interactablesInitTransform.Add(interactableToBeSelected,
                            interactableToBeSelected.transform);
                        interactor.attachTransform = interactableToBeSelected.transform;
                    }

                    manager.SelectEnter(newObject.GetComponent<XRDirectInteractor>(), interactableToBeSelected);
                    grabInteractables.Add(interactableToBeSelected);
                    calculateCentroid();
                }
            }
        }

        /// <summary>
        /// Start selecting an interactable when you press the grip button.
        /// </summary>
        /// <param name="context"></param>
        private void Grip(InputAction.CallbackContext context)
        {
            if (interactable != null)
            {
                isGriping = true;
                if (!interactable.isSelected)
                {
                    selectSingleObject(interactable);
                }
            }
        }

        /// <summary>
        /// Calculate the centroid of the gameObjects selected.
        /// </summary>
        private void calculateCentroid()
        {
            Vector3 centroidPosition = Vector3.zero;
            Quaternion centroidRotation = Quaternion.identity;
            int count = 0;
            centroidGameObject.transform.DetachChildren();
            foreach (XRGrabInteractable grabInteractable in grabInteractables)
            {
                if (grabInteractable.isSelected && interactablesInitTransform.ContainsKey(grabInteractable))
                {
                    centroidPosition += interactablesInitTransform[grabInteractable].position;
                    grabInteractable.transform.position = interactablesInitTransform[grabInteractable].position;
                    grabInteractable.transform.rotation= interactablesInitTransform[grabInteractable].rotation;
                    count += 1;
                }
            }

            if (count != 0)
            {
                centroidPosition /= count;
                TargetRelativePosition = centroidPosition - rightControllerGameObject.transform.position;
                ReferenceRotation = rightControllerGameObject.transform.rotation;
                centroidGameObject.transform.position = centroidPosition;
                markerTransform.rotation = centroidGameObject.transform.rotation;
                markerTransform.position = centroidGameObject.transform.position;
                foreach (XRGrabInteractable grabInteractable in grabInteractables)
                {
                    if (grabInteractable.isSelected)
                    {
                        if (grabInteractable.firstInteractorSelecting != null)
                        {
                            XRDirectInteractor thisInteractor =
                                (XRDirectInteractor)grabInteractable.firstInteractorSelecting;
                            thisInteractor.attachTransform.position = interactablesInitTransform[grabInteractable].position;
                            thisInteractor.attachTransform.rotation = interactablesInitTransform[grabInteractable].rotation;
                            thisInteractor.attachTransform.SetParent(centroidGameObject.transform);
                        }
                    }
                }
            }
            feedbackManager.ActivateFeedback(markerTransform, marker);
        }
    }
}

