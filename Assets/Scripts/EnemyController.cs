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
    private Vector2 _nextWaypoint;
    private float _angleDifference;
    private float _angleToTarget;
    [SerializeField] private Transform[] _patrolRoute;


    // Enemy Death event
    public delegate void EnemyDeath(int position);
    public static event EnemyDeath OnEnemyDeath;

    private GameDirector _gameDirector;
    private PlayerController _player;

    private void Start()
    {
        _gameDirector = FindObjectOfType<GameDirector>();
        _player = FindObjectOfType<PlayerController>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _state = State.PATROL;

        _isGoingForward = false;
    }

    private void Update()
    {

        _ray = new Ray2D(transform.position, _player.transform.position);

        if (_state == State.PATROL)
        {
            PatrolLevel();

            if (Physics2D.Raycast(transform.position, _player.transform.position, 5f, 0))
                Debug.LogWarning("BUSTED!");
        }

        if(_state == State.ALERT)
        {
            // Alert and move to target
            _agent.SetDestination(_player.transform.position);
        }

        

        if (_agent.hasPath)
        {
            if (_nextWaypoint != (Vector2)_agent.path.corners[1])
            {
                // Rotate to Y-axis facing
                StartCoroutine(RotateToWaypoint());
                _nextWaypoint = _agent.path.corners[1];
            }     
        }

        //Debug.DrawRay(transform.position, transform.up * 7f, Color.red);
        //transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);

        // View cone for develop purpose
        //Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.y, -45);
        //Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.y, 45);

        //Debug.DrawRay(transform.position, transform.up + viewAngle1 * 6f, Color.green);
        //Debug.DrawRay(transform.position, transform.up + viewAngle2 * 6f, Color.blue);
    }

    private void OnDrawGizmos()
    {
        #region Gizmos on Y-axis as "Forward" direction
        Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.z, -45);
        Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.z, 45);

        // Forward direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * 7f);

        // Left LOS border
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up + viewAngle1 * 6f);

        // Right LOS border
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up + viewAngle2 * 6f);
        #endregion

        #region Gizmos on Z-axis as "Forward" direction
        //Vector3 viewAngle1 = DirectionFromAngleZ(transform.eulerAngles.y, -45);
        //Vector3 viewAngle2 = DirectionFromAngleZ(transform.eulerAngles.y, 45);

        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.forward * 7f);


        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, transform.forward + viewAngle1 * 6f);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, transform.forward + viewAngle2 * 6f);
        #endregion

        if (_agent.hasPath)
        {
            for (int i = 0; i + 1 < _agent.path.corners.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(_agent.path.corners[i + 1], 0.03f);
                Gizmos.color = Color.black;
                Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
            }
        }
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
        if (_agent.remainingDistance < .5f && _isGoingForward)
        {
            _agent.SetDestination(_patrolRoute[1].position);
            _isGoingForward = false;
        }
        else if (_agent.remainingDistance < .5f && !_isGoingForward)
        {
            _isGoingForward = true;
            _agent.destination = _patrolRoute[0].position;
        }
    }

    IEnumerator RotateToWaypoint()
    {
        float rotateSpeed = 80f;

        Vector2 targetDirection = _agent.path.corners[1] - transform.position;

        _angleDifference = Vector2.SignedAngle(transform.up, targetDirection);
        _angleToTarget = transform.localEulerAngles.z + _angleDifference;

        if (_angleToTarget >= 360)
            _angleToTarget -= 360;
        else if (_angleToTarget < 0)
            _angleToTarget += 360;

        while(transform.localEulerAngles.z < _angleToTarget - 0.1f 
                || transform.localEulerAngles.z > _angleToTarget + 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, _angleToTarget), rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bullet>() != null && _gameDirector.Enemies[0] == this.gameObject)
        {
            if (OnEnemyDeath != null)
                OnEnemyDeath(0);
            gameObject.SetActive(false);
        }
    }
}
