using UnityEngine;
using UnityEngine.InputSystem;

namespace Adam.Runtime.Input
{
    
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : InputHandler
    {

        [SerializeField] public Camera cam;

        private void Awake()
        {
            input.cam = cam;
        }

        public void OnMove(InputValue value)
        {
            input.moveInput = value.Get<Vector2>();
            Invoke();
        }
    }
    
}
