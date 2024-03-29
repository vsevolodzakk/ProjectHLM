﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private float _agentWalkSpeed = 3.5f;
    private float _agentRunSpeed = 8f;
    private float _rotateSpeed = 80f;
    private float _alertWaitTime = 15f;

    private float _angleDifference;
    private float _angleToTarget;
    [SerializeField] private bool _isAlive;
    private bool _isAttack;
    private bool _isGoingForward;
    private Vector2 _nextWaypoint;
    private RaycastHit2D _hitInfo;
    private Animator _animator;
    private GameDirector _gameDirector;
    private PlayerController _player;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] private Transform[] _patrolRoute;

    // Enemy Death event
    public delegate void EnemyDeath(int position);
    public static event EnemyDeath OnEnemyDeath;

    // Enemy AI states
    private enum State { PATROL, ALERT, ATTACK };
    [SerializeField]private State _currentState;

    public bool IsAttack => _isAttack;
    public bool IsAlive => _isAlive;

    private void OnEnable()
    {
        WeaponController.OnShotFired += GetOnAlert; // Set all Enemies on alert
    }

    private void Start()
    {
        _isAlive = true;

        _gameDirector = FindObjectOfType<GameDirector>();
        _player = FindObjectOfType<PlayerController>();

        _animator = GetComponentInChildren<Animator>();

        // Disable NavMeshAgent rotation
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _currentState = State.PATROL;

        _isGoingForward = false;

        Physics2D.queriesStartInColliders = false;
    }

    private void Update()
    {
        if (_isAlive)
        {
            _hitInfo = Physics2D.Raycast(transform.position, _player.transform.position - transform.position, 7f);

            // If Player appeared in view
            CheckFov();

            // Patrol state
            if (_currentState == State.PATROL)
                PatrolLevel();

            // Attack state
            if (_currentState == State.ATTACK)
            { 
                GetOnAttack();
            }

            // Alert state
            if(_currentState == State.ALERT)
            {
                _agent.SetDestination(_gameDirector.ShootNoizePosition);

                Debug.DrawLine(transform.position, _gameDirector.ShootNoizePosition, Color.red);

                if (!_agent.hasPath)
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

            // Character animator parameter
            _animator.SetFloat("Speed", _agent.speed);
        }  
    }

    /// <summary>
    /// Draw dev UI in editor
    /// </summary>
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

    /// <summary>
    /// Calculate angle for dev UI view cone
    /// </summary>
    /// <param name="eulerZ"></param>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    private Vector3 GetDirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees -= eulerZ; // Rotation angle on Z-axis clockwise

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    /// <summary>
    /// Check if Player in Enemy FOV
    /// </summary>
    private void CheckFov()
    {
        if (_hitInfo.collider != null
                    && _hitInfo.collider.GetComponent<PlayerController>()
                    && Vector3.Angle(transform.up, _hitInfo.transform.position - transform.position) < 45f)
        {
            Debug.DrawLine(transform.position, _hitInfo.point, Color.magenta);
            Debug.LogWarning("Busted!");

            _currentState = State.ATTACK;
        }
        else
        {
            Debug.DrawLine(transform.position, _player.transform.position, Color.yellow);
        }
    }

    /// <summary>
    /// Patrol state method
    /// </summary>
    private void PatrolLevel()
    {
        _agent.speed = _agentWalkSpeed;

        if (_agent.remainingDistance < 1f && _isGoingForward)
        {
            _agent.SetDestination(_patrolRoute[1].position);
            _isGoingForward = false;
        }
        else if (_agent.remainingDistance < 1f && !_isGoingForward)
        {
            _isGoingForward = true;
            _agent.destination = _patrolRoute[0].position;
        }
    }

    /// <summary>
    /// Alert state method
    /// </summary>
    private void GetOnAlert()
    {
        StopAllCoroutines();
        
        _currentState = State.ALERT;
        
        _agent.speed = _agentRunSpeed;
        //_agent.SetDestination(_gameDirector.ShootNoizePosition); // Why ShootNoizePosition sets with previous value?
    }

    /// <summary>
    /// Attack state method
    /// </summary>
    private void GetOnAttack()
    {       
        _agent.SetDestination(_hitInfo.point);
        _agent.speed = _agentRunSpeed;
        
        _isAttack = true;
    }

    /// <summary>
    /// Rotation character to movement direction
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateToWaypoint()
    {
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, _angleToTarget), _rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Wait until Alert fades away
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitOnAlert()
    {
        yield return new WaitForSeconds(_alertWaitTime);
        _currentState = State.PATROL;

        _agent.speed = _agentWalkSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bullet>() != null && _gameDirector.Enemies[0] == this.gameObject)
        {
            _isAlive = false;
            if (OnEnemyDeath != null)
                OnEnemyDeath(0);
            gameObject.SetActive(false);   
        }
    }

    private void OnDisable()
    {
        WeaponController.OnShotFired -= GetOnAlert;
    }
}
