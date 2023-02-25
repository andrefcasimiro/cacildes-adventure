﻿using System.Collections;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "NPCs / Characters / New Character")]
    public class Character : ScriptableObject
    {
        public string name;
        public LocalizedText title;
        public Sprite avatar;

    }
}