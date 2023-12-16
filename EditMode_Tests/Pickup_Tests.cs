using UnityEngine;
using AF;
using AF.Pickups;
using NUnit.Framework;

namespace AF.Tests
{

    public class Pickup_Tests : MonoBehaviour
    {
        Pickup pickup;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new();
            pickup = go.AddComponent<Pickup>();

            pickup.monoBehaviourID = pickup.gameObject.AddComponent<MonoBehaviourID>();

            PickupDatabase pickupDatabase = ScriptableObject.CreateInstance<PickupDatabase>();
            pickup.pickupDatabase = pickupDatabase;

            pickup.onAlreadyPickedUp = new UnityEngine.Events.UnityEvent();
        }

        [Test]
        public void OnEnable_ShouldInvokeOnAlreadyPickedUpEvent_IfPickupDatabaseContainsPickup()
        {

            pickup.pickupDatabase.Clear();
            pickup.pickupDatabase.AddPickup(
                pickup.monoBehaviourID.ID,
                "name"
            );

            bool wasCalled = false;
            pickup.onAlreadyPickedUp.AddListener(() =>
            {
                wasCalled = true;
            });

            pickup.OnEnable();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void OnEnable_ShouldNotInvokeOnAlreadyPickedUpEvent_IfPickupDatabaseDoesNotContainPickup()
        {

            pickup.pickupDatabase.Clear();
            pickup.pickupDatabase.AddPickup(
                "someOtherId",
                "name"
            );

            bool wasCalled = false;
            pickup.onAlreadyPickedUp.AddListener(() =>
            {
                wasCalled = true;
            });

            pickup.OnEnable();

            Assert.IsFalse(wasCalled);
        }
    }

}