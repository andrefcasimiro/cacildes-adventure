using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyElementalDamageController : MonoBehaviour
    {
        [Header("Elemental Damage Bonus")]
        public int fireDamageBonus = 0;
        public int frostDamageBonus = 0;

        [Header("FX")]
        public GameObject fireFx;
        public Color fireTextColor;
        public GameObject frostFx;
        public Color frostTextColor;

        [Header("Refs")]
        public FloatingText elementalDamageFloatingText;


        public void OnFireDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = fireTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(fireFx, this.transform.position, Quaternion.identity);
        }

        public void OnFrostDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = frostTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(frostFx, this.transform.position, Quaternion.identity);
        }
    }

}