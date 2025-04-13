using UnityEngine;
using System.Collections;

public class Tether : AbilityBasic, IMobilityAbility
{
    private float range = 2000f;
    private float pullSpeed = 20f;
    private float pullAcceleration = 10f;
    private float swingArcHeight = 2f; // maybe if i wanna do arcs
    private Vector3 pullDirection;
    private Vector3 tetherPoint;

    private Transform lineOrigin;
    private LineRenderer lineRenderer;
    private float pullSpeedDynamic;
    public void ActivateMobility() => Activate();
    public void TickMobility(float dt) => Tick(dt);
    public void DeactivateMobility() => Deactivate();

    protected override void OnActivate()
    {
        // on activate

        // gets cam
        Camera cam = player.GetCamera().GetCam();
        if(cam == null)
        {
            Debug.LogError("no cam!");
            return;
        }
        Debug.Log("Tether activated");

        #region Line Renderer settings
        lineRenderer = 
            lineRenderer == null ? character.gameObject.AddComponent<LineRenderer>() 
            : character.gameObject.GetComponent<LineRenderer>();

        var tetherMat = Resources.Load<Material>("Material/M_TetherLine");
        lineRenderer.material = tetherMat; // finds M_TetherLine material and uses it for line renderer
        lineRenderer.loop = false;

        lineRenderer.positionCount = 3;

        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;

        Gradient tetherGradient = new Gradient();
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(new Color(r: 27, g: 248, b: 227), 0.0f);
        colors[1] = new GradientColorKey(Color.white, 1.0f);
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        tetherGradient.SetKeys(colors, alphas);

        lineRenderer.colorGradient = tetherGradient;

        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.textureScale = new Vector2(1f, 1f);
        lineRenderer.shadowBias = 0.5f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.maskInteraction = SpriteMaskInteraction.None;

        lineRenderer.enabled = false;
        #endregion

        lineOrigin = cam.transform;
        Vector3 origin = lineOrigin.position;
        pullDirection = lineOrigin.forward;

        if (Physics.Raycast(origin, pullDirection, out RaycastHit hit, range, LayerMask.GetMask("Terrain", "Wall", "Platform")))
        {
            Debug.Log("tether hit: " + hit.collider.name);
            tetherPoint = hit.point;
            var distance = Vector3.Distance(origin, tetherPoint);

            // var duration = distance / pullSpeed;

            float a = 0.5f * pullAcceleration;
            float b = pullSpeed;
            float c = -distance;
            float sqrtDiscriminant = Mathf.Sqrt(b * b - 4 * a * c);

            float t1 = (-b + sqrtDiscriminant) / (2 * a);
            float t2 = (-b - sqrtDiscriminant) / (2 * a);

            var duration = Mathf.Max(t1, t2);

            data.activeTime = duration;

            lineRenderer.enabled = true;
            player.GetCamera().RequestFOVChange(100f, 5f);

            pullSpeedDynamic = pullSpeed;

            successful = true;
        }
        else
        {
            Debug.Log("no valid target hit");
            successful = false;
        }
    }
    protected override void OnTick(float deltaTime)
    {
        // updates every tick of active time
        pullSpeedDynamic += pullAcceleration * deltaTime;
        character.InjectExternalVelocity(pullSpeedDynamic * pullDirection, deltaTime, false); // vf = vi + at

        // Debug.Log($"Current pull speed: {pullSpeedDynamic}");

        lineRenderer.SetPosition(0, (lineOrigin.position + new Vector3(-1f, -1.869f, 0f)));
        lineRenderer.SetPosition(1, tetherPoint);
        lineRenderer.SetPosition(2, (lineOrigin.position + new Vector3(1f, -1.869f, 0f)));

        character.GetMotor().ForceUnground();
    }
    protected override void OnDeactivate()
    {
        // on deactivate
        Debug.Log("Tether finished");
        pullSpeedDynamic = pullSpeed;
        lineRenderer.enabled = false;
        player.GetCamera().ResetFOV();
    }
}
