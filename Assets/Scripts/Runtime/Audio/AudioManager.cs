using UnityEngine;
using UnityEngine.Audio;

namespace Yujanggi.Runtime.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _selectClip;
        [SerializeField] private AudioClip _moveClip;
        [SerializeField] private AudioClip _captureClip;

        private void Awake()
        {
            
        }

        public void PlayMove()
        {
            _sfxSource.pitch = 2.0f;
            _sfxSource.PlayOneShot(_moveClip);
        }
        public void PlaySelect()
        {
            _sfxSource.pitch = 1.5f;
            _sfxSource.PlayOneShot(_selectClip);
        }
        public void PlayCapture()
            => _sfxSource.PlayOneShot(_captureClip);
    }

}
