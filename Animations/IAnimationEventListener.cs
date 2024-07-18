namespace AF.Animations
{
    public interface IAnimationEventListener
    {

        public void StopIframes();
        // Footsteps
        public void OnLeftFootstep();
        public void OnRightFootstep();
        public void OnSwim();

        // Combat
        public void OpenLeftWeaponHitbox();
        public void CloseLeftWeaponHitbox();
        public void OpenRightWeaponHitbox();
        public void CloseRightWeaponHitbox();
        public void OpenLeftFootHitbox();
        public void CloseLeftFootHitbox();
        public void OpenRightFootHitbox();
        public void CloseRightFootHitbox();
        public void OnBuff();

        // Locomotion
        public void EnableRootMotion();
        public void DisableRootMotion();
        public void EnableRotation();
        public void DisableRotation();
        public void FaceTarget();

        public void OnSpellCast();
        public void OnFireArrow();
        public void OnFireMultipleArrows();
        public void OnThrow();

        // SFX
        public void OnCloth();
        public void OnImpact();

        // FX
        public void OnBlood();

        public void OpenCombo();

        public void OnShakeCamera();

        public void DropIKHelper();
        public void UseIKHelper();
        public void SetCanTakeDamage_False();

        public void OnWeaponSpecial();

        public void MoveTowardsTarget();
        public void StopMoveTowardsTarget();

        public void PauseAnimation();
        public void ResumeAnimation();


    }
}
