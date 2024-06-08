using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_Instantiate : EventBase
    {
        public GameObject objectToInstantiate;
        public Transform origin;
        public bool unparentOnInstantiate = true;

        public bool shouldRiseFromBelow = false;

        public override IEnumerator Dispatch()
        {
            GameObject instance = Instantiate(objectToInstantiate,
                shouldRiseFromBelow ? origin.up * -1f : origin.position,
                origin.transform.rotation, unparentOnInstantiate ? null : this.transform);


            if (shouldRiseFromBelow)
            {
                Vector3 targetPosition = origin.position;
                Vector3 startPosition = targetPosition - Vector3.up * 10f; // Start below the origin

                float elapsedTime = 0f;
                while (elapsedTime < 1f)
                {
                    float t = elapsedTime / 1f;
                    instance.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                // Ensure the object reaches the exact target position
                instance.transform.position = targetPosition;
            }


            yield return null;
        }
    }

}
