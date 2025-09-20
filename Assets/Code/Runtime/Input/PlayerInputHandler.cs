using UnityEngine;
using UnityEngine.InputSystem;

namespace Adam.Runtime.Input
{
    
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : InputHandler
    {
        
        public void OnMove(InputValue value) => Input.moveInput = value.Get<Vector2>();

    }
    
}
