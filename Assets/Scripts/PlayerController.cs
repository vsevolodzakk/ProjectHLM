using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private float _speed;
    private float _horizontal;
    private float _vertical;

    private Vector2 _direction;

    private Rigidbody2D _playerRb;

    public Vector2 Direction { get { return _direction; } }

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
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
    }
}
