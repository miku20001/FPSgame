using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    [CreateAssetMenu(menuName ="FPS/Impact Audio Data")]
    public class ImpactAudioData:ScriptableObject
    {
        public List<ImpactTagsWithAudio> impactTagsWithAudio;
    }

    [System.Serializable]
    public class ImpactTagsWithAudio
    {
        public string Tag;
        public List<AudioClip> ImpactAudioClips;
    }
}
