using UnityEngine;
using UnityEngine.Events;

namespace AF.Dialogue
{

    public class Response : MonoBehaviour
    {

        [TextArea] public string text;

        [Header("Unity Events")]
        public UnityEvent onResponseSelected;
        public UnityEvent onResponseFinished;

        [Header("Use Sub Events")]
        public GameObject subEventPage;

        [Header("Use Quick Reply")]
        public AF.Character replier;
        [TextArea] public string reply;

        /*
            [Header("Depends on Switch?")]
            public SwitchEntry switchEntry;
            public bool switchValue = false;

            public SwitchEntry switchEntry2;
            public bool switchValue2;
        */

        [Header("Reputation")]
        public int reputationAmountToIncrease = 0;
        public int reputationAmountToDecrease = 0;
        //public SwitchEntry reputationSwitchEntry;
    }

}