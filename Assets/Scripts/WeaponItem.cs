using UnityEngine;

public class WeaponItem: MonoBehaviour
{
    public WeaponItemData weaponData;
    public int ammo; // !!!
    public bool isDroped = false;

    private void Start()
    {
        var weaponImage = GetComponent<SpriteRenderer>();

        if (weaponData != null)
        {
            weaponImage.sprite = weaponData.Image;
            if(!isDroped) ammo = weaponData.StockAmmo;
        }
    }
}
