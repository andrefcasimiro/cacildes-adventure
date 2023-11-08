using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.VirtualTexturing;

namespace AF
{
    public class ParticlePool : MonoBehaviour
    {
        // Collection checks will throw errors if we try to release an item that is already in the pool.
        public bool collectionChecks = true;
        public int maxPoolSize = 10;

        public ParticleSystem particlePrefab;

        IObjectPool<ParticleSystem> m_Pool;

        public IObjectPool<ParticleSystem> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<ParticleSystem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                }
                return m_Pool;
            }
        }
        ParticleSystem CreatePooledItem()
        {
            ParticleSystem particle = Instantiate(particlePrefab); // Instantiate a new ParticleSystem instance

            if (particle == null)
            {
                return null;
            }

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = particle.main;
            main.duration = 1;
            main.startLifetime = 1;
            main.loop = false;
            

            return particle;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(ParticleSystem system)
        {
            if (system == null)
            {
                return;
            }
            system.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(ParticleSystem system)
        {
            if (system == null)
            {
                return;
            }
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(ParticleSystem system)
        {
            if (system == null)
            {
                return;
            }
            Destroy(system.gameObject);
        }
    }

}
