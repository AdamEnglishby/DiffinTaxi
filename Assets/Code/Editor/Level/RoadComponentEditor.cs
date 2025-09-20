using System;
using System.Threading.Tasks;
using Adam.Runtime.Level;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Adam.Editor.Level
{
    
    [CustomEditor(typeof(RoadComponent))]
    public class RoadComponentEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            SceneView.duringSceneGui += SceneViewGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewGUI;
        }

        private void SceneViewGUI(SceneView obj)
        {
            var roadComponent = (RoadComponent) target;
            
            Handles.BeginGUI();

            var selectedKey = "";
            Transform selectedEdge = null;
            
            foreach (var edge in roadComponent.exitEdges)
            {
                if(!edge) continue;
                var guiPosition = HandleUtility.WorldToGUIPoint(edge.transform.position);
                var totalRect = new Rect(guiPosition, new Vector2(100, EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2));

                var topRect = new Rect(totalRect.position.x, totalRect.position.y, totalRect.size.x, totalRect.size.y / 3);
                var middleRect = new Rect(totalRect.position.x, totalRect.position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, totalRect.size.x, totalRect.size.y / 3);
                var bottomRect = new Rect(totalRect.position.x, totalRect.position.y + EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2, totalRect.size.x, totalRect.size.y / 3);

                if (GUI.Button(topRect, "╬"))
                {
                    selectedKey = roadComponent.crossSection;
                    selectedEdge = edge;
                }
                
                var middleLeft = new Rect(middleRect.position.x, middleRect.position.y, middleRect.width / 2f, middleRect.height);
                var middleRight = new Rect(middleRect.position.x + middleRect.width / 2f, middleRect.position.y, middleRect.width / 2f, middleRect.height);
                
                if (GUI.Button(middleLeft, "╣"))
                {
                    selectedKey = roadComponent.tLeft;
                    selectedEdge = edge;
                }
                
                if (GUI.Button(middleRight, "╠"))
                {
                    selectedKey = roadComponent.tRight;
                    selectedEdge = edge;
                }
                
                var bottomLeftRect = new Rect(bottomRect.position.x, bottomRect.position.y, bottomRect.width / 3f, bottomRect.height);
                var bottomCentreRect = new Rect(bottomRect.position.x + bottomRect.width / 3f, bottomRect.position.y, bottomRect.width / 3f, bottomRect.height);
                var bottomRightRect = new Rect(bottomRect.position.x + bottomRect.width * 2 / 3f, bottomRect.position.y, bottomRect.width / 3f, bottomRect.height);

                if (GUI.Button(bottomLeftRect, "╗"))
                {
                    selectedKey = roadComponent.left;
                    selectedEdge = edge;
                }
                
                if (GUI.Button(bottomCentreRect, "║"))
                {
                    selectedKey = roadComponent.straight;
                    selectedEdge = edge;
                }
                
                if (GUI.Button(bottomRightRect, "╔"))
                {
                    selectedKey = roadComponent.right;
                    selectedEdge = edge;
                }
            }
 
            Handles.EndGUI();

            if (selectedKey == "" || !selectedEdge) return;
            
            SceneView.duringSceneGui -= SceneViewGUI;
            _ = Instantiate(selectedKey, selectedEdge);

        }
        

        private async Task Instantiate(string key, Transform selectedEdge)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            await handle.Task;

            if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load prefab with key: {key}");
                Addressables.Release(handle);
                return;
            }

            var prefab = handle.Result;
            var o = (GameObject)PrefabUtility.InstantiatePrefab(prefab, selectedEdge.transform.parent.parent);
            
            Undo.RegisterCreatedObjectUndo(o, o.name);
            
            o.transform.position = selectedEdge.transform.position;
            o.transform.rotation = selectedEdge.transform.rotation;
            
            var correspondingEdge = o.GetComponent<RoadComponent>().entryEdge; // TODO: how can we find the correct edge every time, instead of assuming #0?
            var offset = correspondingEdge.transform.position - selectedEdge.transform.position;
            o.transform.position = selectedEdge.transform.position - offset;
            
            Addressables.Release(handle);
            
            EditorApplication.delayCall += () =>
            {
                Selection.SetActiveObjectWithContext(o, null);
            };
        }

    }
    
}
