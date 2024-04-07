using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AF.Conditions
{
    public class MapDependant : MonoBehaviour
    {
        public string mapName;

        [Header("Events")]
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        private void Awake()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool evaluationResult = SceneManager.GetActiveScene().name == mapName;

            Utils.UpdateTransformChildren(transform, evaluationResult);

            if (evaluationResult)
            {
                onTrue?.Invoke();
            }
            else
            {
                onFalse?.Invoke();
            }
        }
    }
}
