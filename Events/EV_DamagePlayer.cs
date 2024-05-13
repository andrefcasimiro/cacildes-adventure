using System.Collections;
using System.Linq;
using AF.Dialogue;
using UnityEngine;
using AF.Health;

namespace AF
{
    public class EV_DamagePlayer : EventBase
    {
        public Damage damage;

        PlayerManager _playerManager;

        public override IEnumerator Dispatch()
        {
            GetPlayerManager().damageReceiver.TakeDamage(damage);

            yield return null;
        }


        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null)
            {
                _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);

            }

            return _playerManager;
        }

    }
}
