using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Enemy AI states
    private enum State { PATROL, ALERT, ATTACK };
    [SerializeField]private State _state;

    private bool _isAlive;
    private bool _isAttack;

    [SerializeField] NavMeshAgent _agent;
    private bool _isGoingForward;
    private Vector2 _nextWaypoint;
    private float _angleDifference;
    private float _angleToTarget;
    private float _agentWalkSpeed = 3.5f;
    private float _agentRunSpeed = 8f;
    [SerializeField] private Transform[] _patrolRoute;

    private Vector2 _lastKnownPlayerLocation;

    private Animator _animator;

    private GameDirector _gameDirector;
    private PlayerController _player;

    public bool IsAttack => _isAttack;

    // Enemy Death event
    public delegate void EnemyDeath(int position);
    public static event EnemyDeath OnEnemyDeath;

    private void OnEnable()
    {
        WeaponController.OnShotFired += GetOnAlert;
    }

    private void Start()
    {
        _isAlive = true;

        _gameDirector = FindObjectOfType<GameDirector>();
        _player = FindObjectOfType<PlayerController>();

        // Disable NavMeshAgent rotation
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        

        _animator = GetComponentInChildren<Animator>();

        _state = State.PATROL;

        _isGoingForward = false;

        Physics2D.queriesStartInColliders = false;
    }

    private void Update()
    {
        if (_isAlive)
        {
            var _hitInfo = Physics2D.Raycast(transform.position, _player.transform.position - transform.position, 7f);

            if (_hitInfo.collider != null
                    && _hitInfo.collider.GetComponent<PlayerController>()
                    && Vector3.Angle(transform.up, _hitInfo.transform.position - transform.position) < 45f)
            {
                Debug.DrawLine(transform.position, _hitInfo.point, Color.magenta);
                Debug.LogWarning("Busted!");

                _state = State.ATTACK;

                _lastKnownPlayerLocation = _hitInfo.point;  
            }
            else
            {
                Debug.DrawLine(transform.position, _player.transform.position, Color.yellow);
            }

            if (_state == State.PATROL)
            {
                PatrolLevel();
            }

            if (_state == State.ALERT)
            {
                // Alert and move to target
                _agent.SetDestination(_gameDirector.ShootNoizePosition);
                Debug.DrawLine(transform.position, _gameDirector.ShootNoizePosition, Color.red);
            }

            if (_state == State.ATTACK)
            {
                _agent.SetDestination(_hitInfo.point);
                Debug.DrawLine(transform.position, _hitInfo.point, Color.magenta);

                _isAttack = true;
            }
            else if (_hitInfo.collider != null && !_hitInfo.collider.GetComponent<PlayerController>() && _state == State.ATTACK)
            {
                _isAttack = false;
                _state = State.ALERT;

                _agent.SetDestination(_lastKnownPlayerLocation);
                StartCoroutine(WaitOnAlert());
            }

            // Rotrate character across the route. NavMeshAgent rotation workaround.
            if (_agent.hasPath)
            {
                if (_nextWaypoint != (Vector2)_agent.path.corners[1])
                {
                    // Rotate to Y-axis facing
                    StartCoroutine(RotateToWaypoint());
                    _nextWaypoint = _agent.path.corners[1];
                }
            }

            _animator.SetFloat("Speed", _agent.speed);
        }  
    }

    private void OnDrawGizmos()
    {
        #region Gizmos on Y-axis as "Forward" direction
        Vector3 viewAngle1 = GetDirectionFromAngle(transform.eulerAngles.z, -45);
        Vector3 viewAngle2 = GetDirectionFromAngle(transform.eulerAngles.z, 45);

        // Forward direction
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.up * 7f);

        // Left LOS border
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up + viewAngle1 * 6f);

        // Right LOS border
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up + viewAngle2 * 6f);
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

    private Vector3 GetDirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees -= eulerZ; // Rotation angle on Z-axis clockwise

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    /// <summary>
    /// Patrol state
    /// </summary>
    private void PatrolLevel()
    {
        _agent.speed = _agentWalkSpeed;

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

    /// <summary>
    /// Alert state
    /// </summary>
    private void GetOnAlert()
    {
        _state = State.ALERT;

        _agent.speed = _agentRunSpeed;
    }

    private void GetOnAttack()
    {
        // Attacks player
    }

    /// <summary>
    /// Rotation character to movement direction
    /// </summary>
    /// <returns></returns>
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

    IEnumerator WaitOnAlert()
    {
        yield return new WaitForSeconds(5f);
        _state = State.PATROL;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bullet>() != null && _gameDirector.Enemies[0] == this.gameObject)
        {
            if (OnEnemyDeath != null)
                OnEnemyDeath(0);
            gameObject.SetActive(false);
            _isAlive = false;
        }
    }

    private void OnDisable()
    {
        WeaponController.OnShotFired -= GetOnAlert;
    }
}
