using AF;

public class SwitchDependent : SwitchListener, ISaveable
{
    public bool value;

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
        if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == value)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
