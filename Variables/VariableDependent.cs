using AF;

public class VariableDependent : VariableListener, ISaveable
{
    public int requiredValue;

    private void Start()
    {
        Refresh();   
    }

    public void OnGameLoaded(GameData gameData)
    {
        Refresh();
    }

    public override void Refresh()
    {
        if (VariableManager.instance.GetVariableValue(variableEntry) == requiredValue)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
