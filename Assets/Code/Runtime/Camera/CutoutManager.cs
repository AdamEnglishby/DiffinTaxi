using System;
using System.Collections.Generic;
using UnityEngine;

namespace Adam.Runtime.Code.Runtime.Camera
{
    
    public class CutoutManager : MonoBehaviour
    {
        private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");

        [SerializeField] private UnityEngine.Camera cam;
        [SerializeField] private List<Material> materials;

        private void Update()
        {
            var cutoutPos = cam.WorldToViewportPoint(transform.position);

            foreach (var material in materials)
            {
                material.SetVector(CutoutPos, cutoutPos);
            }
        }
    }
    
}
