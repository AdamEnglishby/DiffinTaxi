using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Adam.Runtime
{
    
    public class Menu : MonoBehaviour
    {
        private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");

        [SerializeField] public List<Material> materials;
        [SerializeField] private UIDocument document;

        private void Awake()
        {
            document.rootVisualElement.Q<Button>("start").clicked += OnStart;
            document.rootVisualElement.Q<Button>("quit").clicked += OnQuit;
        }

        private void Start()
        {
            foreach (var material in materials)
            {
                material.SetVector(CutoutPos, new Vector2(-100, -100));
            }
        }

        private void OnQuit()
        {
            Application.Quit();
        }

        private void OnStart()
        {
            SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        }
        
    }
    
}
