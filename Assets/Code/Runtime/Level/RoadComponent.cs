using System.Collections.Generic;
using UnityEngine;

namespace Adam.Runtime.Level
{
    
    public class RoadComponent : MonoBehaviour
    {

        [SerializeField] public Transform entryEdge;
        [SerializeField] public List<Transform> exitEdges;
        [SerializeField] public string crossSection, tLeft, tRight, left, straight, right;

    }
    
}
