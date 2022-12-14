using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;
    [SerializeField] private GameObject _weaponItemTemplate;
    [SerializeField] private WeaponItemData _weaponItemData;

    [SerializeField] private bool _weaponIsArmed;

    //public bool IsArmed { get { return _weaponIsArmed; } }

    public delegate void ShotFired();
    public static event ShotFired OnShotFired;

    private void OnEnable()
    {
        WeaponItem.OnWeaponItemTaken += ArmWeapon;
    }

    private void Start()
    {
        _weaponIsArmed = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && _weaponIsArmed)
        {
            Fire();

            OnShotFired?.Invoke();
        }

        if (_weaponIsArmed && Input.GetKeyDown(KeyCode.G))
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

    private void ArmWeapon(WeaponItemData weaponData)
    {
        _weaponIsArmed = true;
        _weaponItemData= weaponData;
    }

    private void DropWeapon()
    {
        var item = Instantiate(_weaponItemTemplate, _firePoint.position, _firePoint.rotation);
        item.GetComponent<SpriteRenderer>().sprite = _weaponItemData.image;
        
        //_weaponItemData = null;
        _weaponIsArmed = false;
    }

    private void OnDisable()
    {
        WeaponItem.OnWeaponItemTaken -= ArmWeapon;
    }
}
