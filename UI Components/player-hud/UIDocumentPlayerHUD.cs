using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentPlayerHUD: UIDocumentBase
    {

        public IMGUIContainer hpbar;
        public IMGUIContainer mpBar;
        public IMGUIContainer staminaBar;

        protected override void Start()
        {
            base.Start();

            hpbar = root.Q<IMGUIContainer>("hp-bar");
            mpBar = root.Q<IMGUIContainer>("mp-bar");
            staminaBar = root.Q<IMGUIContainer>("stamina-bar");
        }

        public void Update()
        {
            if (this.hpbar == null)
            {
                return;
            }

            float maxHealthPoints = PlayerStatsManager.instance.GetMaxHealthPoints();
            float currentHealth = PlayerStatsManager.instance.GetCurrentHealth();

            float healthPercentage = Mathf.Clamp(
                (currentHealth * 100) / maxHealthPoints,
                0,
                100
            );

            Length formattedHp = new Length(healthPercentage, LengthUnit.Percent);
            this.hpbar.style.width = formattedHp;
        }

    }

}
