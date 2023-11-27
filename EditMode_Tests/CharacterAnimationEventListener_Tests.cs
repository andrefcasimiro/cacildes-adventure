using UnityEngine;
using NUnit.Framework;
using AF.Animations;

namespace AF.Tests
{
    public class CharacterAnimationEventListener_Tests : MonoBehaviour
    {
        CharacterAnimationEventListener characterAnimationEventListener;

        [SetUp]
        public void SetUp()
        {
            characterAnimationEventListener = new GameObject().AddComponent<CharacterAnimationEventListener>();
            var characterManager = characterAnimationEventListener.gameObject.AddComponent<CharacterManager>();
            characterAnimationEventListener.characterManager = characterManager;
            characterManager.animator = characterManager.gameObject.AddComponent<Animator>();
        }

        [Test]
        public void ShouldCallOnLeftFootstep()
        {
            characterAnimationEventListener.onLeftFootstep = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onLeftFootstep.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OnLeftFootstep();
            Assert.IsTrue(wasCalled);
        }
        [Test]
        public void ShouldCallOnRightFootstep()
        {
            characterAnimationEventListener.onRightFootstep = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onRightFootstep.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OnRightFootstep();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ShouldCallOpenRightWeaponHitbox()
        {
            characterAnimationEventListener.onRightWeaponHitboxOpen = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onRightWeaponHitboxOpen.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OpenRightWeaponHitbox();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ShouldCallOpenLeftWeaponHitbox()
        {
            characterAnimationEventListener.onLeftWeaponHitboxOpen = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onLeftWeaponHitboxOpen.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OpenLeftWeaponHitbox();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ShouldCallOpenLeftFootHitbox()
        {
            characterAnimationEventListener.onLeftFootHitboxOpen = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onLeftFootHitboxOpen.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OpenLeftFootHitbox();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ShouldCallOpenRightFootHitbox()
        {
            characterAnimationEventListener.onRightFootHitboxOpen = new UnityEngine.Events.UnityEvent();
            bool wasCalled = false;
            characterAnimationEventListener.onRightFootHitboxOpen.AddListener(() => { wasCalled = true; });
            characterAnimationEventListener.OpenRightFootHitbox();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ShouldEnableRootMotion()
        {
            characterAnimationEventListener.characterManager.animator.applyRootMotion = false;
            Assert.IsFalse(characterAnimationEventListener.characterManager.animator.applyRootMotion);
            characterAnimationEventListener.EnableRootMotion();
            Assert.IsTrue(characterAnimationEventListener.characterManager.animator.applyRootMotion);
        }

        [Test]
        public void ShouldDisableRootMotion()
        {
            characterAnimationEventListener.characterManager.animator.applyRootMotion = true;
            Assert.IsTrue(characterAnimationEventListener.characterManager.animator.applyRootMotion);
            characterAnimationEventListener.DisableRootMotion();
            Assert.IsFalse(characterAnimationEventListener.characterManager.animator.applyRootMotion);
        }
    }

}
