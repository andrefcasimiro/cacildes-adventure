using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDependent : SwitchListener, ISaveable
{
    public bool value;

    private void Awake()
    {
        // This is useless. We dont always need switch (the game object itself)
        this._switch = SwitchManager.instance.GetSwitchInstance(this.switchUuid);
    }

    private void Start()
    {
        EvaluateSwitch();
    }

    public void OnGameLoaded(GameData gameData)
    {
        EvaluateSwitch();
    }

    public override void EvaluateSwitch()
    {
        if (SwitchManager.instance.GetSwitchValue(this.switchUuid) == value)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

}
