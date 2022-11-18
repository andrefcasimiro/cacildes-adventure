using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableDependent : VariableListener, ISaveable
{
    public int value;

    private void Start()
    {
        EvaluateVariable();   
    }


    public void OnGameLoaded(GameData gameData)
    {
        EvaluateVariable();
    }

    public override void EvaluateVariable()
    {
        if (VariableManager.instance.GetVariableValue(this.variableUuid) == value)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

}
