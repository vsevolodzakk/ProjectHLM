using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;
    [SerializeField] private int _ammoCount = 0;

    // These needed to Instantiate "dropped" weapon Item
    [SerializeField] private GameObject _weaponItemTemplate;
    [SerializeField] private WeaponItemData _weaponItemData;

    [SerializeField] private bool _weaponIsArmed;

    public List<WeaponItem> weaponItems= new List<WeaponItem>();

    public int AmmoCount { get { return _ammoCount; } }

    public delegate void ShotFired();
    public static event ShotFired OnShotFired;

    public delegate void WeaponPickedUp(WeaponItemData weaponData);
    public static event WeaponPickedUp OnWeaponPickedUp;

    public delegate void WeaponDroped();
    public static event WeaponDroped OnWeaponDroped;

    private void OnEnable()
    {

    }

    private void Start()
    {
        _weaponIsArmed = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") & _weaponIsArmed & _ammoCount > 0)
        {
            Fire();

            OnShotFired?.Invoke();
        }

        if(!_weaponIsArmed && weaponItems.Count > 0 && Input.GetKeyDown(KeyCode.E))
        {
            PickWeapon(weaponItems[0].weaponData);
            OnWeaponPickedUp?.Invoke(weaponItems[0].weaponData);
            
            Destroy(weaponItems[0].gameObject);
        }
        else if(_weaponIsArmed && Input.GetKeyDown(KeyCode.E))
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

        --_ammoCount;
    }

    private void PickWeapon(WeaponItemData weaponData)
    {
        _weaponIsArmed = true;
        _weaponItemData= weaponData;

        _ammoCount = weaponData.StockAmmo;
    }

    private void DropWeapon()
    {
        var item = Instantiate(_weaponItemTemplate, _firePoint.position, _firePoint.rotation);
        item.GetComponent<SpriteRenderer>().sprite = _weaponItemData.Image;
        item.name = _weaponItemData.name;

        var itemData = item.GetComponent<WeaponItem>();
        itemData.weaponData = _weaponItemData;
        itemData.isDroped = true;
        itemData.ammo = _ammoCount;


        OnWeaponDroped?.Invoke();

        _weaponIsArmed = false;
        _weaponItemData = null; // here needed to change ItemData to Fist .. or not?
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        weaponItems.Add(collision.GetComponent<WeaponItem>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        weaponItems.Remove(collision.GetComponent<WeaponItem>());
    }
}
