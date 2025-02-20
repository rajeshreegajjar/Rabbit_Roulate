using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticRabbitController : MonoBehaviour
{
    public CharacterController characterController;
    Vector3 moveDirection;
    Animator animator;
    public float AngleSpeed;
    public float MoveSpeed;
    public float MovementMultiplier = 2;
    private float CurrentRunMultiplier;
    public bool Sleep;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    float HorizontalAnim;
    void Update()
    {
        transform.Rotate(0.0f, Input.GetAxis("Horizontal") * AngleSpeed, 0.0f);
        moveDirection = transform.forward * Input.GetAxis("Vertical")* CurrentRunMultiplier;
        moveDirection *= MoveSpeed;
        animator.SetFloat("Speed", Input.GetAxis("Vertical")* CurrentRunMultiplier);

        HorizontalAnim = Mathf.Lerp(HorizontalAnim, Input.GetAxis("Horizontal"), 3 * Time.deltaTime);
        animator.SetFloat("SpeedX", HorizontalAnim);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            CurrentRunMultiplier = Mathf.Lerp(CurrentRunMultiplier, MovementMultiplier, Time.deltaTime * 2);
        }
        else
        {
            CurrentRunMultiplier = Mathf.Lerp(CurrentRunMultiplier, 1, Time.deltaTime * 2);
        }

        moveDirection.y -= 200f*Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Sleep = !Sleep;
            animator.SetBool("Sleep", Sleep);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("Attack1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("Attack2");
        }
    }
}
