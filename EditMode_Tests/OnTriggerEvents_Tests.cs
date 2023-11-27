using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using AF.Triggers;

namespace AF.Tests
{
    public class OnTriggerEventsTests
    {
        private OnTriggerEvents triggerEvents;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject();
            triggerEvents = testObject.AddComponent<OnTriggerEvents>();
            triggerEvents.onTriggerEnterEvent = new UnityEvent();
            triggerEvents.onTriggerStayEvent = new UnityEvent();
            triggerEvents.onTriggerExitEvent = new UnityEvent();
            triggerEvents.tagsToDetect = new string[] { "Player" };
        }

        [Test]
        public void OnTriggerEnter_WithMatchingTag_ShouldInvokeOnTriggerEnterEvent()
        {
            // Arrange
            bool eventInvoked = false;
            triggerEvents.onTriggerEnterEvent.AddListener(() => eventInvoked = true);
            GameObject objectThatCollidesWithTrigger = new GameObject() { tag = "Player" };
            // Act
            triggerEvents.OnTriggerEnter(objectThatCollidesWithTrigger.AddComponent<SphereCollider>());

            // Assert
            Assert.IsTrue(eventInvoked);
        }

        [Test]
        public void OnTriggerEnter_WithNonMatchingTag_ShouldNotInvokeOnTriggerEnterEvent()
        {
            // Arrange
            bool eventInvoked = false;
            triggerEvents.onTriggerEnterEvent.AddListener(() => eventInvoked = true);

            GameObject objectThatCollidesWithTrigger = new GameObject() { tag = "Untagged" };
            // Act
            triggerEvents.OnTriggerEnter(objectThatCollidesWithTrigger.AddComponent<SphereCollider>());


            // Assert
            Assert.IsFalse(eventInvoked);
        }

        [Test]
        public void OnTriggerStay_WithMatchingTagAndCanTriggerOnStay_ShouldInvokeOnTriggerStayEvent()
        {
            // Arrange
            bool eventInvoked = false;
            triggerEvents.onTriggerStayEvent.AddListener(() => eventInvoked = true);
            triggerEvents.CanTriggerOnStay = true;

            // Act
            GameObject objectThatCollidesWithTrigger = new GameObject() { tag = "Player" };
            // Act
            triggerEvents.OnTriggerStay(objectThatCollidesWithTrigger.AddComponent<SphereCollider>());


            // Assert
            Assert.IsTrue(eventInvoked);
        }

        [Test]
        public void OnTriggerStay_WithMatchingTagAndCannotTriggerOnStay_ShouldNotInvokeOnTriggerStayEvent()
        {
            // Arrange
            bool eventInvoked = false;
            triggerEvents.onTriggerStayEvent.AddListener(() => eventInvoked = true);

            // Act
            GameObject objectThatCollidesWithTrigger = new GameObject() { tag = "Player" };

            triggerEvents.CanTriggerOnStay = false;

            // Act
            triggerEvents.OnTriggerStay(objectThatCollidesWithTrigger.AddComponent<SphereCollider>());

            // Assert
            Assert.IsFalse(eventInvoked);
        }
    }

}