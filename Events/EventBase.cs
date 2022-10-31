using System.Collections;
using UnityEngine;

namespace AF
{

    [System.Serializable]
    public class EventBase : MonoBehaviour
    {
        public virtual IEnumerator Dispatch()
        {
            yield return null;
        }

    }

}