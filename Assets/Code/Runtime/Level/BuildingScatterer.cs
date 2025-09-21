using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Adam.Runtime.Level
{
    
    public class BuildingScatterer : MonoBehaviour
    {

        [SerializeField] public GameObject planeToScatterOn;
        [SerializeField] public List<GameObject> prefabsToScatter;
        [SerializeField] public int numberOfBuildings = 50;
        [SerializeField] public Transform scatteredBuildingsParent;
        [SerializeField] public int maxPlacementAttempts = 100;

        [SerializeField] public NavMeshSurface navMeshSurface;

    }
    
}
