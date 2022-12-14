using UnityEngine;

[CreateAssetMenu (fileName = "WeaponItem", menuName = "ScriptableObject/New WeaponItem", order = 1)]
public class WeaponItemData : ScriptableObject
{
    public int id;
    public string weaponName;
    public Sprite image;
    public bool isRanged;
    public float attackSpeed;
    public int stockAmmo;
}
