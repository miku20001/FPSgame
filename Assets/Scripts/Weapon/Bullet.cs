using System;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class Bullet:MonoBehaviour
    {
        public float BulletSpeed;
        public GameObject ImpactPrefab;
        public ImpactAudioData ImpactAudio_Data;
        //private Rigidbody bulletRigidbody;
        private Transform bulletTransform;
        private Vector3 prevPosition;

        private void Start()
        {
            //bulletRigidbody = GetComponent<Rigidbody>();
            bulletTransform = transform;
            prevPosition = bulletTransform.position;

        }

        private void Update()
        {
            prevPosition = bulletTransform.position;
            bulletTransform.Translate(0, 0, BulletSpeed * Time.deltaTime);
            //bulletRigidbody.velocity = bulletTransform.forward * BulletSpeed*Time.fixedTime;
            if(Physics.Raycast(prevPosition,(bulletTransform.position-prevPosition).normalized,out RaycastHit tmp_Hit, (bulletTransform.position-prevPosition).magnitude))
            {
                //Debug.DrawRay(bulletTransform.position, bulletTransform.forward, Color.red, 0.1f);
                var tmp_BullectEffect = Instantiate(ImpactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
                Destroy(tmp_BullectEffect, 3);


                var tmp_TagsWithAudio =  ImpactAudio_Data.impactTagsWithAudio.Find((_audioData) => _audioData.Tag.Equals(tmp_Hit.collider.tag));
                if (tmp_TagsWithAudio == null) return;
                int tmp_Length = tmp_TagsWithAudio.ImpactAudioClips.Count;
                AudioClip tmp_AudioClip = tmp_TagsWithAudio.ImpactAudioClips[UnityEngine.Random.Range(0, tmp_Length)];
                AudioSource.PlayClipAtPoint(tmp_AudioClip, tmp_Hit.point);
                
            }
        }
    }
}
