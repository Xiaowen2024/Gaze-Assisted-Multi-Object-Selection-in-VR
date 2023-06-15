using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TriggerSelectAction : MonoBehaviour
{
    private Transform attachTransformCustomized;
    public InputActionReference triggerReference = null;
    public XRGazeInteractor gazeInteractor;
    private XRGrabInteractable interactable;
    private XRDirectInteractor interactor;
    public XRInteractionManager manager;
    private Vector3 TargetRelativePosition;
    

    // Start is called before the first frame update
    private void Awake()
    {
        triggerReference.action.started += Trigger;
        interactor = gameObject.GetComponent<XRDirectInteractor>();
        
    }

    private void Update()
    {

        if (interactable != null)
        {
            attachTransformCustomized = interactable.transform;
            if (interactable.isSelected && TargetRelativePosition != Vector3.zero)
            {
                attachTransformCustomized.position = this.transform.position + TargetRelativePosition;
                attachTransformCustomized.rotation = this.transform.rotation;
                // attachTransformCustomized.position = this.transform.InverseTransformPoint(attachTransformCustomized.position);
                gameObject.GetComponent<XRDirectInteractor>().attachTransform = attachTransformCustomized;
        
            }
        }
       

        if (gazeInteractor.interactablesHovered.Count != 0)
        {
            if (gazeInteractor.interactablesHovered[0] != null && gazeInteractor.interactablesHovered[0] is XRGrabInteractable)
            {
                interactable = (XRGrabInteractable)gazeInteractor.interactablesHovered[0];
            }
        }
    }

    // private void OnDestroy()
    // {
    //     triggerReference.action.canceled -= Trigger;
    // }
    
    // when grip trigger pressed, select the object if it has not been selected, deselect it otherwise
    private void Trigger(InputAction.CallbackContext context) {
        if (interactable != null)
        {
            if (!interactable.isSelected)
            {
                TargetRelativePosition = interactable.transform.position - this.transform.position;
                manager.SelectEnter(interactor, interactable);
            }
            else
            {
                manager.SelectExit(interactor, interactable);
            }
        }
    }
}
