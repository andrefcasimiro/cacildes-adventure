using System;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class VariableManager : MonoBehaviour, ISaveable
    {
        public List<Variable> variables = new List<Variable>();

        public static VariableManager instance;

        public void Awake()
        {

            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            GetVariables();
        }

        public Variable GetVariableInstance(string uuid)
        {
            return this.variables.Find(x => x.uuid == uuid);
        }

        public void GetVariables()
        {
            this.variables.Clear();
            Variable[] variables = this.transform.GetComponentsInChildren<Variable>(true);

            foreach (var _variable in variables)
            {
                this.variables.Add(_variable);
            }
        }

        public void UpdateVariable(string variableId, int nextValue)
        {
            this.variables.Find(x => x.uuid == variableId).value = nextValue;

            var sceneVariableListeners = FindObjectsOfType<VariableListener>(true);
            foreach (var sceneVariableListener in sceneVariableListeners)
            {
                if (sceneVariableListener.variableUuid == variableId)
                {
                    sceneVariableListener.EvaluateVariable();
                }
            }

        }

        public int GetVariableValue(string variableId)
        {
            var targetVariable = this.variables.Find(x => x.uuid == variableId);

            if (targetVariable == null)
            {
                return -1;
            }

            return targetVariable.value;
        }

        public void OnGameLoaded(GameData gameData)
        {
            if (gameData.variables.Length <= 0) { return; }

            foreach (SerializableVariable savedVariable in gameData.variables)
            {
                UpdateVariable(savedVariable.uuid, savedVariable.value);
            }
        }
    }
}