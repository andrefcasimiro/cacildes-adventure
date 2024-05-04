using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF.Companions
{
    [CreateAssetMenu(fileName = "Companion ID", menuName = "System/New Companion ID", order = 0)]
    public class CompanionID : ScriptableObject
    {
        public string GetCompanionID()
        {
            return name;
        }
    }

}
