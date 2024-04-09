using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController charCon;
    [SerializeField] private Transform head;
    [SerializeField] private ParticleSystem spray;

    public float movementSpeed = 5f;

    private Transform camTransform;
    private void OnEnable()
    {
        PlayerControl.instance.EnterFPV();
        camTransform = Camera.main.transform;
    }
    private void Update()
    {
        #region rotation
        transform.rotation = Quaternion.Euler(new Vector3(0f, camTransform.rotation.eulerAngles.y, 0f));
        head.localRotation = Quaternion.Euler(new Vector3(camTransform.rotation.eulerAngles.x, 0f, 0f));
        #endregion

        #region movement
        Vector2 inputDir = PlayerControl.instance.GetPlayerMovement();
        Vector3 moveDir = camTransform.forward * inputDir.y + camTransform.right * inputDir.x;
        moveDir.y = 0f;
        charCon.Move(moveDir * movementSpeed * Time.deltaTime);
        #endregion

        #region paint
        Debug.Log(spray.isPlaying);
        if (PlayerControl.instance.GetMouseDown())
        {
            if(!spray.isPlaying)
                spray.Play();
        }
        else if (spray.isPlaying)
        {
            spray.Stop();
        }
        #endregion
    }
}
