

using System;
using UnityEngine;

namespace Yujanggi.Runtime.Particle
{
    public sealed class PooledParticle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private Action<PooledParticle> _onStopped;
        public void Initialize(Action<PooledParticle> onStopped)
        {
            Debug.Log("Init Particle");
            _onStopped = onStopped;

            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (_particleSystem == null)
            {
                Debug.LogError($"{nameof(PooledParticle)} requires ParticleSystem.");
                return;
            }

            var main = _particleSystem.main;
            main.playOnAwake = false;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        public void Play(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            gameObject.SetActive(true);

            _particleSystem.Clear(true);
            _particleSystem.Play(true);
        }

        private void OnParticleSystemStopped()
        {
            Debug.Log("ComebackHome Particle");
            _particleSystem.Clear(true);
            gameObject.SetActive(false);
            _onStopped?.Invoke(this);
        }
    }
}