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
        private void Awake()
        {
            
        }

        public void BindEvents(MatchEvents events)
        {
            events.OnPieceMoved         += PlayMove;
            events.OnSelectionChanged   += PlaySelect;
            events.OnMunggun            += PlayMunggun;
            events.OnCheck              += PlayJanggun;
            events.OnPieceCaptured      += PlayCapture;
        }
        public void UnBindEvents(MatchEvents events)
        {
            events.OnPieceMoved         -= PlayMove;
            events.OnSelectionChanged   -= PlaySelect;
            events.OnMunggun            -= PlayMunggun;
            events.OnCheck              -= PlayJanggun;
            events.OnPieceCaptured      -= PlayCapture;
        }
        private void PlayJanggun(PlayerTeam team)
        {
            _sfxSource.PlayOneShot(_janggunClip, 1.5f);
        }
        private void PlayMunggun()
        {
            _sfxSource.PlayOneShot(_munggunClip, 1.5f);
        }
        private void PlayMove(MoveRecord _)
        {
            _sfxSource.PlayOneShot(_moveClip, 0.6f);
        }
        private void PlaySelect(int? idx, IReadOnlyList<Pos> _)
        {
            if (idx.HasValue)
                _sfxSource.PlayOneShot(_selectClip);
        }
        private void PlayCapture(PieceType _)
        {
            _sfxSource.PlayOneShot(_captureClip, 0.8f);
        }

        public void PlayButton()
        {
            _sfxSource.PlayOneShot(_buttonClip);
        }

    }

}
