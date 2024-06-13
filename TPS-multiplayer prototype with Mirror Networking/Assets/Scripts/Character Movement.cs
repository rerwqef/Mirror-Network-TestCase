using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
using UnityEngine.UI;

public class CharacterMovement : NetworkBehaviour
{
    CharacterController characterController;
    public float walkSpeed = 4;
    public float runSpeed = 8;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    GameObject cam;
    CinemachineFreeLook freeLookCam;
    public GameObject LookAt;
    public bool isRunning;
    Animator anim;
    Interactor interactor;

    // Gravity variables
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private float verticalVelocity;
   
  //  private bool isGrounded;
    GameObject jumpBtn;
    GameObject runBtn;

    public void Start()
    {
        cam = GameObject.Find("Main Camera");
        freeLookCam = GameObject.FindAnyObjectByType<CinemachineFreeLook>();
        jumpBtn = GameObject.Find("Jump_Btn");
        runBtn = GameObject.Find("Run_Btn");

        if (isLocalPlayer)
        {
            anim = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            cam.GetComponent<Camera>().enabled = true;
            jumpBtn.GetComponent<Button>().enabled = true;
            runBtn.GetComponent<Button>().enabled = true;
            jumpBtn.GetComponent<Button>().onClick.AddListener(Jump);
            runBtn.GetComponent<Button>().onClick.AddListener(Run);

            freeLookCam.LookAt = LookAt.transform;
            freeLookCam.Follow = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

       

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small value to keep the character grounded
        }

        if (isRunning)
        {
            RunMovement();
        }
        else
        {
            NormalMovement();
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Apply vertical movement
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0);
        characterController.Move(verticalMove * Time.deltaTime);
    }

    void NormalMovement()
    {
        float horizontal = SimpleInput.GetAxis("Horizontal");
        float vertical = SimpleInput.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDir * walkSpeed * Time.deltaTime);

            float moveSpeed = moveDirection.magnitude;
            anim.SetFloat("moveSpeed", moveSpeed);
        }
        else
        {
            anim.SetFloat("moveSpeed", 0);
        }
    }

    void RunMovement()
    {
        float targetAngle = cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        characterController.Move(moveDir * runSpeed * Time.deltaTime);
        anim.SetFloat("moveSpeed", 3); // Setting a fixed animation speed for running
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.Play("Jump");
        }
    }

    public void Run()
    {
        isRunning = !isRunning;
    }
}