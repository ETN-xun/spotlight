using System.Collections;
using UnityEngine;

namespace System
{
    public class EffectInstance : MonoBehaviour
    {
        private EffectPool _pool;
        private Coroutine _autoReturn;
        private float _duration;

        internal void Initialize(EffectPool pool)
        {
            _pool = pool;
        }

        internal void Play(float duration)
        {
            _duration = duration;
            gameObject.SetActive(true);
            PlayAllParticleSystems();
            PlayAllAnimators();
            if (_autoReturn != null) StopCoroutine(_autoReturn);
            if (_duration > 0f)
            {
                _autoReturn = StartCoroutine(AutoReturn(_duration));
            }
            else
            {
                float psDur = GetParticleSystemDuration();
                if (psDur > 0f) _autoReturn = StartCoroutine(AutoReturn(psDur));
            }
        }

        private IEnumerator AutoReturn(float t)
        {
            yield return new WaitForSeconds(t);
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            if (_autoReturn != null)
            {
                StopCoroutine(_autoReturn);
                _autoReturn = null;
            }
            gameObject.SetActive(false);
            _pool?.Return(gameObject);
        }

        private void PlayAllParticleSystems()
        {
            var systems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in systems)
            {
                ps.Play(true);
            }
        }

        private void PlayAllAnimators()
        {
            var animators = GetComponentsInChildren<Animator>(true);
            foreach (var a in animators)
            {
                a.Play(0, -1, 0f);
            }
        }

        private float GetParticleSystemDuration()
        {
            var systems = GetComponentsInChildren<ParticleSystem>(true);
            float max = 0f;
            foreach (var ps in systems)
            {
                var main = ps.main;
                if (!main.loop)
                {
                    if (main.duration > max) max = main.duration;
                }
            }
            return max;
        }
    }
}
