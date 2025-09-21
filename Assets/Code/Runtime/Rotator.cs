using UnityEngine;

namespace Adam.Runtime
{
    
    public class Rotator : MonoBehaviour
    {

        [SerializeField] private float rotationSpeed = 45f;

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
    }
    
}
