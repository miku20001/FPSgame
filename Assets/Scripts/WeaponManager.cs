using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Weapon;
using Assets.Scripts.Item;

public class WeaponManager : MonoBehaviour
{
    public Firearms MainWeapon;
    public Firearms SecondaryWeapon;

    public Text AmmoCountTextLabel;

    public Firearms carriedWeapon;

    [SerializeField] private FPCharacterControllerMovement fPCharacterControllerMovement;


    private IEnumerator WaitingForHolsterEndCoroutine;

    public List<Firearms> Arms = new List<Firearms>();

    public Transform WorldCameraTransform;
    public float RaycastMaxDistance=2f;
    public LayerMask CheckItemLayerMask;


    private void UpdateAmmoInfo(int _ammo,int _remaningAmmo)
    {
        AmmoCountTextLabel.text = _ammo+"/"+_remaningAmmo;
    }

    private void Start()
    {
        Debug.Log($"Current weapon is null?{carriedWeapon == null}");

        if (MainWeapon)
        {
            carriedWeapon = MainWeapon;
            //fPCharacterControllerMovement = FindObjectOfType<FPCharacterControllerMovement>();
            fPCharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);
        }
    }

    private void Update()
    {
        CheckItem();

        if (!carriedWeapon) return;

        SwapWeapon();

        if (Input.GetMouseButton(0))
        {
            //按下扳机
            carriedWeapon.HoldTrigger();
        }

        if (Input.GetMouseButtonUp(0))
        {
            //松开扳机
            carriedWeapon.ReleaseTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            carriedWeapon.ReloadAmmo();
        }

        if (Input.GetMouseButtonDown(1))
        {
            //瞄准
            carriedWeapon.Aiming(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            //退出瞄准
            carriedWeapon.Aiming(false);
        }

        UpdateAmmoInfo(carriedWeapon.GetCurrentAmmo,carriedWeapon.GetCurrentMaxAmmoCarried);
    }

    public void CheckItem()
    {
        bool tmp_IsItem = Physics.Raycast(WorldCameraTransform.position, WorldCameraTransform.forward,
            out RaycastHit tmp_RaycastHit,
            RaycastMaxDistance,
            CheckItemLayerMask);

        if (tmp_IsItem)
        {
            Debug.Log(tmp_RaycastHit.collider.name);
            if (Input.GetKeyDown(KeyCode.E))
            {
                bool tmp_HastItem = tmp_RaycastHit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                if (tmp_HastItem)
                {
                    PickupWeapon(tmp_BaseItem);
                    PickupAttachment(tmp_BaseItem);
                }
            }
            //Debug.Log(tmp_RaycastHit.collider.name);
        }

    }

    private void PickupWeapon(BaseItem _baseItem)
    {
        if (_baseItem is FirearmItem tmp_FirearmsItem)
        {

            foreach (Firearms tmp_Arm in Arms)
            {
                if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_Arm.name) == 0)
                {
                    switch (tmp_FirearmsItem.CurrentFirearmsType)
                    {
                        case FirearmItem.FirearmsType.AssultRefile:
                            MainWeapon = tmp_Arm;
                            break;
                        case FirearmItem.FirearmsType.HandGun:
                            SecondaryWeapon = tmp_Arm;
                            break;
                        default:
                            break;
                    }

                    SetCarriedWeapon(tmp_Arm);
                }
            }

        }
    }

    private void PickupAttachment(BaseItem _baseItem)
    {
        if (_baseItem is AttachmentItem tmp_AttachmentItem)
        {
            switch (tmp_AttachmentItem.CurrentAttackmentType)
            {
                case AttachmentItem.AttachmentType.Scope:
                    foreach (ScopeInfo tmp_ScopeInfo in carriedWeapon.ScopeInfos)
                    {
                        if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.ItemName) != 0)
                        {
                            tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                            continue;
                        }
                        tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                        carriedWeapon.BaseIronSight.ScopeGameObject.SetActive(false);
                        carriedWeapon.SetupCarriedScope(tmp_ScopeInfo);
                    }
                    break;
                case AttachmentItem.AttachmentType.Other:
                    break;
                default:
                    break;

            }
            
        }

    }

    private void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //carriedWeapon.gameObject.SetActive(false);
            //carriedWeapon = MainWeapon;
            //carriedWeapon.gameObject.SetActive(true);
            //fPCharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);

            if (MainWeapon == null) return;

            if (carriedWeapon == MainWeapon) return;
            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHolsterEndCoroutine();
                carriedWeapon.GunAnimator.SetTrigger("holster");
            }
            else
            {
                SetCarriedWeapon(MainWeapon);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //carriedWeapon.gameObject.SetActive(false);
            //carriedWeapon = SecondaryWeapon;
            //carriedWeapon.gameObject.SetActive(true);
            //fPCharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);

            if (SecondaryWeapon == null) return;

            if (carriedWeapon == SecondaryWeapon) return;
            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHolsterEndCoroutine();
                carriedWeapon.GunAnimator.SetTrigger("holster");
            }
            else
            {
                SetCarriedWeapon(SecondaryWeapon);
            }
        }
    }

    private void StartWaitingForHolsterEndCoroutine()
    {
        if (WaitingForHolsterEndCoroutine == null)
        {
            WaitingForHolsterEndCoroutine = WaitingForHolsterEnd();
        }
        StartCoroutine(WaitingForHolsterEndCoroutine);
    }


    private IEnumerator WaitingForHolsterEnd()
    {
        while (true)
        {
            AnimatorStateInfo animatorStateInfo = carriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(0);
            if(animatorStateInfo.IsTag("holster")){
                if (animatorStateInfo.normalizedTime >= 0.9f)
                {
                    var tmp_TargetWeapon =  carriedWeapon == MainWeapon ? SecondaryWeapon : MainWeapon;
                    SetCarriedWeapon(tmp_TargetWeapon);
                    WaitingForHolsterEndCoroutine = null;
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void SetCarriedWeapon(Firearms _targetWeapon)
    {
        if (carriedWeapon != null)
        {
            carriedWeapon.gameObject.SetActive(false);
        }
        carriedWeapon = _targetWeapon;
        carriedWeapon.gameObject.SetActive(true);
    }

}
