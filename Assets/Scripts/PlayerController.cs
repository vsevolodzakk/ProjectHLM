using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _speed;
    private float _horizontal;
    private float _vertical;

    private Vector2 _direction;

    private Rigidbody2D _rb2d;

    //[SerializeField] Rigidbody2D _weapon;

    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _direction = new Vector2(_horizontal, _vertical);
        
        AimToCursor();
    }

    private void FixedUpdate()
    {
        _rb2d.MovePosition(_rb2d.position + _direction * _speed * Time.fixedDeltaTime);
    }

    private void AimToCursor()
    {
        Vector2 _mouseScreenPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 _direction = _mouseScreenPosition - (Vector2)transform.position;
        _direction.Normalize();

        transform.up = _direction;
        //if (Input.GetKeyDown(KeyCode.E))
        //    ThrowWeapon();
    }

    //private void ThrowWeapon()
    //{
    //   float _force = 10f;
    //   _weapon.AddForce(transform.up * _force, ForceMode2D.Impulse);
    //    _weapon.transform.parent = null;
    //}
}
