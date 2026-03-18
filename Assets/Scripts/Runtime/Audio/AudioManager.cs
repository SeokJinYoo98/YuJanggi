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
        [SerializeField] private AudioClip _buttonClip;
        [SerializeField] private AudioClip _janggunClip;
        [SerializeField] private AudioClip _munggunClip;
        [SerializeField] private AudioClip _checkmateClip;
        private void Awake()
        {
            
        }
        public void PlayJanggun()
        {
            _sfxSource.PlayOneShot(_janggunClip, 1.5f);
        }
        public void PlayMunggun()
        {
            _sfxSource.PlayOneShot(_munggunClip, 1.5f);
        }
        public void PlayMove()
        {
            _sfxSource.PlayOneShot(_moveClip, 0.6f);
        }
        public void PlaySelect()
        {
            _sfxSource.PlayOneShot(_selectClip);
        }
        public void PlayCapture()
        {
            _sfxSource.PlayOneShot(_captureClip, 0.8f);
        }

        public void PlayButton()
        {
            _sfxSource.PlayOneShot(_buttonClip);
        }

    }

}
