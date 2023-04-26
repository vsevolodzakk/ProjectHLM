using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Vector2 _movement;

    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorOverrideController[] _animatorOverride;
    private int _currentAnimatorOverrideIndex;

    [SerializeField] private WeaponController _playerWeapon;

    private void OnEnable()
    {
        WeaponController.OnWeaponPickedUp += SwitchCharacterWeaponView;
        WeaponController.OnWeaponDroped += SwitchCharacterWeaponView;
    }

    private void Start()
    {
        _playerWeapon = GetComponent<WeaponController>();

        _currentAnimatorOverrideIndex = 0;
        _animator.runtimeAnimatorController = _animatorOverride[_currentAnimatorOverrideIndex];
    }

    private void Update()
    {
        _movement = GetComponent<PlayerController>().Direction;

        if (Input.GetButtonDown("Fire1") & _playerWeapon.AmmoCount > 0)
            Attack();

        if (_movement != Vector2.zero)
            _animator.SetFloat("Speed", 4f);
        else
            _animator.SetFloat("Speed", 0f);
    }

    private void SwitchCharacterWeaponView()
    {
        // After switch weapon to Fist will be implemented
        //  this override may be removed

        _animator.runtimeAnimatorController = _animatorOverride[0];
    }

    private void SwitchCharacterWeaponView(WeaponItemData weapontData)
    {
        _animator.runtimeAnimatorController = _animatorOverride[weapontData.Id];
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    private void OnDisable()
    {
        WeaponController.OnWeaponPickedUp -= SwitchCharacterWeaponView;
        WeaponController.OnWeaponDroped -= SwitchCharacterWeaponView;
    }
}
