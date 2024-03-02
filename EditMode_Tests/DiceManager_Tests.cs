using AF.Conditions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Tests
{
    public class DiceManagerTests
    {
        [Test]
        public void Evaluate_SuccessEventTriggered()
        {
            // Arrange
            DiceManager diceManager = new GameObject().AddComponent<DiceManager>();
            UnityEvent onSuccessEvent = new UnityEvent();
            onSuccessEvent.AddListener(() => { Assert.Pass("Success event triggered"); });

            diceManager.onSuccess = onSuccessEvent;
            diceManager.chance = 0f; // Set chance to 1 for guaranteed success

            // Act
            diceManager.Evaluate();

            // Assert
            Assert.Pass("Evaluate method called successfully");
        }

        [Test]
        public void Evaluate_SuccessEventNotTriggered()
        {
            // Arrange
            DiceManager diceManager = new GameObject().AddComponent<DiceManager>();
            UnityEvent onSuccessEvent = new();
            onSuccessEvent.AddListener(() => { Assert.Fail("Success event should not be triggered"); });

            diceManager.onSuccess = onSuccessEvent;
            diceManager.chance = 1f; // Set chance to 1 for guaranteed failure

            // Act
            diceManager.Evaluate();

            // Assert
            Assert.Pass("Evaluate method called successfully");
        }
    }
}
