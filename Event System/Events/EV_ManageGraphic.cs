using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManageGraphic : EventBase
    {
        public GameObject graphic;

        public bool isActive;


        public override IEnumerator Dispatch()
        {
            yield return null;

            graphic.gameObject.SetActive(isActive);
        }
    }

}
