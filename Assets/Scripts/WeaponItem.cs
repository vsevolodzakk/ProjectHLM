using UnityEditor.XR;
using UnityEngine;

public class WeaponItem: MonoBehaviour
{
    [SerializeField] private WeaponItemData _weaponData;
    private CircleCollider2D _pickupZoneTrigger;

    private bool _isRanged;

    public delegate void WeaponItemTaken(WeaponItemData weaponData);
    public static event WeaponItemTaken OnWeaponItemTaken;

    public bool IsRanged { get { return _isRanged; } }

    private void Start()
    {
        _pickupZoneTrigger= GetComponent<CircleCollider2D>();
        var weaponImage = GetComponent<SpriteRenderer>();

        if(_weaponData != null)
        {
            weaponImage.sprite = _weaponData.image;
            _isRanged = _weaponData.isRanged; //???
        }
    }

    private void PickupWeapon()
    {
        Destroy(gameObject); // for testing

        OnWeaponItemTaken?.Invoke(_weaponData);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            PickupWeapon();
    }
}
