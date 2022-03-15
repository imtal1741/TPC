using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCMovement : MonoBehaviour
{
    CharacterController characterController;
    Camera cam;
    Animator anim;

    public float RotSpeed;
    public float allowRot;
    public bool isRootMotion;
    public float speedNRM;

    float InputX;
    float InputZ;
    float InputXSmooth;
    float InputZSmooth;
    Vector3 MoveDir;
    float speed;
    float verticalVel;
    bool isCrouch;
    float crouchMultiplier;
    float runMultiplier;
    Vector3 camForward;

    public static class States
    {
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";
        public const string Crouch = "Crouch";
        public const string InputX = "InputX";
        public const string InputZ = "InputZ";
        public const string MoveSpeed = "MoveSpeed";
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
        anim = GetComponent<Animator>();

        crouchMultiplier = 1;
        runMultiplier = 1;
    }

    void Update()
    {
        InputMagnitude();

        camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        if (!isRootMotion)
            characterController.Move(transform.forward * Time.deltaTime * speed * speedNRM);
    }

    void InputMagnitude()
    {
        InputX = Input.GetAxis(States.Horizontal) * crouchMultiplier * runMultiplier;
        InputZ = Input.GetAxis(States.Vertical) * crouchMultiplier * runMultiplier;
        InputXSmooth = Mathf.Lerp(InputXSmooth, InputX, Time.deltaTime * 10);
        InputZSmooth = Mathf.Lerp(InputZSmooth, InputZ, Time.deltaTime * 10);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            crouchMultiplier = 0.5f;
            isCrouch = true;
            anim.SetBool(States.Crouch, true);
        }
        else
        {
            crouchMultiplier = 1;
            isCrouch = false;
            anim.SetBool(States.Crouch, false);
        }

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouch)
            runMultiplier = 2;
        else
            runMultiplier = 1;

        anim.SetFloat(States.InputX, InputXSmooth, 0, Time.deltaTime * 2);
        anim.SetFloat(States.InputZ, InputZSmooth, 0, Time.deltaTime * 2);

        speed = Mathf.Clamp(new Vector2(InputX, InputZ).sqrMagnitude, 0, 1);

        if (speed > allowRot)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward), RotSpeed);
        }
        else if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward), 0.2f);
        }

        anim.SetFloat(States.MoveSpeed, speed, 0, Time.deltaTime);
    }

}
