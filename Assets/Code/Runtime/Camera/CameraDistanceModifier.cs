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
        [SerializeField] private AnimationCurve velocityNoiseCurve;
        [SerializeField] private float adjustmentSpeed = 15f;

        private CinemachineCamera _cinemachineCamera;
        private CinemachineBasicMultiChannelPerlin _noise;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _noise = _cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        }

        private void LateUpdate()
        {
            var velocity = carController.RigidbodyReference.linearVelocity.magnitude;
            var distance = velocityDistanceCurve.Evaluate(velocity);
            var target = Mathf.Lerp(_cinemachineCamera.Lens.OrthographicSize, distance, Time.deltaTime * adjustmentSpeed);
            _cinemachineCamera.Lens.OrthographicSize = target;

            _noise.AmplitudeGain = Mathf.Lerp(_noise.AmplitudeGain, velocityNoiseCurve.Evaluate(velocity), Time.deltaTime * adjustmentSpeed);
        }
        
    }
    
}
