using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Activate Gaze Interactors and controllers in the scene. 
    /// </summary>
    public class ActivateGazeInteractor : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private XRGazeInteractor GazeInteractor;
        [SerializeField] private GameObject leftControllerObject;
        [SerializeField] private GameObject rightControllerObject;

        void Start()
        {
            GazeInteractor.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            leftControllerObject.gameObject.SetActive(true);
            rightControllerObject.gameObject.SetActive(true);
        }
    }
}
