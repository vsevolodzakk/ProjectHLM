using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Enemy AI states
    private enum State { PATROL, ALERT, ATTACK };

    private State _state;

    private CircleCollider2D _viewCone;
    private Ray2D _ray;
    private RaycastHit2D _hit;

    [SerializeField] NavMeshAgent _agent;
    private bool _isGoingForward;
    [SerializeField] private Transform[] _patrolRoute;


    // Enemy Death event
    public delegate void EnemyDeath(int position);
    public static event EnemyDeath OnEnemyDeath;

    private GameDirector gameDirector;

    private void Start()
    {
        gameDirector = FindObjectOfType<GameDirector>();

        //_agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _state = State.PATROL;

        _isGoingForward = false;
    }

    private void Update()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.C) && gameDirector.Enemies.Contains(this.gameObject))
        {
            //if (OnEnemyDeath != null)
            //    OnEnemyDeath(0);
            //gameObject.SetActive(false);
        }

        if(_state == State.PATROL)
        {
            if (_agent.remainingDistance < .5f && _isGoingForward)
            {
                _agent.SetDestination(_patrolRoute[1].position);
                _isGoingForward=false;
            }
            else if (_agent.remainingDistance < .5f && !_isGoingForward)
            {
                _isGoingForward = true;
                _agent.destination = _patrolRoute[0].position;
            }
        }

        _ray = new Ray2D(transform.position, transform.up);
        //Debug.DrawRay(transform.position, transform.up * 7f, Color.red);


        // View cone for develop purpose
        //Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.y, -45);
        //Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.y, 45);

        //Debug.DrawRay(transform.position, transform.up + viewAngle1 * 6f, Color.green);
        //Debug.DrawRay(transform.position, transform.up + viewAngle2 * 6f, Color.blue);
    }

    private void OnDrawGizmos()
    {
        #region Gizmos on Y-axis as "Forward" direction
        //Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.z, -45);
        //Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.z, 45);

        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.up * 7f);


        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, transform.up + viewAngle1 * 6f);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, transform.up + viewAngle2 * 6f);
        #endregion

        Vector3 viewAngle1 = DirectionFromAngleZ(transform.eulerAngles.y, -45);
        Vector3 viewAngle2 = DirectionFromAngleZ(transform.eulerAngles.y, 45);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 7f);


        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward + viewAngle1 * 6f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward + viewAngle2 * 6f);
    }

    private Vector3 DirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees -= eulerZ; // Rotation angle on Z-axis clockwise

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    private Vector3 DirectionFromAngleZ(float eulerY, float angle)
    {
        angle += eulerY;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private void PatrolLevel()
    {
        
         // Refactor method here  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bullet>() != null && gameDirector.Enemies[0] == this.gameObject)
        {
            if (OnEnemyDeath != null)
                OnEnemyDeath(0);
            gameObject.SetActive(false);
        }
    }
}
