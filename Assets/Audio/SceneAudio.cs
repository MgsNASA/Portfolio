using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GooyesPlugin
{
    public class SceneAudio : MonoBehaviour
    {
        public List<AudioClip> allClips;
        public AudioClip bgMusic;
        public bool setBGMusicAtStart = true;

        public void Init()
        {
            if (Audio.Get() != null)
            {
                Destroy(gameObject);
                return;
            }
            Audio audio = GetComponent<Audio>();
            audio.Init();
            audio.InitSoundTableForScene(this);
        }
    }
}
