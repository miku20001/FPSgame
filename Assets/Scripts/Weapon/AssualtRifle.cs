using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    class AssualtRifle : Firearms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        

        private FCMouseLook mouseLook;

        protected override void Awake()
        {
            base.Awake();
            reloadAmmoCheckerCoroutine = CheckReloadAnimationEnd();
            
            mouseLook = FindObjectOfType<FCMouseLook>();
        }
        protected override void Reload()
        {
            //CurrentAmmo = AmmoInMag;
            //CurrentMaxAmmoCarried -= AmmoInMag;
            GunAnimator.SetLayerWeight(2, 1);//改变动画layer权重
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");
            //StartCoroutine(CheckReloadAnimationEnd());

            FirearmsReloadAudioSource.clip = CurrentAmmo>0?firearmsAudioData.ReloadLeft:firearmsAudioData.ReloadOutOf;
            FirearmsReloadAudioSource.Play();

            if(reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);

            }
        }

        protected override void Shooting()
        {
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            MuzzleParticle.Play();
            CurrentAmmo -= 1;
            GunAnimator.Play("Fire", IsAiming?1:0, 0);

            FirearmsShootingAudioSource.clip = firearmsAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();

            CreateBullet();
            CasingParticle.Play();

            mouseLook.FiringForTest();  //后坐力

            lastFireTime = Time.time;
        }

        //protected override void Aim()
        //{
            
        
        //}

        //private void Update()
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        DoAttack();
        //    }

        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        Reload();
        //    }

        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        //瞄准
        //        IsAiming = true;
        //        Aim();
        //    }

        //    if (Input.GetMouseButtonUp(1))
        //    {
        //        //退出瞄准
        //        IsAiming = false;
        //        Aim();
        //    }
        //}

        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPerfab, MuzzlePoint.position, MuzzlePoint.rotation);

            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();
            //var tmp_BulletRigidbody = tmp_Bullet.AddComponent<Rigidbody>();
            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.ImpactPrefab = BulletImpactPrefab;
            tmp_BulletScript.ImpactAudio_Data = Impact_Audio_Data;
            tmp_BulletScript.BulletSpeed = 100;
            //tmp_BulletRigidbody.velocity = tmp_Bullet.transform.forward * 200f;
        }

        
        
    }
}
