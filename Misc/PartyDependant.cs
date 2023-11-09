using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class PartyDependant : MonoBehaviour, ISaveable
    {
        public Companion companion;
        public bool requireInParty;

        private void Start()
        {
            Reevaluate();
        }

        public void Reevaluate()
        {
            /*if (requireInParty && Player.instance.companions.Exists(x => x.companionId == companion.companionId))
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                return;
            }

            if (requireInParty == false && Player.instance.companions.Exists(x => x.companionId == companion.companionId) == false)
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                return;
            }*/

            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        public void OnGameLoaded(object gameData)
        {
            Reevaluate();
        }
    }

}
