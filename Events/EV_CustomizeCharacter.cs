using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_CustomizeCharacter : EventBase
    {
        PlayerAppearance _playerAppearance;

        public override IEnumerator Dispatch()
        {
            GetPlayerAppearance()?.ActivatePlayerAppearanceManager();
            yield return null;
        }

        PlayerAppearance GetPlayerAppearance()
        {
            if (_playerAppearance == null)
            {
                _playerAppearance = FindAnyObjectByType<PlayerAppearance>(FindObjectsInactive.Include);
            }

            return _playerAppearance;
        }

    }
}
