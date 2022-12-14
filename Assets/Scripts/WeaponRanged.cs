using UnityEngine;

public class WeaponRanged : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private int _ammoCount;

    private int _ammoInBox = 10;

    private bool _isRanged;

    //public delegate void ShotFired();
    //public static event ShotFired OnShotFired;

    private void OnEnable()
    {
        Reload();
    }

    private void Shoot()
    {
        _ammoCount--;
    }

    private void Reload()
    {
        _ammoCount = _ammoInBox;
    }

    private void Attack()
    {

    }
}
