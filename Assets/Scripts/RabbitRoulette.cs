using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RabbitRoulette : MonoBehaviour
{
    public GameObject rabbitPrefab;
    public Transform[] holePositions;
    public float moveSpeed = 5f;
    public float rotationSpeed = 50f;
    public float circleRadius = 5f;
    public float jumpDistance = 3f; //
    public float movementDuration = 10f; // Duration before selecting winner
    [SerializeField] private float totalDuration = 30f;

    private GameObject rabbit;
    private Animator animator;
    private float totalTimer;
    public int finalHoleIndex = -1;
    Vector3 targetPosition;
    private bool isGameActive = true, isFinalMove = false;
    private bool isIdle = false;
    private float idleTimer;
    private float idleInterval = 2f;
    private bool hasReachedFinalPosition = false;


    void Start()
    {
        rabbit = Instantiate(rabbitPrefab, transform.position, Quaternion.identity);
        animator = rabbit.GetComponent<Animator>();
        SetNewRandomPosition();
        isGameActive = true;
        isFinalMove = false;
        hasReachedFinalPosition = false;
        totalDuration = Random.Range(60f, 90f);
        finalHoleIndex = Random.Range(0, holePositions.Length);
        ChangeAnimation(0, 1, 0);
    }



    private void Update()
    {
        //if (hasReachedFinalPosition) return;

        if (!isGameActive && !isFinalMove)
        {
            StartFinalMove();
        }


        totalTimer += Time.deltaTime;
        if (totalTimer >= totalDuration && isGameActive)
        {
            Debug.Log("Last Move");
            isGameActive = false;
        }


        float distanceToTarget = Vector3.Distance(rabbit.transform.position, targetPosition);


        if (distanceToTarget < 0.1f)
        {
            if (isFinalMove)
            {
                isFinalMove = false;
                hasReachedFinalPosition = true;
                animator.enabled = false; // Stop all animations
            }
            else if (!isIdle)
            {
                EnterIdleState();
            }
            else
            {
                HandleIdleState();
                HandleJumpState(distanceToTarget);
            }
        }
        else if (!isIdle)
        {
            MoveTowardsTarget();
           
        }
    }


    private void EnterIdleState()
    {
        isIdle = true;
        idleTimer = 0f;
        ChangeAnimation(1, 0, 0);
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleInterval && !isFinalMove)
        {
            isIdle = false;
            ChangeAnimation(0, 1, 0);
            SetNewRandomPosition();
        }
    }

    private void HandleJumpState(float distanceToTarget)
    {

        bool shouldJump = distanceToTarget > jumpDistance;
        if (shouldJump)
        {
            Debug.Log("Jump");
            ChangeAnimation(0, 0, 1);
        }
    }



    private void StartFinalMove()
    {
        if (finalHoleIndex >= 0 && finalHoleIndex < holePositions.Length)
        {
            isFinalMove = true;
            targetPosition = holePositions[finalHoleIndex].position;
            isIdle = false;
            ChangeAnimation(0, 1, 0);
        }
    }

    private void SetNewRandomPosition()
    {
        Debug.Log("SetNewRandomPosition");
        Vector2 randomPoint = Random.insideUnitCircle;
        targetPosition = new Vector3(
            randomPoint.x * circleRadius,
            rabbit.transform.position.y,
            randomPoint.y * circleRadius
        );
    }

    private void MoveTowardsTarget()
    {
        Vector3 directionToTarget = (targetPosition - rabbit.transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        rabbit.transform.rotation = Quaternion.Slerp(
            rabbit.transform.rotation,
            Quaternion.Euler(0, targetAngle, 0),
            Random.Range(1f, rotationSpeed) * Time.deltaTime
        );

        rabbit.transform.position = Vector3.MoveTowards(
            rabbit.transform.position,
            targetPosition,
            Random.Range(1f,moveSpeed) * Time.deltaTime
        );


    }

    void ChangeAnimation(float idle, float walk, float run)
    {
        if (animator != null)
        {
            animator.SetFloat("Idle", idle);
            animator.SetFloat("Walk", walk);
            animator.SetFloat("Run", run);
        }
    }
}
