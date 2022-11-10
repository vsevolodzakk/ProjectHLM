using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _weaponIsArmed = true;
        //gameObject.GetComponent<Collider2D>().enabled = false;
    }

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _direction = new Vector2(_horizontal, _vertical);
        
        AimToCursor();
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

    private void ThrowWeapon()
    {
        Debug.Log("DROP");
        _weaponIsArmed = false;
        _weapon.transform.parent = null;
        _weapon.simulated = true;
        //float _force = 20f;
        //_weapon.AddForce(transform.up * _force, ForceMode2D.Impulse);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("TOUCH");
        if (other.CompareTag("Weapon") && _weaponIsArmed == false && Input.GetKeyDown(KeyCode.E))
            PickWeapon();
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
