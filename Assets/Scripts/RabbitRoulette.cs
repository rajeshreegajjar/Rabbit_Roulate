using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class RabbitRoulette : MonoBehaviour
{
    public GameObject rabbitPrefab;
    public Transform[] holePositions;
    public float moveSpeed = 5f;
    public float rotationSpeed = 50f;
    public float circleRadius = 5f;
    //public float jumpDistance = 3f; //
    //public float movementDuration = 10f; // Duration before selecting winner
    //public float totalDuration;
    public bool isGameActive = true, isFinalMove = false;

    private GameObject rabbit;
    public int finalHoleIndex = -1, myHoleIndex;
    private Vector3 targetPosition;

    private bool isIdle = false;
    private float idleTimer;
    private float idleInterval = 2f;


    private Animator animator;
    private float speedX = 0f;
    private float speed = 0f;

    // Animation parameter names
    private const string SPEED_X_PARAM = "SpeedX";
    private const string SPEED_PARAM = "Speed";

    // Movement settings
    [SerializeField] private float distanceThresholdForRun = 5f; // Distance at which rabbit starts running
    [SerializeField] private float stoppingDistance = 0.1f;     // How close rabbit needs to be to stop
    Vector3 IdlePos;

    private void Start()
    {
        if (rabbit == null)
            rabbit = Instantiate(rabbitPrefab, transform.position, Quaternion.identity);
        IdlePos = rabbit.transform.position;
        animator = rabbit.GetComponent<Animator>();
    }


    void OnEnable()
    {
        ResetRabitPosition();
    }

    public void ResetRabitPosition()
    {
        if (rabbit != null)
            rabbit.transform.position = IdlePos;
        isGameActive = false;
        isFinalMove = false;
        finalHoleIndex = -1;
        isIdle = false;
        idleTimer = 0f;
        speedX = 0;
        speed = 0;
        if (animator != null)
        {
            animator.SetFloat(SPEED_X_PARAM, speedX);
            animator.SetFloat(SPEED_PARAM, speed);
        }
        for (int i = 0; i < holePositions.Length; i++)
        {
            //holePositions[i].parent.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
            //holePositions[i].parent.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);
            ParticleSystem[] particles = holePositions[i].parent.GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < particles.Length; j++)
            {
                particles[j].Stop(true);
            }
            AudioSource[] audio = holePositions[i].parent.GetComponentsInChildren<AudioSource>();
            for (int k = 0; k < audio.Length; k++)
            {
                audio[k].Stop();
            }

        }
        SetNewRandomPosition();
    }


    private void Update()
    {

        if (!isGameActive && !isFinalMove)
        {
            return;
        }


        //if (isGameActive)
        //{
        //    totalTimer += Time.deltaTime;
        //    if (totalTimer >= totalDuration)
        //    {
        //        Debug.Log("Last Move");
        //        isGameActive = false;
        //        hasReachedFinalPosition = false;
        //        isFinalMove = false;
        //    }
        //}

        //Debug.Log("Start rabbit movement");

        float distanceToTarget = Vector3.Distance(rabbit.transform.position, targetPosition);


        speedX = 0;

        // Set target speed based on distance
        float targetSpeed = distanceToTarget > distanceThresholdForRun ? 2f : distanceToTarget > stoppingDistance ? 1f : 0f;

        // Smoothly interpolate speed
        speed =  Mathf.Lerp(speed, targetSpeed, moveSpeed * Time.deltaTime);


        animator.SetFloat(SPEED_X_PARAM, speedX);
        animator.SetFloat(SPEED_PARAM, speed);


        if (distanceToTarget < 0.1f)
        {
            if (isFinalMove)
            {
                isFinalMove = false;
                ParticleSystem[] particles = holePositions[finalHoleIndex].parent.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Play();
                }
                AudioSource[] audio = holePositions[finalHoleIndex].parent.GetComponentsInChildren<AudioSource>();
                for (int k = 0; k < audio.Length; k++)
                {
                    audio[k].Play();
                }
                animator.SetFloat(SPEED_X_PARAM, 0);
                animator.SetFloat(SPEED_PARAM, 0);
                Invoke("SetWinLossEffect", 2f);
                Invoke("ResetRabitPosition", 5f);
            }
            else if (!isIdle)
            {
                EnterIdleState();
            }
            else
            {
                HandleIdleState();
            }
        }
        else if (!isIdle)
        {
            MoveTowardsTarget();

        }
    }

    private void SetWinLossEffect()
    {
        Debug.Log("finalHoleIndex : " + finalHoleIndex);
        Debug.Log("myHoleIndex : " + myHoleIndex);
        Debug.Log("parent : " + holePositions[finalHoleIndex].parent.GetComponent<MeshRenderer>().material.GetColor("_BaseColor"));
        //if (finalHoleIndex == myHoleIndex)
        //{
        //    //holePositions[finalHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
        //    //holePositions[finalHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green);
        //}
        //else
        //{
        //    holePositions[finalHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
        //    holePositions[finalHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green);
        //    if (myHoleIndex > -1)
        //    {
        //        holePositions[myHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
        //        holePositions[myHoleIndex].parent.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
        //    }
          
        //}
    }

    private void EnterIdleState()
    {
        isIdle = true;
        idleTimer = 0f;
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleInterval && !isFinalMove)
        {
            isIdle = false;
            SetNewRandomPosition();
        }
    }

    [ContextMenu("Do Something")]
    public void startFinalMove()
    {
        StartFinalMove(finalHoleIndex, myHoleIndex);
    }


    public void StartFinalMove(int winningNumber, int myBetNumber)
    {
        finalHoleIndex = winningNumber;
        myHoleIndex = myBetNumber;
        if (finalHoleIndex >= 0 && finalHoleIndex < holePositions.Length)
        {
            isGameActive = false;
            isFinalMove = true;
            targetPosition = holePositions[finalHoleIndex].position;
            isIdle = false;
        }
    }

    private void SetNewRandomPosition()
    {
        // Generate a random point within a unit circle
        Vector2 randomPoint = Random.insideUnitCircle.normalized;

        // Scale the point to be within the desired radius range (1.5 to 5.0)
        float randomRadius = Random.Range(1.5f, circleRadius);
        randomPoint *= randomRadius;

        if (rabbit == null)
            return;

        targetPosition = new Vector3(
             randomPoint.x,
            rabbit.transform.position.y,
            randomPoint.y
        );
    }

    private void MoveTowardsTarget()
    {

        //Debug.Log("MoveTowardsTarget " + targetPosition);
        Vector3 directionToTarget = (targetPosition - rabbit.transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        rabbit.transform.rotation = Quaternion.Slerp(
            rabbit.transform.rotation,
            Quaternion.Euler(0, targetAngle, 0),
           rotationSpeed * Time.deltaTime
        );

        rabbit.transform.position = Vector3.MoveTowards(
            rabbit.transform.position,
            targetPosition,
           moveSpeed * Time.deltaTime
        );



    }

}
