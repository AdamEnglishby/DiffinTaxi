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
                
                var middleLeftRect = new Rect(middleRect.position.x, middleRect.position.y, middleRect.width / 3f, middleRect.height);
                var middleCentreRect = new Rect(middleRect.position.x + middleRect.width / 3f, middleRect.position.y, middleRect.width / 3f, middleRect.height);
                var middleRightRect = new Rect(middleRect.position.x + middleRect.width * 2 / 3f, middleRect.position.y, middleRect.width / 3f, middleRect.height);
                
                if (GUI.Button(middleLeftRect, "╣"))
                {
                    selectedKey = roadComponent.tLeft;
                    selectedEdge = edge;
                }
                
                if (GUI.Button(middleCentreRect, "╦"))
                {
                    selectedKey = roadComponent.tCentre;
                    selectedEdge = edge;
                }
                
                if (GUI.Button(middleRightRect, "╠"))
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
            _ = Instantiate(selectedKey, selectedEdge, roadComponent);

        }
        

        private async Task Instantiate(string key, Transform selectedEdge, RoadComponent oldRoadComponent)
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
            var o = (GameObject) PrefabUtility.InstantiatePrefab(prefab, selectedEdge.transform.parent.parent);
            
            Undo.RegisterCreatedObjectUndo(o, o.name);
            Addressables.Release(handle);
            
            var newRoadComponent = o.GetComponent<RoadComponent>();
            if (newRoadComponent && newRoadComponent.entryEdge)
            {
                var entryEdge = newRoadComponent.entryEdge;
                
                var targetRotation = selectedEdge.rotation;
                o.transform.rotation = targetRotation * Quaternion.Inverse(entryEdge.rotation);

                var worldSpaceOffset = entryEdge.position - o.transform.position;
                o.transform.position = selectedEdge.position - worldSpaceOffset;
            }
            
            Selection.SetActiveObjectWithContext(o, null);
            SceneView.RepaintAll(); 
        }

    }
    
}
