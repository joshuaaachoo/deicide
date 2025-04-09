using System;
using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [Space]
    [SerializeField] private float sensitivity = 0.1f;
    //[SerializeField] private float baseFOV = 60f;
    //[SerializeField] private float maxFOV = 80f;
    //[SerializeField] private float fovSmoothSpeed = 10f;

    float pitchMin = -80f; // look up limit (in degrees)
    float pitchMax = 80f; // look down limit (in degrees)

    private Vector3 _eulerAngles;
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }
    public Camera GetCam() => cam;
    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, pitchMin, pitchMax);
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }

    /* 
     * public void UpdateFOV(float t)
    {
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);
    }*
     */
}
