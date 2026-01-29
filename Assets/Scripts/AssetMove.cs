using UnityEngine;

public class AssetMove : MonoBehaviour
{
    [SerializeField] public Camera cam;
    public GameObject asset;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        asset.transform.forward = cam.transform.forward; // faces same direction as camera
    }
}
