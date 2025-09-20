using System;
using UnityEngine;

namespace Adam.Runtime.Input
{
    
    public abstract class InputHandler : MonoBehaviour
    {

        public InputState input;

        [Serializable]
        public struct InputState
        {
            public Camera cam;
            public Vector2 moveInput;
        }

    }
    
}
