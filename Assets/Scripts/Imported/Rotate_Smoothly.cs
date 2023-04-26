using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Rotate_Smoothly : MonoBehaviour
{
    public bool showAhead;
    public bool showPath;

    private NavMeshAgent agent;
    private Vector2 nextWaypoint;
    private float angleDifference;
    private float targetAngle;
    private float rotateSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        rotateSpeed = 80f;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                agent.SetDestination(hit.point);

                if (agent.path.corners.Length > 1)
                {
                    StartCoroutine("RotateToWaypoints");
                    nextWaypoint = agent.path.corners[1];
                }
            }
        }

        if (agent.hasPath)
        {
            if (nextWaypoint != (Vector2)agent.path.corners[1])
            {
                StartCoroutine("RotateToWaypoints");
                nextWaypoint = agent.path.corners[1];
            }
        }
    }

    IEnumerator RotateToWaypoints()
    {
        Vector2 targetVector = agent.path.corners[1] - transform.position;
        angleDifference = Vector2.SignedAngle(transform.up, targetVector);
        targetAngle = transform.localEulerAngles.z + angleDifference;

        if (targetAngle >= 360) { targetAngle -= 360; }
        else if (targetAngle < 0) { targetAngle += 360; }

        while (transform.localEulerAngles.z < targetAngle - 0.1f || transform.localEulerAngles.z > targetAngle + 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (showPath && agent.hasPath)
            {
                for (int i = 0; i + 1 < agent.path.corners.Length; i++)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(agent.path.corners[i + 1], 0.03f);
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
                }
            }

            if (showAhead)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.up * 0.5f);
            }
        }
    }
}
