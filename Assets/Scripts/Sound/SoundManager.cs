using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] sounds;
        private AudioSource _audioSource;
        // Start is called before the first frame update
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.Play();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
