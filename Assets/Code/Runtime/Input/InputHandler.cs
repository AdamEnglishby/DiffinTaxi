using System;
using UnityEngine;

namespace Adam.Runtime.Input
{
    
    public abstract class InputHandler : MonoBehaviour
    {

        protected InputState Input;
        public InputState GetInput => Input;

        [Serializable]
        public struct InputState
        {
            public Vector2 moveInput;
        }

    }
    
}
