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

    private Vector3 _direction;

    private CharacterController _player;

    [SerializeField] Rigidbody2D _weapon;

    void Start()
    {
        _player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _direction = new Vector3(_horizontal, _vertical, 0f);

        _player.Move(_direction * _speed * Time.deltaTime);
        AimToCursor();
    }

    private void AimToCursor()
    {
        Vector2 _mouseScreenPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 _direction = _mouseScreenPosition - (Vector2)transform.position;
        _direction.Normalize();

        transform.up = _direction;
        if (Input.GetKeyDown(KeyCode.E))
            ThrowWeapon();
    }

    private void ThrowWeapon()
    {
       float _force = 10f;
       _weapon.AddForce(transform.up * _force, ForceMode2D.Impulse);
        _weapon.transform.parent = null;
    }
}
