
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public abstract class State : MonoBehaviour
    {
        public abstract void OnStateEnter(StateManager stateManager);
        public abstract void OnStateExit(StateManager stateManager);
        public abstract State Tick(StateManager stateManager);
    }

}