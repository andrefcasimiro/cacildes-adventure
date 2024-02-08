using UnityEngine;

namespace AF.Conditions
{
    public class ChildrenRandomizer : MonoBehaviour
    {
        private void Awake()
        {
            RandomizeChildren();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void Randomize()
        {
            RandomizeChildren();
        }

        public int RandomizeChildren()
        {
            Utils.UpdateTransformChildren(transform, false);

            if (transform.childCount <= 0)
            {
                return -1;
            }

            int idx = Random.Range(0, transform.childCount);
            if (idx != -1)
            {
                transform.GetChild(idx).gameObject.SetActive(true);
            }

            return idx;
        }
    }
}
