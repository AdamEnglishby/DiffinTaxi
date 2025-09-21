using System;
using Adam.Runtime.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Adam.Runtime.Code.Runtime.Camera
{
    
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraDistanceModifier : MonoBehaviour
    {

        [SerializeField] private CarController carController;
        [SerializeField] private AnimationCurve velocityDistanceCurve;

        private CinemachineCamera _cinemachineCamera;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        private void LateUpdate()
        {
            var distance = velocityDistanceCurve.Evaluate(carController.RigidbodyReference.linearVelocity.magnitude);
            Debug.Log(distance);
        }
        
    }
    
}
