using UnityEngine;

namespace AF.Misc
{
    public class DeactivateChildren : MonoBehaviour
    {
        public void Execute()
        {
            Utils.UpdateTransformChildren(transform, false);
        }
    }
}

