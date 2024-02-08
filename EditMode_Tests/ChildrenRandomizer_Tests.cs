using AF.Conditions;
using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class ChildrenRandomizer_Tests
    {
        [Test]
        public void ShouldRandomizeWhenThereAreOneOrMoreChildren()
        {
            // Arrange
            ChildrenRandomizer diceManager = new GameObject().AddComponent<ChildrenRandomizer>();

            GameObject child1 = new GameObject();
            child1.transform.parent = diceManager.transform;
            child1.SetActive(false);
            GameObject child2 = new GameObject();
            child2.transform.parent = diceManager.transform;
            child2.SetActive(false);

            int idx = diceManager.RandomizeChildren();
            Assert.IsTrue(idx != -1);

            if (idx == 0)
            {
                Assert.IsTrue(child1.activeSelf);
                Assert.IsFalse(child2.activeSelf);
            }
            else if (idx == 1)
            {
                Assert.IsFalse(child1.activeSelf);
                Assert.IsTrue(child2.activeSelf);
            }
        }

        [Test]
        public void ShouldNotRandomizeWhenThereAreNoChildren()
        {
            // Arrange
            ChildrenRandomizer diceManager = new GameObject().AddComponent<ChildrenRandomizer>();

            int idx = diceManager.RandomizeChildren();
            Assert.IsTrue(idx == -1);
        }
    }
}
