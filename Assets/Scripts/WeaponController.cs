﻿using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _weapon;
    [SerializeField] private ObjectPool _bulletPool;

    public delegate void ShotFired();
    public static event ShotFired OnShotFired;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && _weapon.parent != null)
        {
            Fire();

            OnShotFired?.Invoke();
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
