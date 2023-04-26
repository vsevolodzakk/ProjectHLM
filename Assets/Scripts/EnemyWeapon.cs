using UnityEngine;

public class EnemyWeapon : MonoBehaviour

{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;

    [SerializeField] private GameObject _weaponItemPrefab;
    [SerializeField] private WeaponItemData _weaponItemData;

    private float _shotCooldown = 0.7f;
    private float _cooldown;

    private EnemyController _enemyController;

    private void Start()
    {
        _enemyController = GetComponent<EnemyController>();
        _cooldown = _shotCooldown;
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (_enemyController.IsAttack & _cooldown <= 0)
        {
            Fire();
            _cooldown = _shotCooldown;
        }

        if (!_enemyController.IsAlive)
        {
            DropWeapon();
        }
    }

    private void Fire()
    {
        var _shot = _bulletPool.Get();
        _shot.transform.rotation = _firePoint.rotation;
        _shot.transform.position = _firePoint.position;

        _shot.gameObject.SetActive(true);
    }

    private void DropWeapon()
    {
        var item = Instantiate(_weaponItemPrefab, _firePoint.position, _firePoint.rotation);
        item.GetComponent<SpriteRenderer>().sprite = _weaponItemData.Image;
        item.name = _weaponItemData.name;

        var itemData = item.GetComponent<WeaponItem>();
        itemData.weaponData = _weaponItemData;
        itemData.isDroped = true;
        itemData.ammo = _weaponItemData.StockAmmo;
    }
}
