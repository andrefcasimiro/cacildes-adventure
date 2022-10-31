using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_FaceEachOther : EventBase
    {
        public Transform npc;

        public override IEnumerator Dispatch()
        {
            Transform player = GameObject.FindWithTag("Player").transform;

            var npcLookPos = player.transform.position - npc.transform.position;
            npcLookPos.y = 0;

            var playerLookPos = npc.transform.position - player.transform.position;
            playerLookPos.y = 0;

            npc.transform.rotation = Quaternion.LookRotation(npcLookPos);
            player.transform.rotation = Quaternion.LookRotation(playerLookPos);

            yield return null;
        }
    }
}