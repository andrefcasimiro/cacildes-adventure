using System.Collections;
using UnityEngine.SceneManagement;

namespace AF
{

    public class EV_Teleport : EventBase
    {

        public string spawnGameObjectName;
        public string sceneName;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(Teleport());
        }

        private IEnumerator Teleport()
        {
            yield return null;

            MapManager.instance.Teleport(sceneName, spawnGameObjectName);
        }

    }

}