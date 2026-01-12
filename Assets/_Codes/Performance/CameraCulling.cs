using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Create a plane representing the camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Use the new API and avoid the sort overhead by requesting no sorting.
        foreach (var obj in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            // Enable rendering only when inside the camera's view frustum
            obj.enabled = GeometryUtility.TestPlanesAABB(planes, obj.bounds);
        }
    }
}