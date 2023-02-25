using UnityEngine;

namespace AF
{
    public interface IEventNavigatorCapturable
    {
        public void OnCaptured();
        public void OnInvoked();
    }

}