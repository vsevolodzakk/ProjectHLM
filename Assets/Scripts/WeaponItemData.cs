using UnityEngine;

[CreateAssetMenu (fileName = "WeaponItem", menuName = "ScriptableObject/New WeaponItem", order = 1)]
public class WeaponItemData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _weaponName;
    [SerializeField] private Sprite _image;
    [SerializeField] private bool _isRanged;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _stockAmmo;

    public int Id { get { return _id; } }
    public string WeaponName { get { return _weaponName; } }
    public Sprite Image { get { return _image; } }
    public bool IsRanged { get { return _isRanged; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int StockAmmo { get { return _stockAmmo; } }
}
