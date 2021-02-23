using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Weapon;

public class CrosshairUI : MonoBehaviour
{
    public RectTransform Reticle;
    public CharacterController characterController;

    [SerializeField]
    private Firearms carriedWeapon;

    private WeaponManager weaponManager;

    public float OriginalSize;
    public float TargetSize;

    private float currentSize;
    private bool IsFiring;

    private void Start()
    {
        weaponManager = characterController.GetComponent<WeaponManager>();
    }

    private void Update()
    {
        bool tmp_IsMoving = characterController.velocity.magnitude > 0;

        carriedWeapon = weaponManager.carriedWeapon;

        if (carriedWeapon == null)
        {
            IsFiring = false;
        }
        else
        {
            IsFiring = carriedWeapon.IsHoldingTrigger;
        }

        if (tmp_IsMoving || IsFiring)
        {
            currentSize = Mathf.Lerp(currentSize, TargetSize, Time.deltaTime * 5);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, OriginalSize, Time.deltaTime * 5);
        }
        Reticle.sizeDelta = new Vector2(currentSize,currentSize);
    }
}
