using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private float xSensitivity = 10f;
    [SerializeField] private float ySensitivity = 10f;
    [SerializeField] private float clampAngle = 80f;
    private Vector3 startingRotation;
    protected override void Awake()
    {
        base.Awake();

        if (startingRotation == null) startingRotation = transform.localRotation.eulerAngles;
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vCam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vCam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                Vector2 camDir = PlayerControl.instance.GetMouseMovement();
                startingRotation.x += camDir.x * xSensitivity * Time.deltaTime;
                startingRotation.y += camDir.y * ySensitivity * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}
