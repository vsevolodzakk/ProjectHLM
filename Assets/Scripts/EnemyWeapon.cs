using UnityEngine;

public class EnemyWeapon : MonoBehaviour

{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;

    private EnemyController _enemyController;

    private void Start()
    {
        _enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (_enemyController.IsAttack)
        {
            Fire();
        }
    }

    private void Fire()
    {
        var _shot = _bulletPool.Get();
        _shot.transform.rotation = _firePoint.rotation;
        _shot.transform.position = _firePoint.position;

        _shot.gameObject.SetActive(true);
    }
}
