using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private float targetAspect = 16.0f / 9.0f;

    void Start()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = targetAspect / windowAspect;

        Camera.main.orthographicSize = scaleHeight >= 1.0f ? 5 : 5 / scaleHeight; // Atur angka 5 sesuai kebutuhan Anda
    }
}
