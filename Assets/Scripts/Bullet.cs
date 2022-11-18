using UnityEngine;

public class Bullet : MonoBehaviour, IGameObjectPooled
{ 
    [SerializeField] private float _speed;
    [SerializeField] private float _maxLifetime;
    private Rigidbody2D _rb2d;
    
    private float _lifetime; // Bullet lifetime
    private ObjectPool _bulletPool; // Pool of bullets

    public ObjectPool Pool
    {
        get { return _bulletPool; }
        set
        {
            if (_bulletPool == null)
                _bulletPool = value;
            else
                throw new System.Exception("Bad pool use!");
        }
    }

    private void OnEnable()
    {
        _lifetime = 0f;
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //transform.Translate(Vector3.forward * _speed * Time.deltaTime); // Bullet fly
        
        _lifetime += Time.deltaTime;

        // Return Bullet to Pool after lifetime end
        if (_lifetime > _maxLifetime)
            Pool.ReturnToPool(this.gameObject);
    }

    private void FixedUpdate()
    {
        //_rb2d.MovePosition(transform.up * _speed * Time.fixedDeltaTime);
        _rb2d.AddForce(transform.up * _speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            // Return Bullet to Pool after hit
            Pool.ReturnToPool(this.gameObject);
        }      
    }
}
