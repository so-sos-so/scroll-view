using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    public class UIEffect : UIBehaviour
    {
        private List<ParticleSystem> m_ParticleSystems = new List<ParticleSystem>();

        public bool includeChildren = true;
        protected override void OnEnable()
        {
            GetComponentsInChildren<ParticleSystem>(false, m_ParticleSystems);
        }

        public bool IsAlive()
        {
            foreach (var particleSystem in m_ParticleSystems)
            {
                if (particleSystem.IsAlive(includeChildren))
                {
                    return true;
                }
            }
            return false;
        }

        public void Pause()
        {
            foreach (var particleSystem in m_ParticleSystems)
            {
                particleSystem.Pause(includeChildren);
            }
        }
        
        public void Play()
        {
            foreach (var particleSystem in m_ParticleSystems)
            {
                particleSystem.Play(includeChildren);
            }
        }

        public void Stop()
        {
            foreach (var particleSystem in m_ParticleSystems)
            {
                particleSystem.Stop(includeChildren);
            }
        }
    }
}