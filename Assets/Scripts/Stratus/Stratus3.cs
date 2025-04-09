using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Abilities/Tether")]
public class Stratus3 : Ability
{
    public float range = 15f;
    public float pullSpeed = 20f;
    public float swingArcHeight = 2f; // maybe if i wanna do arcs 

    public override void Activate(GameObject user)
    {
        // gets cam
        Camera cam = user.transform.root.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("no cam!");
            return;
        }
        
        Transform cameraTransform = cam.transform;
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, LayerMask.GetMask("Tetherable")))
        {
            Debug.Log("tether hit: " + hit.collider.name);
            TetherController tether = user.GetComponent<TetherController>();
            if (tether != null)
            {
                tether.StartGrapple(hit.point, pullSpeed);
            }
        }
        else
        {
            Debug.Log("no valid target hit");
        }
    }
}
