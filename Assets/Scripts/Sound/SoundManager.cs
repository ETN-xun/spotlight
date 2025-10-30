using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] sounds;
        
        [SerializeField] private AudioSource bgmPlayer;
        [SerializeField] private AudioSource sfxPlayer;
        
        public static SoundManager Instance { get; private set; }
        
        private void Start()
        {
            bgmPlayer.Play();
        }

        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sounds.Length) return;
            sfxPlayer.PlayOneShot(sounds[index]);
        }
    }
}
