using System.Collections;
using System.Linq;
using AF.Characters;
using UnityEngine;

namespace AF
{

    public class EV_UpdateAnimatorClip : EventBase
    {
        public string nameOfAnimationClipToOverride;
        public AnimationClip animationClip;
        public CharacterManager characterManager;

        public override IEnumerator Dispatch()
        {
            characterManager.UpdateAnimatorOverrideControllerClips(nameOfAnimationClipToOverride, animationClip);

            yield return null;
        }
    }

}
