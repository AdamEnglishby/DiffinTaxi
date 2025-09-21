using System;
using UnityEngine;

namespace Adam.Runtime.Input
{
    
    public abstract class InputHandler : MonoBehaviour
    {

        public event Action OnInput;
        
        public InputState input;

        [Serializable]
        public struct InputState
        {
            public Camera cam;
            public Vector2 moveInput;
        }

        protected void Invoke()
        {
            OnInput?.Invoke();
        }

    }
    
}
