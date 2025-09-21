using System;
using System.Collections.Generic;
using Adam.Runtime.Input;
using UnityEngine;
using UnityEngine.VFX;

namespace Adam.Runtime.Player
{
    
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {

        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private Transform meshRoot;
        [SerializeField] private List<VisualEffect> smokeVfx;
        
        [SerializeField] private float moveForceScalar = 10f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Vector3 meshOffset = new(0, -0.5f, 0);
        [SerializeField] private AnimationCurve forceByAngleCurve;
        [SerializeField] private float smokeAngleThreshold = 45f;
        [SerializeField] private float smokeSpeedThreshold = 1f;
        [SerializeField] private VisualEffect exhaustVfx;

        private Rigidbody _rigidbody;
        private Vector3 _lastMovementInput;
        private bool _drifting;

        public Rigidbody RigidbodyReference => _rigidbody;

        public event Action OnDriftStart;
        public event Action OnDriftEnd;
        public event Action<Cone> OnConeHit;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = maxSpeed;
            
            foreach (var visualEffect in smokeVfx)
            {
                visualEffect.gameObject.SetActive(true);
                visualEffect.Stop();
            }
            exhaustVfx.gameObject.SetActive(true);

            OnDriftStart += () =>
            {
                foreach (var visualEffect in smokeVfx)
                {
                    visualEffect.Play();
                }
            };
            
            OnDriftEnd += () =>
            {
                foreach (var visualEffect in smokeVfx)
                {
                    visualEffect.Stop();
                }
            };
        }

        private void FixedUpdate()
        {
            var moveInput = inputHandler.input.moveInput;
            var camTransform = inputHandler.input.cam.transform;

            var camForward = camTransform.forward;
            var camRight = camTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward = camForward.normalized;
            camRight = camRight.normalized;

            var worldSpaceInput = (camForward * moveInput.y + camRight * moveInput.x);
            
            if (worldSpaceInput.magnitude > 0.01f)
            {
                _lastMovementInput = worldSpaceInput;
            }
            
            var angle = Vector3.Angle(_lastMovementInput, meshRoot.forward);
            var weightedForce = forceByAngleCurve.Evaluate(angle);
            
            _rigidbody.AddForce(weightedForce * moveForceScalar * worldSpaceInput);
        }

        private void Update()
        {
            var targetRotation = Quaternion.identity;
            if (_lastMovementInput != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(_lastMovementInput);
            }
            
            var newRotation = Quaternion.Slerp(meshRoot.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            meshRoot.transform.position = transform.position + meshOffset;
            meshRoot.transform.SetPositionAndRotation(transform.position + meshOffset, newRotation);
            
            var angle = Vector3.Angle(_rigidbody.linearVelocity, meshRoot.forward);
            if (angle >= smokeAngleThreshold && _rigidbody.linearVelocity.magnitude >= smokeSpeedThreshold)
            {
                if (_drifting) return;
                _drifting = true;
                OnDriftStart?.Invoke();
            }
            else
            {
                if (!_drifting) return;
                _drifting = false;
                OnDriftEnd?.Invoke();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Cone"))
            {
                OnConeHit?.Invoke(other.gameObject.GetComponent<Cone>());
            }
        }
        
    }
    
}
