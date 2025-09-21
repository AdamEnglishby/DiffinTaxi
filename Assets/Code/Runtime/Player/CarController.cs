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

        public Rigidbody RigidbodyReference => _rigidbody;

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
            foreach (var visualEffect in smokeVfx)
            {
                if (angle >= smokeAngleThreshold && _rigidbody.linearVelocity.magnitude >= smokeSpeedThreshold)
                {
                    visualEffect.Play();
                }
                else
                {
                    visualEffect.Stop();
                }
            }
        }
        
    }
    
}
