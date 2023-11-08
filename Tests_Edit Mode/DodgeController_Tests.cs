using UnityEngine;
using AF;
using NUnit.Framework;

public class DodgeController_Tests : MonoBehaviour
{
    DodgeController dodgeController;

    [SetUp]
    public void SetUp()
    {
        dodgeController = new GameObject().AddComponent<DodgeController>();
    }

    [Test]
    public void ShouldBackstepIfNotMoving()
    {
        ThirdPersonController thirdPersonController = dodgeController.gameObject
            .AddComponent<ThirdPersonController>();
        thirdPersonController.skateRotation = false;
        dodgeController.thirdPersonController = thirdPersonController;

        StarterAssetsInputs starterAssetsInputs = dodgeController.gameObject
            .AddComponent<StarterAssetsInputs>();
        starterAssetsInputs.move = Vector2.zero;
        dodgeController.starterAssetsInputs = starterAssetsInputs;

        Assert.IsTrue(dodgeController.ShouldBackstep());
    }

    [Test]
    public void ShouldNotBackstepIfMoving()
    {
        ThirdPersonController thirdPersonController = dodgeController.gameObject
            .AddComponent<ThirdPersonController>();
        thirdPersonController.skateRotation = false;
        dodgeController.thirdPersonController = thirdPersonController;

        StarterAssetsInputs starterAssetsInputs = dodgeController.gameObject
            .AddComponent<StarterAssetsInputs>();
        starterAssetsInputs.move = new Vector2(1f, 0f);
        dodgeController.starterAssetsInputs = starterAssetsInputs;

        Assert.IsFalse(dodgeController.ShouldBackstep());
    }
}
