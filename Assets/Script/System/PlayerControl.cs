using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Singleton<PlayerControl>
{
    public PInput pInput;
    public override void Awake()
    {
        base.Awake();
        pInput = new PInput();
    }
    private void OnEnable()
    {
        pInput.Enable();
    }
    private void OnDisable()
    {
        pInput.Enable();
    }
    public Vector2 GetPlayerMovement() => pInput.Player.Move.ReadValue<Vector2>();

    public Vector2 GetMouseMovement() => pInput.Player.MouseMove.ReadValue<Vector2>();
    public bool GetMouseDown() => pInput.Player.MouseHold.IsPressed();
    public void EnterFPV()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pInput.UI.Disable();
        pInput.Player.Enable();
    }
    public void ExitFPV()
    {
        Cursor.lockState = CursorLockMode.None;
        pInput.Player.Disable();
        pInput.UI.Enable();
    }
}
