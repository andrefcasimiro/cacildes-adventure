using UnityEngine;
using UnityEngine.Pool;

namespace AF
{

    public class CombatNotificationManager : MonoBehaviour
    {
        public Color damage;
        public Color criticalDamage;
        public Color fireDamage;
        public Color frostDamage;

        [SerializeField] private CombatNotificationEntry combatNotificationPrefab;
        private IObjectPool<CombatNotificationEntry> combatNotificationEntryPool;

        private void Awake()
        {
            this.combatNotificationEntryPool = new ObjectPool<CombatNotificationEntry>(Create, OnGet, OnRelease, null, maxSize: 5);
        }

        private CombatNotificationEntry Create()
        {
            CombatNotificationEntry combatNotificationEntry = Instantiate(combatNotificationPrefab, transform.position, Quaternion.identity);
            combatNotificationEntry.SetPool(combatNotificationEntryPool);

            return combatNotificationEntry;
        }

        private void OnGet(CombatNotificationEntry combatNotificationEntry)
        {
            combatNotificationEntry.gameObject.SetActive(true);
        }

        private void OnRelease(CombatNotificationEntry combatNotificationEntry)
        {
            combatNotificationEntry.gameObject.SetActive(false);
        }

        public CombatNotificationEntry GetInstance()
        {
            return combatNotificationEntryPool?.Get();
        }
    }
}
