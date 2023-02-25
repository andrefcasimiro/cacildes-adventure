using System.Collections;

namespace AF
{
    public class EV_SwitchCondition : EV_Condition
    {
        public SwitchEntry switchEntry;
        public bool value;

        public override IEnumerator Dispatch()
        {
            yield return DispatchConditionResults(SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == value);
        }
    }
}
