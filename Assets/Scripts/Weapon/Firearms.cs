using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public abstract class Firearms:MonoBehaviour,IWeapon
    {
        public Camera EyeCamera;
        public Camera GunCamera;

        public Transform MuzzlePoint;
        public Transform CasingPoint;

        public ParticleSystem MuzzleParticle;
        public ParticleSystem CasingParticle;

        public AudioSource FirearmsShootingAudioSource;  //射击音效
        public AudioSource FirearmsReloadAudioSource;    //换弹音效
        public FirearmsAudioData firearmsAudioData;
        public ImpactAudioData Impact_Audio_Data;     //从此处引入再初始化时赋给bullet脚本
        public GameObject BulletImpactPrefab;

        public int AmmoInMag = 30;    //弹夹弹药量
        public int MaxAmmoCarried = 1200;    //最大携弹量
        public GameObject BulletPerfab;   //子弹预制体

        public float SpreadAngle;   //散射程度

        public float FireRate = 11.7f;
        protected float lastFireTime;

        public int GetCurrentAmmo
        {
            get { return CurrentAmmo; }
        }

        public int GetCurrentMaxAmmoCarried
        {
            get { return CurrentMaxAmmoCarried; }
        }

        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;

        public List<ScopeInfo> ScopeInfos;

        public ScopeInfo BaseIronSight;
        protected ScopeInfo rigoutScopeInfo;

        internal Animator GunAnimator;
        protected AnimatorStateInfo GunStateInfo;

        protected float OriginFOV;
        protected float GunOriginFOV;
        protected bool IsAiming;
        public bool IsHoldingTrigger;

        private IEnumerator doAimCoroutine;

        private Vector3 originalEyePosition;
        protected Transform gunCamraTransform;

        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();
            OriginFOV = EyeCamera.fieldOfView;
            GunOriginFOV = GunCamera.fieldOfView;
            doAimCoroutine = DoAim();
            gunCamraTransform = GunCamera.transform;
            originalEyePosition = gunCamraTransform.localPosition;
            rigoutScopeInfo = BaseIronSight;

        }


        public void DoAttack()
        {
            Shooting();

            
        }

        protected abstract void Shooting();
        protected abstract void Reload();

        //protected abstract void Aim();

        internal void Aiming(bool _isAiming)
        {
            IsAiming = _isAiming;
            GunAnimator.SetBool("Aim", IsAiming);
            if (doAimCoroutine == null)
            {
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
            else
            {
                StopCoroutine(doAimCoroutine);
                doAimCoroutine = null;
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
        }

        protected bool IsAllowShooting()
        {
            return Time.time - lastFireTime > 1 / FireRate;
            
        }

        protected Vector3 CalculateSpreadOffset()
        {
            float tmp_SpreadPercent =  SpreadAngle / EyeCamera.fieldOfView;  //散射百分比

            return tmp_SpreadPercent * UnityEngine.Random.insideUnitCircle;

        }


        protected IEnumerator CheckReloadAnimationEnd()
        {
            while (true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
                if (GunStateInfo.IsTag("ReloadAmmo"))   //两种换弹动画都带有ReloadAmmo的Tag
                {
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        //CurrentAmmo = AmmoInMag;
                        int tmp_NeedAmmoCount = AmmoInMag - CurrentAmmo;
                        int tmp_RemainingAmmo = CurrentMaxAmmoCarried - tmp_NeedAmmoCount;
                        if (tmp_RemainingAmmo <= 0)
                        {
                            CurrentAmmo += CurrentMaxAmmoCarried;
                        }
                        else
                        {
                            CurrentAmmo = AmmoInMag;
                        }

                        CurrentMaxAmmoCarried = tmp_RemainingAmmo <= 0 ? 0 : tmp_RemainingAmmo;
                        yield break;
                    }
                }

            }
        }

        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;
                float tmp_CurrentFOV = 0;
                EyeCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView, IsAiming ? rigoutScopeInfo.EyeFov : OriginFOV,
                    ref tmp_CurrentFOV, Time.deltaTime * 2);
                float tmp_GunCurrentFOV = 0;
                GunCamera.fieldOfView = Mathf.SmoothDamp(GunCamera.fieldOfView, IsAiming ? rigoutScopeInfo.GunFov : GunOriginFOV,
                    ref tmp_GunCurrentFOV, Time.deltaTime * 2);

                Vector3 tmp_RefPosition = Vector3.zero;
                gunCamraTransform.localPosition = Vector3.SmoothDamp(gunCamraTransform.localPosition, IsAiming ? rigoutScopeInfo.GunCameraPosition : originalEyePosition,
                    ref tmp_RefPosition,
                    Time.deltaTime * 2);
            }
        }

        internal void HoldTrigger()
        {
            DoAttack();
            IsHoldingTrigger = true;
        }

        internal void ReleaseTrigger()
        {
            IsHoldingTrigger = false;
        }

        internal void ReloadAmmo()
        {
            Reload();
        }

        internal void SetupCarriedScope(ScopeInfo _scopeInfo)
        {
            rigoutScopeInfo = _scopeInfo;
        }

    }

    [System.Serializable]
    public class ScopeInfo
    {
        public string ScopeName;
        public GameObject ScopeGameObject;
        public float EyeFov;
        public float GunFov;
        public Vector3 GunCameraPosition;
    }
}
