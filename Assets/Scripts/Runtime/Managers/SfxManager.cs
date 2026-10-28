using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Managers
{
    public class SfxManager : MonoBehaviour
    {
        private static SfxManager _instance;

        public static SfxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SfxManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("SFXManager");
                        _instance = go.AddComponent<SfxManager>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        [SerializeField] private AudioConfig audioConfig;
        [SerializeField] private int maxAudioSources = 20;
        [SerializeField] private float defaultSFXVolume = 1f;

      

        private Queue<AudioSource> _sfxAudioSourcePool;
        private List<AudioSource> _activeSFXSources;
       
        
        private bool _isInitialized = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    DontDestroyOnLoad(transform.root.gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        public void Initialize(GlobalConfig config)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SFXManager: Already initialized!");
                return;
            }
            
            _sfxAudioSourcePool = new Queue<AudioSource>();
            _activeSFXSources = new List<AudioSource>();

            _isInitialized = true;
        }
        
        
        public AudioSource PlaySFX(SFXType sfxType, float? volume = null, float? pitch = null)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("SfXManager Not initialized");
                return null;
            }

            if (audioConfig == null)
            {
                Debug.LogError("audioConfig is not assigned");
                return null;
            }

            var clipData = audioConfig.GetSfxClip(sfxType);
            if (clipData == null || clipData.audioClip == null)
            {
                return null;
            }

            AudioSource audioSource = GetPooledAudioSource();
            if (audioSource == null)
            {
                return null;
            }
           
            audioSource.clip = clipData.audioClip;
            audioSource.volume = volume ?? clipData.volume;
            audioSource.pitch = pitch ?? clipData.pitch;
            audioSource.Play();
            
            StartCoroutine(ReturnToPoolWhenFinished(audioSource));

            return audioSource;
        }

        private AudioSource GetPooledAudioSource()
        {
            AudioSource audioSource;

            if (_sfxAudioSourcePool.Count > 0)
            {
                audioSource = _sfxAudioSourcePool.Dequeue();
            }
            else
            {
                if (_activeSFXSources.Count < maxAudioSources)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                else
                {
                    audioSource = _activeSFXSources[0];
                    _activeSFXSources.RemoveAt(0);
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
            }

            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            _activeSFXSources.Add(audioSource);
            return audioSource;
        }

        private void ReturnToPool(AudioSource audioSource)
        {
            if (audioSource == null) return;

            audioSource.Stop();
            audioSource.clip = null;
            _activeSFXSources.Remove(audioSource);
            _sfxAudioSourcePool.Enqueue(audioSource);
        }

        private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource audioSource)
        {
            if (audioSource == null) yield break;

            yield return new WaitWhile(() => audioSource != null && audioSource.isPlaying);

            if (audioSource != null)
            {
                ReturnToPool(audioSource);
            }
        }


        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}