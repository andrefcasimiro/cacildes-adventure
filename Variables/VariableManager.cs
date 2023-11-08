using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class VariableEntryInstance
    {
        public VariableEntry variableEntry;
        public int currentValue;
        public int initialValue;
    }

    public class VariableManager : MonoBehaviour, ISaveable
    {
        public List<VariableEntryInstance> variableEntryInstances = new();

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

            LoadVariables();
        }

        public void LoadVariables()
        {
            variableEntryInstances.Clear();
            var variableEntries = Resources.LoadAll<VariableEntry>("Variables").ToList();

            foreach (var _variable in variableEntries)
            {
                VariableEntryInstance variableEntryInstance = new()
                {
                    variableEntry = _variable,
                    currentValue = _variable.value,
                    initialValue = _variable.value
                };

                variableEntryInstances.Add(variableEntryInstance);
            }
        }

        public void UpdateVariable(VariableEntry variableToUpdate, int nextValue)
        {
            var idx = variableEntryInstances.FindIndex(variableEntryInstance => variableEntryInstance.variableEntry == variableToUpdate);

            if (idx == -1)
            {
                Debug.LogError("Could not find variable: " + variableToUpdate.name);
                return;
            }

            variableEntryInstances[idx].currentValue = nextValue;

            var sceneVariableListeners = FindObjectsOfType<VariableListener>(true);
            foreach (var sceneVariableListener in sceneVariableListeners)
            {
                if (sceneVariableListener.variableEntry == variableToUpdate)
                {
                    sceneVariableListener.Refresh();
                }
            }

        }

        public int GetVariableValue(VariableEntry variableEntry)
        {
            var targetVariable = variableEntryInstances.Find(
                variableEntryInstance => variableEntryInstance.variableEntry == variableEntry);

            return targetVariable == null ? -1 : targetVariable.currentValue;
        }

        public void ResetVariables()
        {
            if (variableEntryInstances.Count <= 0) { return; }    

            foreach (VariableEntryInstance variableEntryInstance in variableEntryInstances)
            {
                variableEntryInstance.currentValue = -1;
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            if (gameData.variables.Length <= 0) { return; }

            foreach (SerializableVariable savedVariable in gameData.variables)
            {
                var variableIndex = variableEntryInstances.FindIndex(variableEntryInstance => variableEntryInstance.variableEntry.name == savedVariable.variableName);
                if (variableIndex == -1)
                {
                    Debug.LogError("Could not find variable with name: " + savedVariable.variableName);
                    return;
                }

                UpdateVariable(variableEntryInstances[variableIndex].variableEntry, savedVariable.value);
            }
        }
    }
}
