using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBossHUD : UIDocumentBase
    {
        public Enemy enemy;
        public string bossName;

        public IMGUIContainer hpbar;
        public Label bossNameLabel;

        protected override void Start()
        {
            base.Start();

            hpbar = root.Q<IMGUIContainer>("hp-bar");
            bossNameLabel = root.Q<Label>("boss-name");

            bossNameLabel.text = bossName;
        }

        public void Update()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            if (this.hpbar == null)
            {
                return;
            }

            float maxHealthPoints = enemy.GetMaxHealth();
            float currentHealth = enemy.GetCurrentHealth();

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
