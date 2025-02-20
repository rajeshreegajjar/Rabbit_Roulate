using UnityEngine;

public class RabbitMovementController : MonoBehaviour
{
    private Animator animator;
    private float speedX = 0f;
    private float speed = 0f;

    // Animation parameter names
    private const string SPEED_X_PARAM = "SpeedX";
    private const string SPEED_PARAM = "Speed";

    // Movement settings
    [SerializeField] private float distanceThresholdForRun = 5f; // Distance at which rabbit starts running
    [SerializeField] private float stoppingDistance = 0.1f;     // How close rabbit needs to be to stop

    // Current destination
    private Vector3 destinationPosition;
    private bool hasDestination = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDestination(Vector3 newDestination)
    {
        destinationPosition = newDestination;
        hasDestination = true;
    }

    void Update()
    {
        if (!hasDestination)
        {
            // No destination - set to idle
            speed = 0;
            speedX = 0;
            animator.SetFloat(SPEED_X_PARAM, speedX);
            animator.SetFloat(SPEED_PARAM, speed);
            return;
        }

        // Calculate distance to destination
        float distanceToTarget = Vector3.Distance(transform.position, destinationPosition);

        // Stop if we're close enough
        if (distanceToTarget <= stoppingDistance)
        {
            speed = 0;
            speedX = 0;
            hasDestination = false;
        }
        else
        {
            // Determine direction (-1 for left, 1 for right)
            Vector3 direction = (destinationPosition - transform.position).normalized;
            speedX = direction.x > 0 ? 1 : -1;

            // Set speed based on distance
            if (distanceToTarget > distanceThresholdForRun)
            {
                speed = 2; // Run
            }
            else
            {
                speed = 1; // Walk
            }
        }

        // Update animator parameters
        animator.SetFloat(SPEED_X_PARAM, speedX);
        animator.SetFloat(SPEED_PARAM, speed);
    }

    // Optional: Visualize the run threshold in the editor
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, distanceThresholdForRun);
    //}
}