using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Store the initial transform for each XR Grab interactable this script is attached to. 
    /// </summary>
    public class InitialTransform : MonoBehaviour
    {
        // Start is called before the first frame update
        private Transform prev;
        private UnityEngine.Vector3 initScale;
        private UnityEngine.Vector3 initPosition;

        public Transform Prev
        {
            get => prev;
            set => prev = value;
        }
        
        public UnityEngine.Vector3 InitScale
        {
            get => initScale;
            set => initScale = value;
        }
        
        public UnityEngine.Vector3 InitPosition
        {
            get => initPosition;
            set => initPosition = value;
        }

        void Start()
        {
            prev = this.transform;
            InitScale = this.transform.localScale;
            InitPosition = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
