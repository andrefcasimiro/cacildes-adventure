using System.Collections;
using UnityEngine;

namespace AF.Companions
{
    [System.Serializable]
    public class CompanionState
    {
        public bool isWaitingForPlayer = false;
        public string sceneNameWhereCompanionsIsWaitingForPlayer;
        public Vector3 waitingPosition;

    }
}
