using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
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
