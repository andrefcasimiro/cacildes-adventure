using UnityEngine;
using NUnit.Framework;

namespace AF.Tests
{

    public class DodgeController_Tests : MonoBehaviour
    {
        PlayerManager playerManager;
        DodgeController dodgeController;

        [SetUp]
        public void SetUp()
        {
            dodgeController = new GameObject().AddComponent<DodgeController>();
            playerManager = dodgeController.gameObject.AddComponent<PlayerManager>();
            dodgeController.playerManager = playerManager;
        }

        [Test]
        public void ShouldBackstepIfNotMoving()
        {
            ThirdPersonController thirdPersonController = dodgeController.gameObject
                .AddComponent<ThirdPersonController>();
            thirdPersonController.isSliding = false;

            playerManager.thirdPersonController = thirdPersonController;

            StarterAssetsInputs starterAssetsInputs = dodgeController.gameObject
                .AddComponent<StarterAssetsInputs>();
            starterAssetsInputs.move = Vector2.zero;
            playerManager.starterAssetsInputs = starterAssetsInputs;

            Assert.IsTrue(dodgeController.ShouldBackstep());
        }

        [Test]
        public void ShouldNotBackstepIfMoving()
        {
            ThirdPersonController thirdPersonController = dodgeController.gameObject
                .AddComponent<ThirdPersonController>();
            thirdPersonController.isSliding = false;
            playerManager.thirdPersonController = thirdPersonController;

            StarterAssetsInputs starterAssetsInputs = dodgeController.gameObject
                .AddComponent<StarterAssetsInputs>();
            starterAssetsInputs.move = new Vector2(1f, 0f);
            playerManager.starterAssetsInputs = starterAssetsInputs;

            Assert.IsFalse(dodgeController.ShouldBackstep());
        }
    }
}
