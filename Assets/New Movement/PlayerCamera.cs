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
    [SerializeField] private float baseFOV = 80f;

    private float _fovSmoothSpeed;
    private float _targetFOV;

    float pitchMin = -85f; // look up limit (in degrees)
    float pitchMax = 85f; // look down limit (in degrees)

    private Vector3 _eulerAngles;
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
        cam.fieldOfView = baseFOV;
        _targetFOV = baseFOV;
        _fovSmoothSpeed = 5f;
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

    public void UpdateFOV(float t)
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _targetFOV, _fovSmoothSpeed * t);
    }

    public void RequestFOVChange(float requestedFOV, float requestedFOVSmoothSpeed)
    {
        _targetFOV = requestedFOV;
        _fovSmoothSpeed = requestedFOVSmoothSpeed;
    }
    public void ResetFOV()
    {
        _targetFOV = baseFOV;
        _fovSmoothSpeed = 5f;
    }
}
