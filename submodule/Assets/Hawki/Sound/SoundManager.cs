using System.Collections.Generic;
using UnityEngine;

namespace Hawki.Sound
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        private Dictionary<string, AudioClip> dictClips;
        private AudioSource _backgroundAudioSource;

        private List<AudioSource> audioSourcesPool = new List<AudioSource>();
        private const int POOL_INITIAL_SIZE = 3;

        private float _sound;
        private float _music;

        public float sound
        {
            get
            {
                return _sound;
            }
            set
            {
                _sound = value;
                foreach (var audio in audioSourcesPool)
                {
                    audio.volume = _sound;
                }
            }
        }

        public float music
        {
            get
            {
                return _music;
            }
            set
            {
                _music = value;

                if (_backgroundAudioSource != null)
                {
                    _backgroundAudioSource.volume = _music;
                }
            }
        }

        private void Awake()
        {
            var allClip = Resources.LoadAll<AudioClip>("Sound");

            dictClips = new Dictionary<string, AudioClip>();

            foreach (var clip in allClip)
            {
                dictClips[clip.name] = clip;
            }

            for (int i = 0; i < POOL_INITIAL_SIZE; i++)
            {
                CreateNewAudioSource();
            }
        }

        public void PlayBackground(string soundId)
        {
            if (!dictClips.ContainsKey(soundId))
            {
                Debug.LogError("Sound ID not found: " + soundId);
                return;
            }

            CreateBackgroundAudioSource();

            AudioSource availableSource = _backgroundAudioSource;
            if (availableSource != null)
            {
                availableSource.clip = dictClips[soundId];
                availableSource.volume = _music;
                availableSource.Play();
            }
            else
            {
                Debug.LogWarning("No available audio source!");
            }
        }

        public void PlaySound(string soundId)
        {
            if (!dictClips.ContainsKey(soundId))
            {
                Debug.LogError("Sound ID not found: " + soundId);
                return;
            }

            AudioSource availableSource = GetAvailableAudioSource();
            if (availableSource != null)
            {
                availableSource.clip = dictClips[soundId];
                availableSource.volume = _sound;
                availableSource.Play();
            }
            else
            {
                Debug.LogWarning("No available audio source!");
            }
        }

        private void CreateBackgroundAudioSource()
        {
            if (_backgroundAudioSource == null)
            {
                _backgroundAudioSource = CreateNewBackgroundAudioHandle();
                _backgroundAudioSource.transform.SetParent(this.transform);
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            AudioSource audioSource = CreateNewAudioHandle();
            audioSource.transform.SetParent(this.transform);
            audioSourcesPool.Add(audioSource);

            return audioSource;
        }

        private AudioSource CreateNewBackgroundAudioHandle()
        {
            var audioSourcePrefab = Resources.Load<AudioSource>("Sound/BGAudioSource");

            if (audioSourcePrefab == null)
            {
                GameObject audioSourceGameObject = new GameObject("BG_AudioSource", typeof(AudioSource));
                AudioSource audioSource = audioSourceGameObject.GetComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.volume = _music;
                return audioSource;
            } else
            {
                return Instantiate(audioSourcePrefab, this.transform);
            }
        }

        private AudioSource CreateNewAudioHandle()
        {
            var audioSourcePrefab = Resources.Load<AudioSource>("Sound/VFXAudioSource");

            if (audioSourcePrefab == null)
            {
                GameObject audioSourceGameObject = new GameObject("AudioSource", typeof(AudioSource));
                AudioSource audioSource = audioSourceGameObject.GetComponent<AudioSource>();
                return audioSource;
            } else
            {
                return Instantiate(audioSourcePrefab, this.transform);
            }
        }

        private AudioSource GetAvailableAudioSource()
        {
            foreach (AudioSource source in audioSourcesPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // Nếu không có AudioSource sẵn sàng, tạo mới và thêm vào pool
            return CreateNewAudioSource();
        }
    }
}
