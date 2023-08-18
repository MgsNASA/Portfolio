using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GooyesPlugin
{
    public class Audio : MonoBehaviour
    {
        public enum SFXType { UI, INGAME }

        #region Fields
        [SerializeField] protected AudioSource _soundSource;
        [SerializeField] protected AudioSource _UISource;
        [SerializeField] protected AudioSource _musicSource;

        private Dictionary<string, AudioClip> _typeClipTable;

        public bool SoundIsMuted
        {
            get
            {
                return SoundVolume == 0f;
            }
            set
            {
                if (value) LastActiveSoundVolume = SoundVolume;
                SetSoundVolume(value ? 0 : LastActiveSoundVolume);
            }
        }
        public bool MusicIsMuted
        {
            get
            {
                return MusicVolume == 0f;
            }
            set
            {
                if (value) LastActiveMusicVolume = MusicVolume;
                SetMusicVolume(value ? 0 : LastActiveMusicVolume);
            }
        }
        public bool VibrationIsOn
        {
            get
            {
                return PlayerPrefs.GetInt("VIBRATION_ON", 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("VIBRATION_ON", value ? 1 : 0);
            }
        }
        public static Audio instance;

        [Range(0, 1)] public float defaultMusicVolume = 0.2f;
        [Range(0, 1)] public float defaultSoundVolume = 1f;

        #endregion

        #region Properties
        private float SoundVolume
        {
            get { return PlayerPrefs.GetFloat("SOUND_VOLUME", defaultSoundVolume); }
            set { PlayerPrefs.SetFloat("SOUND_VOLUME", value); }
        }

        private float MusicVolume
        {
            get { return PlayerPrefs.GetFloat("MUSIC_VOLUME", defaultMusicVolume); }
            set { PlayerPrefs.SetFloat("MUSIC_VOLUME", value); }
        }

        private float LastActiveSoundVolume
        {
            get
            {
                float defaultVolume = instance == null ? 1 : instance.defaultSoundVolume;
                return PlayerPrefs.GetFloat("LastActiveSoundVolume", defaultVolume);
            }
            set { PlayerPrefs.SetFloat("LastActiveSoundVolume", value); }
        }

        public float LastActiveMusicVolume
        {
            get
            {
                float defaultVolume = instance == null ? 1 : instance.defaultMusicVolume;
                return PlayerPrefs.GetFloat("LastActiveMusicVolume", defaultVolume);
            }
            set { PlayerPrefs.SetFloat("LastActiveMusicVolume", value); }
        }

        #endregion

        public static Audio Get()
        {
            return instance;
        }

        #region Initizlization


        public void Init()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                _soundSource.volume = SoundVolume;
                _musicSource.volume = MusicVolume;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void InitSoundTableForScene(SceneAudio audio)
        {
            _typeClipTable = new Dictionary<string, AudioClip>();

            if (audio != null)
            {
                if (audio.allClips != null)
                {
                    foreach (AudioClip clip in audio.allClips)
                    {
                        string name = ParseSoundName(clip.name);
                        if (!string.IsNullOrEmpty(name))
                        {
                            AddSoundToTheTable(name, clip);
                        }

                    }
                }

                SetBGMusic(audio.bgMusic);
                if (audio.setBGMusicAtStart)
                {
                    _musicSource.Play();
                    return;
                }
                _musicSource.Stop();

            }
            else
            {
                Debug.LogError($"Unable to load sounds for Current Scene! {SceneManager.GetActiveScene().name}");
            }
        }

        public void SetBGMusic(AudioClip clip)
        {
            if (clip != null)
            {
                _musicSource.clip = clip;
            }
        }

        #endregion

        #region Public
        public void SetSoundVolume(float value)
        {
            SoundVolume = value;
            _soundSource.volume = value;
        }

        public void SetMusicVolume(float Value)
        {
            MusicVolume = Value;
            _musicSource.volume = Value;
        }

        public void TemporaryDisableSounds(bool disable)
        {
            if (!SoundIsMuted)
            {
                _soundSource.volume = disable ? 0f : 1f;
            }
            if (!MusicIsMuted)
            {
                _musicSource.volume = disable ? 0f : 1f;
            }
        }

        public void Vibrate(int milliseconds)
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            if (VibrationIsOn)
            {
                Handheld.Vibrate();
            }
#else
            Debug.Log("Vibrated");
#endif
        }

        public bool PlaySound(string sound, SFXType sfxType = SFXType.INGAME)
        {
            if (!SoundIsMuted && _typeClipTable != null)
            {
                return ProcessPlayRequest(sound, sfxType);
            }
            return false;
        }


        public void PlaySound(AudioClip clip)
        {
            if (_soundSource)
            {
                _soundSource.PlayOneShot(clip);
            }
        }

        public void PlaySoundWithDelay(string sound, float delay)
        {
            StartCoroutine(DelayCoroutine(sound, delay));
        }

        public void PlayBGMusic(bool play)
        {
            if (_musicSource.clip != null)
            {
                if (play)
                {
                    if (!MusicIsMuted)
                    {
                        _musicSource.Play();
                    }
                }
                else
                {
                    _musicSource.Stop();
                }
            }
        }
#endregion

        #region Helpers
        private IEnumerator DelayCoroutine(string sound, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(sound);
        }

        private bool ProcessPlayRequest(string name, SFXType sfxType)
        {
            if (_typeClipTable != null && _typeClipTable.ContainsKey(name))
            {
                AudioClip clipToPlay = _typeClipTable[name];
                if (clipToPlay != null)
                {
                    AudioSource sourceToPlay = sfxType == SFXType.UI ? _UISource : _soundSource;
                    sourceToPlay.PlayOneShot(clipToPlay);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Scene Management
        private string ParseSoundName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.StartsWith("S_"))
                {
                    name = name.Substring(2);
                    return name;
                }
                return name;
            }
            return (null);
        }

        private void AddSoundToTheTable(string soundName, AudioClip clipToAdd)
        {
            if (!string.IsNullOrEmpty(soundName) && clipToAdd != null)
            {
                if (_typeClipTable.ContainsKey(soundName))
                {
                    Debug.LogError($"[AUDIO] Sound {soundName} already exists");
                }
                else
                {
                    _typeClipTable.Add(soundName, clipToAdd);
                }
            }
        }
        #endregion
    }
}