using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AF
{
    public class ParticlePoolManager : MonoBehaviour
    {
        public ParticlePool goldPool;

        public ParticlePool bloodPool;

        public ParticlePool stunnedFxPool;

        public IEnumerator DropBloodOnEnemy(Vector3 origin, Color startColor)
        {
            var blood = bloodPool.Pool.Get();
            var main = blood.main;
            main.startColor = startColor;
            blood.transform.position = origin;
            blood.Play();

            yield return new WaitForSeconds(2f);

            bloodPool.Pool.Release(blood);
        }

    }

}
