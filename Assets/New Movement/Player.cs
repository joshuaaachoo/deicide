/*  IMPORTANT NOTES (from Iggy):
 *  You can only look up and down max 80 degrees but you can change this in PlayerCamera
 *  Looking to add: Dynamic FOV, shift dash
 */

using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] public AbilitySet abilitySet;
    [SerializeField] public CharacterDefinition selectedCharacter;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;

    private PlayerInputActions _inputActions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize(abilitySet, selectedCharacter);
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        // cameraSpring.Initialize();

        abilitySet.Initialize(this, playerCharacter, selectedCharacter);
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();

        // get camera input and update its rotation
        var cameraInput = new CameraInput
        {
            Look = input.Look.ReadValue<Vector2>()
        };

        playerCamera.UpdateRotation(cameraInput);
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        playerCamera.UpdateFOV(deltaTime);

        // cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);

        // get character input and update it
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move     = input.Move.ReadValue<Vector2>(),
            Jump     = input.Jump.WasPressedThisFrame(),
            Dash     = input.Dash.WasPressedThisFrame(),
            Fire     = input.PrimaryFire.IsPressed(),
            Abil2    = input.SecondaryFire.WasPressedThisFrame(),
            Abil3    = input.Mobility.WasPressedThisFrame(),
            Abil4    = input.Miscellaneous.WasPressedThisFrame(),
            Reload   = input.ManualReload.WasPressedThisFrame()
        };

        // Debug.Log($"Player move vector: {characterInput.Move}");
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        // update ability tick
        abilitySet.UpdateAbilities(deltaTime);
    }
    void LateUpdate()
    {
        // playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        // var deltaTime = Time.deltaTime;
        // var cameraTarget = playerCharacter.GetCameraTarget();

        // cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
    }

    public PlayerCharacter GetCharacter() => playerCharacter;
    public PlayerCamera GetCamera() => playerCamera;
}
