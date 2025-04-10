using UnityEngine;
using System.Collections;
using KinematicCharacterController;

public class TetherController : MonoBehaviour
{
    public Transform tetherOrigin;      
    public LineRenderer lineRenderer;     
    public KinematicCharacterMotor motor;

    public void StartGrapple(Vector3 targetPoint, float pullSpeed)
    {
        StopAllCoroutines();
        if (targetPoint.y < transform.position.y)
        {
            StartCoroutine(GrappleRoutine_Down(targetPoint, pullSpeed));
        }
        else
        {
            StartCoroutine(GrappleRoutine_Normal(targetPoint, pullSpeed));
        }
    }

    private IEnumerator GrappleRoutine_Down(Vector3 targetPoint, float pullSpeed)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, targetPoint);
        float duration = distance / pullSpeed;
        float elapsed = 0f;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, tetherOrigin.position);
        lineRenderer.SetPosition(1, targetPoint);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPoint, t);
            motor.SetPosition(newPos);
            elapsed += Time.deltaTime;
            yield return null;
        }
        motor.SetPosition(targetPoint);
        lineRenderer.enabled = false;
    }

    private IEnumerator GrappleRoutine_Normal(Vector3 targetPoint, float pullSpeed)
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 velocity = direction * pullSpeed;
        motor.BaseVelocity = velocity;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, tetherOrigin.position);
        lineRenderer.SetPosition(1, targetPoint);

        yield return new WaitForSeconds(0.3f);
        lineRenderer.enabled = false;
    }
}
