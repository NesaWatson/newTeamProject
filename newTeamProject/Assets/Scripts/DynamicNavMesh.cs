using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour
{
    public NavMeshData navMeshData;

    void Start()
    {
        UpdateNavMesh();
    }

    void UpdateNavMesh()
    {
        NavMeshDataInstance dataInstance = NavMesh.AddNavMeshData(navMeshData);

        dataInstance.Remove();
    }
}
