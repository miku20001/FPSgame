using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteplistener : MonoBehaviour
{
    public FootstepAudioData footstep;
    public AudioSource FootstepAudioSource;

    private CharacterController characterController;
    private Transform footstepTransform;

    private float nextPlayTime;   //计时
    public LayerMask layerMask;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = transform;
    }

    private void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {
                nextPlayTime += Time.deltaTime;

                bool tmp_IsHit = Physics.Linecast(footstepTransform.position,  //从中心点发射往下的射线，检测
                    footstepTransform.position+
                    Vector3.down * (characterController.height / 2+characterController.skinWidth-characterController.center.y), 
                    out RaycastHit tmp_HitInfo,layerMask);
#if UNITY_EDITOR
                Debug.DrawLine(footstepTransform.position, footstepTransform.position+Vector3.down * (characterController.height / 2+characterController.skinWidth-characterController.center.y), Color.red, 0.25f);
#endif
                if (tmp_IsHit)
                {
                    foreach(var tmp_AudioElement in footstep.FootstepAudios)
                    {
                        if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag)) 
                        {
                            if (nextPlayTime >= tmp_AudioElement.Delay)
                            {
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = UnityEngine.Random.Range(0, tmp_AudioCount);
                                AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootstepAudioSource.clip = tmp_FootstepAudioClip;
                                FootstepAudioSource.Play();
                                nextPlayTime = 0;
                                break;
                            }
                        }
                        
                    }
                }
            }
        }
    }
}
