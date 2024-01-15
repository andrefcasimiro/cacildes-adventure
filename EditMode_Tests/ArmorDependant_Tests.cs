using UnityEngine;
using NUnit.Framework;
using AF.Companions;
using AF.Conditions;

namespace AF.Tests
{

    public class ArmorDependant_Tests : MonoBehaviour
    {
        ArmorDependant armorDependant;
        GameObject child1;
        GameObject child2;
        EquipmentDatabase equipmentDatabase;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new();
            armorDependant = go.AddComponent<ArmorDependant>();

            equipmentDatabase = ScriptableObject.CreateInstance<EquipmentDatabase>();
            armorDependant.equipmentDatabase = equipmentDatabase;

            // Add children
            child1 = Instantiate(new GameObject(), armorDependant.transform);
            child2 = Instantiate(new GameObject(), armorDependant.transform);
        }


        [Test]
        public void ShouldActivateChildren_IfArmorIsEquipped_AndOnlyArmorIsRequiredToBeEquipped()
        {
            Helmet ironHelmet = ScriptableObject.CreateInstance<Helmet>();
            Armor ironArmor = ScriptableObject.CreateInstance<Armor>();
            Armor steelArmor = ScriptableObject.CreateInstance<Armor>();
            Gauntlet ironGauntlet = ScriptableObject.CreateInstance<Gauntlet>();
            Legwear ironLegwear = ScriptableObject.CreateInstance<Legwear>();

            armorDependant.helmet = ironHelmet;
            armorDependant.armor = ironArmor;
            armorDependant.gauntlet = ironGauntlet;
            armorDependant.legwear = ironLegwear;

            equipmentDatabase.armor = ironArmor;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            armorDependant.requireOnlyTorsoArmorToBeEquipped = true;
            armorDependant.Evaluate();

            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            equipmentDatabase.armor = null;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            equipmentDatabase.armor = steelArmor;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);
        }

        [Test]
        public void ShouldActivateChildren_IfAllArmorIsEquipped_AndAllrmorIsRequiredToBeEquipped()
        {
            Helmet ironHelmet = ScriptableObject.CreateInstance<Helmet>();
            Armor ironArmor = ScriptableObject.CreateInstance<Armor>();
            Armor steelArmor = ScriptableObject.CreateInstance<Armor>();
            Gauntlet ironGauntlet = ScriptableObject.CreateInstance<Gauntlet>();
            Legwear ironLegwear = ScriptableObject.CreateInstance<Legwear>();

            armorDependant.helmet = ironHelmet;
            armorDependant.armor = ironArmor;
            armorDependant.gauntlet = ironGauntlet;
            armorDependant.legwear = ironLegwear;

            equipmentDatabase.armor = ironArmor;
            armorDependant.requireAllPiecesToBeEquipped = true;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            equipmentDatabase.helmet = ironHelmet;
            equipmentDatabase.legwear = ironLegwear;
            equipmentDatabase.gauntlet = ironGauntlet;

            armorDependant.Evaluate();

            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            equipmentDatabase.armor = steelArmor;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);
        }


        [Test]
        public void ShouldActivateChildren_IfNoArmorIsEquipped_AndPlayerIsRequiredToBeNaked()
        {
            Gauntlet steelGauntlet = ScriptableObject.CreateInstance<Gauntlet>();
            Armor steelArmor = ScriptableObject.CreateInstance<Armor>();
            equipmentDatabase.armor = steelArmor;

            armorDependant.requirePlayerToBeNaked = true;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);

            equipmentDatabase.armor = null;
            equipmentDatabase.helmet = null;
            equipmentDatabase.legwear = null;
            equipmentDatabase.gauntlet = null;

            armorDependant.Evaluate();
            Assert.IsTrue(child1.activeSelf);
            Assert.IsTrue(child2.activeSelf);

            equipmentDatabase.gauntlet = steelGauntlet;

            armorDependant.Evaluate();

            Assert.IsFalse(child1.activeSelf);
            Assert.IsFalse(child2.activeSelf);
        }
    }
}
