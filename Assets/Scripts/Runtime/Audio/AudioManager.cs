using UnityEngine;
using UnityEngine.Audio;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Audio
{
    using Core.Match;
    using System.Collections.Generic;

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
        [SerializeField] private AudioClip _turnClip;
        [SerializeField] private AudioClip _winClip;
        [SerializeField] private AudioClip _loseClip;

        public void PlayTurn()
        {
            _sfxSource.PlayOneShot(_turnClip);
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
        public void PlayWin()
            => _sfxSource.PlayOneShot(_winClip, 1.2f);
        public void PlayLose()
            => _sfxSource.PlayOneShot(_loseClip, 1.2f);
    }

}
