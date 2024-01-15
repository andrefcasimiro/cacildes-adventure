namespace AF.Animations
{
    public interface IAnimationEventListener
    {

        // Footsteps
        public void OnLeftFootstep();
        public void OnRightFootstep();

        // Combat
        public void OpenLeftWeaponHitbox();
        public void CloseLeftWeaponHitbox();
        public void OpenRightWeaponHitbox();
        public void CloseRightWeaponHitbox();
        public void OpenLeftFootHitbox();
        public void CloseLeftFootHitbox();
        public void OpenRightFootHitbox();
        public void CloseRightFootHitbox();


        // Locomotion
        public void EnableRootMotion();
        public void DisableRootMotion();
        public void DisableRotation();
        public void FaceTarget();

        public void OnSpellCast();
        public void OnFireArrow();

        // SFX
        public void PlayCloth();
    }
}
