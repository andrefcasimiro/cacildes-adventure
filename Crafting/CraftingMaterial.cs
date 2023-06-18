using System.Collections;
using UnityEngine;

namespace AF
{
    public class CraftingMaterial : Item
    {
        [Range(0, 100)]
        public float chanceToRuinMixture = 0;

    }
}
