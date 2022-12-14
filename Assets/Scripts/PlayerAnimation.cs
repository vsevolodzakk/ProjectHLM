using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Vector2 _movement;

    private WeaponController _weaponController;

    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorOverrideController[] _animatorOverride;
    private int _currentAnimatorOverrideIndex;

    private void OnEnable()
    {
        WeaponItem.OnWeaponItemTaken += SwitchCharacterWeaponView;
    }

    private void Start()
    {
        _weaponController= GetComponent<WeaponController>();

        _currentAnimatorOverrideIndex = 0;
        _animator.runtimeAnimatorController = _animatorOverride[_currentAnimatorOverrideIndex];
    }

    private void Update()
    {
        _movement = GetComponent<PlayerController>().Direction;

        if (Input.GetButtonDown("Fire1"))
            Attack();

        if (_movement != Vector2.zero)
            _animator.SetFloat("Speed", 4f);
        else
            _animator.SetFloat("Speed", 0f);

        //if (!_weaponController.IsArmed)
        //    SwitchCharacterWeaponView(0);
    }

    private void SwitchCharacterWeaponView(WeaponItemData weaponData)
    {
        _animator.runtimeAnimatorController = _animatorOverride[weaponData.id];
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");
    }
}
