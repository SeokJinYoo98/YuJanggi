using UnityEngine;
using UnityEngine.Audio;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Audio
{
    using Core.Match;
    using System.Collections.Generic;

    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private SfxAudios  _sfx;
        [SerializeField] private UIAudio    _ui;

        private float _sfxVolume = 1.0f;
        private float _uiVolume  = 1.0f;
        public void PlaySfxOneShot(JanggiSfx type)
            => _sfx.PlaySfx(type, _sfxVolume);
        public void PlayUI(UISfx type)
            => _ui.PlayUI(type, _uiVolume);
        public void PlayButton()
            => PlayUI(UISfx.Button);
    }

}
