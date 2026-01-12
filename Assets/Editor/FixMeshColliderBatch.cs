using UnityEngine;
using UnityEditor;

public class FixMeshColliderBatch
{
    [MenuItem("Tools/Lab2/Remove Concave MeshColliders")]
    static void RemoveConcaveMeshColliders()
    {
        int removed = 0;

        MeshCollider[] meshColliders =
            Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);

        foreach (MeshCollider mc in meshColliders)
        {
            if (!mc.convex)
            {
                Object.DestroyImmediate(mc);
                removed++;
            }
        }

        Debug.Log("[Lab2] Removed Concave MeshColliders: " + removed);
    }
}
