using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCharacterControllerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField]private Animator characterAnimator;
    private Transform characterTransform;
    private Vector3 movementDirection;
    private float velocity;


    private bool isCrouched;          //是否蹲下
    private float originHeight;        //原来的高度
    public float SprintingSpeed;          //奔跑速度
    public float WalkSpeed;            //行走速度
    public float SprintingSpeedWhenCrouched;          //蹲下奔跑速度
    public float WalkSpeedWhenCrouched;            //蹲下行走速度
    public float Gravity = 9.8f;
    public float JumpHeight;
    public float CrouchHeight = 1f;         //蹲下高度
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //characterAnimator = GetComponentInChildren<Animator>();
        originHeight = characterController.height;
        characterTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float tmp_CurrentSpeed = WalkSpeed;
        if (characterController.isGrounded)   //必须调用move移动才会进行isGrounded的状态更改
        {
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");

            //Debug.Log(new Vector3(tmp_Horizontal, 0, tmp_Vertical));
            //Debug.LogFormat("movementDirection={0}", new Vector3(tmp_Horizontal, 0, tmp_Vertical));
            movementDirection =
                characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));
            //movementDirection = new Vector3(tmp_Horizontal, 0, tmp_Vertical);

            

            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = JumpHeight;
            }

            if (Input.GetKeyDown(KeyCode.C)){
                var tmp_CurentHeight = isCrouched ? originHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmp_CurentHeight));
                isCrouched = !isCrouched;
            }



            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (isCrouched)
                {
                    tmp_CurrentSpeed = SprintingSpeedWhenCrouched;
                }
                else
                {
                    tmp_CurrentSpeed = SprintingSpeed;
                }
            }
            else
            {
                if (isCrouched)
                {
                    tmp_CurrentSpeed = WalkSpeedWhenCrouched;
                }
                else
                {
                    tmp_CurrentSpeed = WalkSpeed;
                }
            }

            velocity = characterController.velocity.magnitude;

            if (characterAnimator != null)
            {
                characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
            }

        }
        
        movementDirection.y -= Gravity*Time.deltaTime;
        //Debug.Log(movementDirection);
        characterController.Move(tmp_CurrentSpeed * movementDirection.normalized * Time.deltaTime);  //normalized 归一化限制斜向速度变快
        //Debug.Log(characterController.velocity.magnitude);
        
    }

    private IEnumerator DoCrouch(float target)
    {
        float tmp_CurrentHeight = 0;
        while (Mathf.Abs(characterController.height - target) > 0.1f)
        {
            yield return null;
            characterController.height =
                Mathf.SmoothDamp(characterController.height, target, ref tmp_CurrentHeight, Time.deltaTime * 5);
        }
    }

    internal void SetupAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}
