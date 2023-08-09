using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Define and manage visual feedback for selection and manipulation interactions. 
    /// </summary>
    public class SelectFeedback : MonoBehaviour
    {
        /// <summary>
        /// Set the feedback marker to be active. 
        /// </summary>
        /// <param name="newtransform"> The new transform of the marker. </param>
        /// <param name="marker"> The marker object to be set active. </param>
        public void ActivateFeedback(Transform newtransform, GameObject marker)
        {
            marker.SetActive(true);
        }

        /// <summary>
        /// Set the feedback marker to be inactive. 
        /// </summary>
        /// <param name="marker"> The marker object to be set inactive. </param>
        public void DeactivateFeedback(GameObject marker)
        {
            marker.SetActive(false);
        }

        /// <summary>
        /// Update the transform of the feedback marker.
        /// </summary>
        /// <param name="newtransform"> The new transform the marker will update to. </param>
        /// <param name="marker"> The marker object to be updated. </param>
        public void UpdateFeedback(Transform newtransform, GameObject marker)
        {
            marker.transform.position = newtransform.position;
            marker.transform.rotation = newtransform.rotation;
        }
    }
}