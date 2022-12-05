using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _speed;
    private float _horizontal;
    private float _vertical;

    private Vector2 _direction;

    private Rigidbody2D _playerRb;

    [SerializeField] Transform _weaponHoldPoint;
    [SerializeField] private bool _weaponIsArmed;

    [SerializeField] Rigidbody2D _weapon;

    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorOverrideController[] _animatorOverride;
    private int _currentAnimatorOverrideIndex;

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _weaponIsArmed = true;

        _currentAnimatorOverrideIndex= 0;
        _animator.runtimeAnimatorController = _animatorOverride[_currentAnimatorOverrideIndex];
    }

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _direction = new Vector2(_horizontal, _vertical);
        
        AimToCursor();

        if (Input.GetKeyDown(KeyCode.E))
            ThrowWeapon();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon();

        if (_direction != Vector2.zero)
            _animator.SetFloat("Speed", 4f);
        else
            _animator.SetFloat("Speed", 0f);
    }

    private void SwitchWeapon()
    {
        _currentAnimatorOverrideIndex++;

        if(_currentAnimatorOverrideIndex >= _animatorOverride.Length)
            _currentAnimatorOverrideIndex = 0;

        _animator.runtimeAnimatorController = _animatorOverride[_currentAnimatorOverrideIndex];
    }

    private void FixedUpdate()
    {
        _playerRb.MovePosition(_playerRb.position + _direction * _speed * Time.fixedDeltaTime);
    }

    private void AimToCursor()
    {
        Vector2 _mouseScreenPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 _direction = _mouseScreenPosition - (Vector2)transform.position;
        _direction.Normalize();

        transform.up = _direction;
        if (Input.GetKeyDown(KeyCode.E) && _weaponIsArmed == true)
            ThrowWeapon();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("TOUCH");
        if (other.CompareTag("Weapon") && _weaponIsArmed == false && Input.GetKeyDown(KeyCode.E))
            PickWeapon();
    }

    private void ThrowWeapon()
    {
        Debug.Log("DROP");
        _weaponIsArmed = false;
        _weapon.transform.parent = null;
        _weapon.simulated = true;
        //float _force = 20f;
        //_weapon.AddForce(transform.up * _force, ForceMode2D.Impulse);  
    }

    private void PickWeapon()
    {
        Debug.Log("PICK");
        _weapon.simulated = false;

        _weapon.transform.parent = _weaponHoldPoint;

        _weapon.transform.position = _weaponHoldPoint.position;
        _weapon.transform.rotation = _weaponHoldPoint.rotation;

        _weaponIsArmed = true;
    }
}
