using Adam.Runtime.Level;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Adam.Editor.Level
{
    
    [CustomEditor(typeof(BuildingScatterer))]
    public class BuildingScattererEditor : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var scatterer = (BuildingScatterer) target;
            var scatteredBuildingsParent = scatterer.scatteredBuildingsParent;
            
            if (GUILayout.Button("Clear all"))
            {
                for (var i = scatteredBuildingsParent.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(scatteredBuildingsParent.GetChild(i).gameObject);
                }
            }
            
            if (!GUILayout.Button("Scatter")) return;
            
            var planeToScatterOn = scatterer.planeToScatterOn;
            var buildingPrefabs = scatterer.prefabsToScatter;
            var numberOfBuildings = scatterer.numberOfBuildings;
            var maxPlacementAttempts = scatterer.maxPlacementAttempts;

            for (var i = scatteredBuildingsParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(scatteredBuildingsParent.GetChild(i).gameObject);
            }

            var bounds = planeToScatterOn.GetComponent<Collider>().bounds;

            for (var i = 0; i < numberOfBuildings; i++)
            {
                var placementAttempts = 0;
                var placed = false;

                while (placementAttempts < maxPlacementAttempts && !placed)
                {
                    placementAttempts++;

                    var randomPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
                    var randomPosition = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        bounds.center.y,
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    if (NavMesh.SamplePosition(randomPosition, out var hit, 100.0f, NavMesh.AllAreas))
                    {
                        var buildingBounds = randomPrefab.GetComponent<Renderer>().bounds;
                        var halfExtents = buildingBounds.size / 2f;

                        if (!Physics.CheckBox(hit.position + buildingBounds.center, halfExtents, Quaternion.identity))
                        {
                            var newBuilding = (GameObject)PrefabUtility.InstantiatePrefab(randomPrefab, scatteredBuildingsParent);
                            newBuilding.transform.position = hit.position;
                            newBuilding.isStatic = true;
                            placed = true;
                        }
                    }
                }

                if (!placed)
                {
                    Debug.LogWarning($"Failed to place building {i + 1} after {maxPlacementAttempts} attempts.");
                }
            }
        }
        
    }
    
}
