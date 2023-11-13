using UnityEngine;

namespace AF.UI.EquipmentMenu
{

    public class ViewEquipmentMenu : ViewMenu
    {

        [Header("UI Components")]
        public EquipmentSlots equipmentSlots;
        public ItemTooltip itemTooltip;
        public PlayerStatsAndAttributesUI playerStatsAndAttributesUI;
        public ItemList itemList;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        protected override void OnEnable()
        {
            base.OnEnable();

            equipmentSlots.gameObject.SetActive(false);
            itemList.gameObject.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            playerStatsAndAttributesUI.gameObject.SetActive(true);

            InitUI();
        }

        private void OnDisable()
        {
            equipmentSlots.gameObject.SetActive(false);
            itemList.gameObject.SetActive(false);
            itemTooltip.gameObject.SetActive(false);
            playerStatsAndAttributesUI.gameObject.SetActive(false);

            equipmentSlots.shouldRerender = true;
            itemList.shouldRerender = true;
            itemTooltip.shouldRerender = true;
            playerStatsAndAttributesUI.shouldRerender = true;
        }


        void InitUI()
        {
            equipmentSlots.gameObject.SetActive(true);

            playerStatsAndAttributesUI.DrawStats(null);
        }

    }

}
