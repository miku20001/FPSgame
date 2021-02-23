using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMovement : MonoBehaviour
{

    public float gravity;      //重力
    public float JumpHeight;   //角色能跳多高

    private Transform charcaterTransform;
    private Rigidbody characterRigbody;
    public float Speed;
    private bool isGrounded;        //是否触地
    // Start is called before the first frame update
    private void Start()
    {
        charcaterTransform = transform;
        characterRigbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            var tmp_Morizontal = Input.GetAxis("Horizontal");
            var tmp_Vortical = Input.GetAxis("Vertical");

            var tmp_CurrentDirection = new Vector3(tmp_Morizontal, 0, tmp_Vortical);
            tmp_CurrentDirection = charcaterTransform.TransformDirection(tmp_CurrentDirection);

            tmp_CurrentDirection *= Speed;

            var tmp_CurrentVelocity = characterRigbody.velocity;
            var tmp_VeloctityChange = tmp_CurrentDirection - tmp_CurrentVelocity;
            tmp_VeloctityChange.y = 0;

            Debug.Log(tmp_CurrentVelocity);

            characterRigbody.AddForce(tmp_VeloctityChange, ForceMode.VelocityChange);

            if (Input.GetButtonDown("Jump"))
            {
                characterRigbody.velocity = new Vector3(tmp_CurrentVelocity.x, CalculateJumpHeightSpeed(), tmp_CurrentVelocity.z);
            }
        }


        characterRigbody.AddForce(new Vector3(0, -gravity*characterRigbody.mass, 0));

    }

    private float CalculateJumpHeightSpeed()
    {
        return Mathf.Sqrt(2 * gravity * JumpHeight);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

}
