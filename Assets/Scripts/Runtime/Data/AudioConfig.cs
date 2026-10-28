using System;
using System.Collections.Generic;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [Serializable]
    public class AudioClipData
    {
        public AudioClip audioClip;
        public float volume;
        public float pitch;
    }

    [Serializable]
    public class AudioClipPairData
    {
        public SFXType sfxType;
        public AudioClipData audioClip;
    }
    
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Data/Config/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public List<AudioClipPairData> audioClipPairs = new ();
        public Dictionary<SFXType, AudioClipData> audioClipsCached = new ();

        public AudioClipData GetSfxClip(SFXType sfxType)
        {
            CachePairs();
            if (audioClipsCached.Count > 0 && audioClipsCached.ContainsKey(sfxType))
            {
                return audioClipsCached[sfxType];
            }
            Debug.LogWarning("SFX clip not found");
            return null;
        }

        private void CachePairs()
        {
            
            foreach (var pair in audioClipPairs)
            {
                if (audioClipsCached.ContainsKey(pair.sfxType))
                    continue;
                audioClipsCached.Add(pair.sfxType, pair.audioClip);
            }
        }
        
    }
}
