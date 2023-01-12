using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Services.Analytics;

namespace AF
{
    [System.Serializable]
    public class DropCurrency
    {
        public Item item;

        [Range(0, 100)]
        public float chanceToGet;
    }

    [System.Serializable]
    public class Buff
    {
        [Header("Animation That Starts")]
        public string animationStartName;
        public string animationLoopName;
        public string animationEndName;

        public UnityEvent onBuffEventStart;
        public UnityEvent onBuffEventCast;
        public UnityEvent onBuffEventEnd;
        public UnityEvent onBuffInterrupted;

        [Header("Conditions")]
        [Range(0, 100f)]
        public float maximumHealthThresholdBeforeUsingBuff = 100;
        [Range(0, 100f)]
        public float minimumHealthThresholdBeforeUsingBuff = 0;
        public float minimumDistanceToUseBuff = 0f;
        public float maximumDistanceToUseBuff = 15f;

        [Header("Cooldowns")]
        public float currentBuffCooldown = 0f;
        public float maxBuffCooldown = 15f;

        [Header("Options")]
        public bool facePlayer = true;
        public bool stopMovementWhileCasting = true;

        [Header("Pistol Or Bow")]
        public GameObject weaponToShow;
        public float showWeaponAfter;

        [Header("Frequency")]
        [Tooltip("Higher means more often")][Range(0, 100)] public float weight = 30f;
    }


    public enum CombatAction
    {
        Shoot,
        LightAttack,
        HeavyAttack,
        Block,
        UseBuff,
    }

    [System.Serializable]
    public class EnemyAppliedStatus
    {
        public AppliedStatus appliedStatus;

        public EnemyStatusEffectIndicator fullAmountUIIndicator;

        public float maxAmountBeforeDamage;

        public float decreaseRateWithoutDamage = 5;

        public float decreaseRateWithDamage = 1;
    }

    public class EnemyManager : MonoBehaviour, IClockListener
    {
        #region Animation Hashes
        public readonly int hashIdle = Animator.StringToHash("Idle");
        public readonly int hashPatrol = Animator.StringToHash("Patrolling");
        public readonly int hashChasing = Animator.StringToHash("Chasing");

        public readonly int hashIsInCombat = Animator.StringToHash("IsInCombat");

        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashWaiting = Animator.StringToHash("Waiting");
        public readonly int hashIsWaiting = Animator.StringToHash("IsWaiting");

        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        public readonly int hashIsBuffing = Animator.StringToHash("IsBuffing");

        public readonly int hashIsSleeping = Animator.StringToHash("IsSleeping");
        public readonly int hashSleeping = Animator.StringToHash("Sleeping");

        public readonly int hashIsFalling = Animator.StringToHash("IsFalling");
        public readonly int hashFalling = Animator.StringToHash("Falling");

        public readonly int hashPostureHit = Animator.StringToHash("PostureHit");
        public readonly int hashPostureBreak = Animator.StringToHash("PostureBreak");
        public readonly int hashIsStunned = Animator.StringToHash("IsStunned");

        public readonly int hashTurnBehind = Animator.StringToHash("TurnBehind");
        public readonly int hashTurnLeft = Animator.StringToHash("TurnLeft");
        public readonly int hashTurnRight = Animator.StringToHash("TurnRight");

        public readonly int hashShooting = Animator.StringToHash("Shooting");
        public readonly int hashIsShooting = Animator.StringToHash("IsShooting");

        public readonly int hashTakingDamage = Animator.StringToHash("TakingDamage");

        public readonly int hashIsStrafing = Animator.StringToHash("IsStrafing");

        public readonly int hashDying = Animator.StringToHash("Dying");
        public readonly int hashIsDead = Animator.StringToHash("Dead");
        #endregion

        [Header("Components")]
        public Animator animator;
        public NavMeshAgent agent;
        public Rigidbody rigidbody => GetComponent<Rigidbody>();

        [Header("UI Components")]
        public UnityEngine.UI.Slider healthBarSlider;
        public FloatingText damageFloatingText;
        public UnityEngine.UIElements.UIDocument bossHud;
        public UnityEngine.UIElements.IMGUIContainer bossFillBar;

        [Header("Enemy")]
        public Enemy enemy;
        public int currentLevel;

        [Header("Sight")]
        public SightCone sightCone;
        public bool ignorePlayer = false;
        public UnityEvent onPlayerSight;

        [Header("Patrolling")]
        public Transform waypointsParent;
        [HideInInspector] public List<Transform> waypoints = new List<Transform>();
        public float restingTimeOnWaypoint = 2f;
        int destinationPoint = 0;

        [Header("Shooting Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPointRef;
        public float rotationDurationAfterFiringProjectile = 1f;
        [Range(0, 100)] public int shootingWeight = 50;
        public float minimumDistanceToFire = 4f;
        public float maxProjectileCooldown = 10f;
        private float projectileCooldown = 0f;
        public GameObject bowGraphic;
        public bool isReadyToShoot = false;

        [Header("Weapon Hitboxes")]
        public EnemyWeaponHitbox leftHandWeapon;
        public EnemyWeaponHitbox rightHandWeapon;
        public EnemyWeaponHitbox leftLegWeapon;
        public EnemyWeaponHitbox rightLegWeapon;
        public EnemyWeaponHitbox headWeapon;
        public EnemyWeaponHitbox areaOfImpactWeapon;

        [Header("Area Of Impact FX")]
        public GameObject areaOfImpactFX;
        public Transform areaOfImpactTransform;

        [Header("Stats affected by animation clips")]
        public int weaponDamage = 100;
        public StatusEffect weaponStatusEffect = null;
        public float statusEffectAmount = 0f;
        public float bonusBlockStaminaCost = 0f;
        public int currentPoiseDamage = 1;

        [Header("Light Attacks")]
        [Range(0, 100)] public int lightAttackWeight = 75;

        [Header("Heavy Attacks")]
        [Range(0, 100)] public int heavyAttackWeight = 25;

        [Header("Dodge Settings")]
        [Range(0, 100)][Tooltip("0 means never, 100 means always")] public int dodgeWeight = 100;
        public float maxDodgeCooldown = 10f;
        [HideInInspector] public float dodgeCooldown = Mathf.Infinity; // Used by the EnemyStateWaiting to control the dodge frequency
        public bool dodgeLeftOrRight = false;
        public string[] customDodgeClips;

        [Header("Waiting Settings")]
        [Tooltip("The higher, the more idle wait time is chanced out in combat")] public int passivityWeight = 1;
        public float minWaitingTimeBeforeResumingCombat = 0.25f;
        public float maxWaitingTimeBeforeResumingCombat = 2f;
        [HideInInspector] public float turnWaitingTime = 0f;
        protected float waitingCounter = 0f;

        [Header("Circle Around Settings")]
        public bool canCircleAround = false;
        [Range(0, 100)] public float circleAroundWeight = 75;
        public string circleAroundRightAnimation = "Strafe Right";
        public string circleAroundLeftAnimation = "Strafe Left";

        [Header("Health")]
        public float currentHealth;
        private EnemyHealthHitbox[] enemyHealthHitboxes;

        [Header("On Killing Events")]
        public UnityEvent onEnemyDeath;

        [Header("Combat Flow - Action Sequences")]
        public CombatAction[] respondToPlayer;
        public CombatAction[] attackPlayer;

        [Header("Block")]
        public DestroyableParticle blockParticleEffect;
        public EnemyShieldCollider shield;
        [Range(0, 100)] public int blockWeight = 0;
        public bool isShieldAlwaysVisible = false;

        [Header("Posture")]
        public float currentPostureDamage;
        public UnityEngine.UI.Slider postureBarSlider;
        public GameObject stunnedParticle;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;

        [Header("Loot")]
        public List<DropCurrency> lootTable = new List<DropCurrency>();
        public bool overrideLootTable = false;

        [Header("Sleep")]
        public bool canSleep = false;
        public float sleepFrom = 22f;
        public float sleepUntil = 05f;
        public bool isSleeping = false;
        public GameObject bed;

        [Header("Buffs")]
        public Buff[] buffs;
        [HideInInspector] public List<Buff> usedBuffs = new List<Buff>();
        public AudioClip enemyBuffSfx;

        [Header("Negative Status")]
        [SerializeField] List<EnemyAppliedStatus> appliedStatus = new List<EnemyAppliedStatus>();
        public EnemyStatusEffectIndicator uiStatusPrefab;
        public GameObject statusEffectContainer;
        public FloatingText statusFloatingText;

        [Header("Elemental Damage")]
        public GameObject fireFx;
        public Color fireTextColor;
        public GameObject frostFx;
        public Color frostTextColor;
        public FloatingText elementalDamageFloatingText;

        [Header("Poise")]
        int currentPoiseHitCount = 0;
        float resetPoiseTimer;
        float cooldownBeforeTakingAnotherHitToPoise = Mathf.Infinity;

        [Header("Boss")]
        public bool isBoss = false;
        public string bossName = "";
        public GameObject fogWall;
        public AudioClip bossMusic;
        public string bossSwitchUuid;
        public bool updateBossSwitchWithRefresh = false;

        [Header("Character Rotations")]
        public float rotationSpeed = 5f;

        [Header("Gravity")]
        public bool canFall = true;


        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        // Flags
        [HideInInspector] public bool facePlayer = false;

        // Internal Components
        [HideInInspector] public GameObject player;
        NotificationManager notificationManager;
        PlayerInventory playerInventory;
        [HideInInspector] public PlayerComponentManager playerComponentManager;
        [HideInInspector] public PlayerCombatController playerCombatController;
        ClimbController playerClimbController;
        PlayerLevelManager playerLevelManager;

        LockOnManager lockOnManager;
        LockOnRef lockOnRef;

        Vector3 initialPosition; // For bonfire respawns

        // When a companion attacks, focus on a companion for a short period of time
        [Header("Companion AI")]
        public bool ignoreCompanions = false;
        [Range(0, 100)] public float focusOnCompanionWeight = 50;
        public CompanionManager currentCompanion;
        public float maxTimeFocusedOnCompanions = 10f;
        float focusedOnCompanionsTimer = Mathf.Infinity;

        [Header("Analytics")]
        public bool trackEnemyKill = false;

        private void Start()
        {

            // Component initialization
            notificationManager = FindObjectOfType<NotificationManager>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            player = playerInventory.gameObject;
            playerClimbController = player.GetComponent<ClimbController>();
            playerCombatController = player.GetComponent<PlayerCombatController>();
            playerComponentManager = player.GetComponent<PlayerComponentManager>();
            playerLevelManager = player.GetComponent<PlayerLevelManager>();

            lockOnManager = FindObjectOfType<LockOnManager>(true);
            lockOnRef = GetComponentInChildren<LockOnRef>(true);
            if (lockOnRef == null)
            {
                Debug.LogError("Could not find Lock On Ref on " + gameObject.name + " children. Please add one");
            }

            enemyHealthHitboxes = GetComponentsInChildren<EnemyHealthHitbox>(true);
            if (enemyHealthHitboxes == null || enemyHealthHitboxes.Length <= 0)
            {
                Debug.LogError("Could not find any enemy health hitboxes on " + gameObject.name + " children. Please add one");
            }


            initialPosition = transform.position;


            RestoreHealth();

            InitializeEnemyHUD();
            InitializeBossHUD();
            InitializeShield();
            InitializeSleep();
            InitializePostureHUD();

            HideBow();
            ShowWeapons();

            DisableAllWeaponHitboxes();


            InitializeWaypoints();

            if (fogWall != null)
            {
                fogWall.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (facePlayer)
            {
                var target = player.transform;

                if (ignoreCompanions == false && currentCompanion != null)
                {
                    target = currentCompanion.transform;
                }

                var lookRotation = target.position - this.transform.position;
                lookRotation.y = 0;
                var rotation = Quaternion.LookRotation(lookRotation);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }

            UpdateSleep();
            UpdateBuffs();
            UpdateWaiting();
            UpdatePosture();
            UpdateNegativeStatus();
            UpdatePoise();
            UpdateProjectile();
            UpdateDodgeCounter();
            UpdateHealthSlider();
            UpdateCompanionsFocusAI();
        }

        #region Patrolling

        void InitializeWaypoints()
        {
            if (waypointsParent != null)
            {
                foreach (Transform waypointChild in waypointsParent.transform)
                {
                    this.waypoints.Add(waypointChild);
                }
            }
        }

        public void GotoNextPoint()
        {
            if (waypoints.Count <= 0)
            {
                return;
            }

            agent.destination = waypoints[destinationPoint].position;

            destinationPoint = (destinationPoint + 1) % waypoints.Count;
        }

        #endregion

        #region Sight

        public bool IsPlayerInSight()
        {
            if (ignoreCompanions == false && currentCompanion != null)
            {
                return false;
            }

            if (playerComponentManager.IsBusy())
            {
                return false;
            }

            if (ignorePlayer)
            {
                return false;
            }

            if (currentHealth <= 0)
            {
                return false;
            }

            if (isSleeping)
            {
                return false;
            }

            if (Vector3.Distance(player.transform.position, this.transform.position) <= agent.stoppingDistance)
            {
                return true;
            }

            if (Vector3.Distance(this.transform.position, player.transform.position) > maximumChaseDistance)
            {
                return false;
            }

            Vector3 enemyEyes = sightCone.transform.position;
            Vector3 playerEyes = playerClimbController.playerHeadRef.transform.position;

            if (sightCone.playerWithinRange)
            {
                Debug.DrawLine(enemyEyes, playerEyes, Color.blue);

                RaycastHit hitInfo;

                if (Physics.Linecast(enemyEyes, playerEyes, out hitInfo))
                {
                    if (hitInfo.collider.gameObject.tag == "Player" || hitInfo.collider.gameObject.tag == "PlayerHealthbox")
                    {
                        if (onPlayerSight != null)
                        {
                            onPlayerSight.Invoke();
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Combat
        public bool IsInCombat()
        {
            return animator.GetBool(hashIsInCombat);
        }

        public int GetCurrentAttack()
        {
            return Player.instance.CalculateAIAttack(enemy.basePhysicalAttack, this.currentLevel) + weaponDamage;
        }

        public void ForceIntoCombat() {
            if (currentHealth <= 0)
            {
                return;
            }

            animator.Play(hashChasing);
        }
        #endregion

        #region Loot
        public void GetLoot()
        {
            var itemsToReceive = new List<Item>();

            bool hasPlayedFanfare = false;

            var finalLootTable = overrideLootTable ? lootTable : enemy.lootTable;

            if (overrideLootTable == false && lootTable.Count > 0)
            {
                foreach (var dropC in lootTable)
                {
                    finalLootTable.Add(dropC);
                }
            }

            foreach (DropCurrency dropCurrency in finalLootTable)
            {
                float calc_dropChance = Random.Range(0, 100f);

                if (calc_dropChance <= dropCurrency.chanceToGet)
                {
                    if (hasPlayedFanfare == false)
                    {

                        BGMManager.instance.PlayItem();
                        hasPlayedFanfare = true;
                    }

                    itemsToReceive.Add(dropCurrency.item);
                }
            }

            UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt = null;
            if (isBoss)
            {
                uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);

                if (uIDocumentReceivedItemPrompt != null)
                {
                    uIDocumentReceivedItemPrompt.itemsUI.Clear();
                }
            }

            foreach (var item in itemsToReceive)
            {
                playerInventory.AddItem(item, 1);

                if (isBoss && uIDocumentReceivedItemPrompt != null)
                {
                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name;
                    itemReceived.quantity = 1;
                    itemReceived.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);
                }
                else
                {
                    notificationManager.ShowNotification("Found " + item.name, item.sprite);
                }
            }

            if (isBoss && itemsToReceive.Count > 0)
            {
                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
            }
        }

        IEnumerator GiveLoot()
        {
            yield return new WaitForSeconds(1f);

            var goldToReceive = Player.instance.CalculateAIGenericValue(enemy.baseGold, playerLevelManager.GetCurrentLevel());

            var equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            if (equipmentGraphicsHandler != null)
            {
                var additionalCoinPercentage = equipmentGraphicsHandler.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = 0;

                    additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }
            }

            FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(goldToReceive);
            Player.instance.currentGold += (int)goldToReceive;

            GetLoot();
        }
        #endregion

        #region Health

        void RestoreHealth()
        {
            this.currentHealth = GetMaxHealth();
        }

        void UpdateHealthSlider()
        {
            if (healthBarSlider != null && isBoss == false)
            {
                healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
            else if (isBoss)
            {
                var percentage = currentHealth * 100 / GetMaxHealth();
                bossFillBar.style.width = new Length(percentage, LengthUnit.Percent);
            }
        }

        int GetMaxHealth()
        {
            return Player.instance.CalculateAIHealth(enemy.baseHealth, this.currentLevel);
        }

        void InitializeEnemyHUD()
        {
            if (healthBarSlider != null && isBoss == false)
            {
                healthBarSlider.gameObject.SetActive(true);
                healthBarSlider.maxValue = GetMaxHealth() * 0.01f;
                healthBarSlider.value = currentHealth * 0.01f;
            }
        }
        #endregion

        #region Taking Damage

        public void TakeEnvironmentalDamage(float damage)
        {
            if (this.currentHealth <= 0)
            {
                return;
            }

            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, GetMaxHealth());


            if (damageFloatingText != null)
            {
                damageFloatingText.Show(damage);
            }

            if (enemy.damagedParticle != null)
            {
                Instantiate(enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            if (this.currentHealth <= 0)
            {
                Die();
            }
        }

        public void TakeDamage(AttackStatManager playerAttackStatManager, Weapon weapon, Transform attackerTransform, AudioClip weaponHitSfx, float hitboxDamageBonus)
        {
            if (this.currentHealth <= 0)
            {
                return;
            }

            if (IsBlocking())
            {
                return;
            }

            if (isSleeping)
            {
                WakeUp();
            }

            if (usedBuffs.Count > 0)
            {
                foreach (var usedBuff in usedBuffs)
                {
                    usedBuff.onBuffInterrupted.Invoke();
                }
            }

            if (isBoss == false)
            {
                BGMManager.instance.PlayBattleMusic();
            }

            BGMManager.instance.PlaySoundWithPitchVariation(
                weaponHitSfx,
                combatAudioSource
                );

            DisableAllWeaponHitboxes();

            float appliedDamage = weapon != null ? playerAttackStatManager.GetWeaponAttack(weapon) : playerAttackStatManager.GetCurrentPhysicalAttack();
            appliedDamage += hitboxDamageBonus;

            if (IsStunned())
            {
                appliedDamage *= enemy.brokenPostureDamageMultiplier;
            }

            // Disable stunned stars on hit
            stunnedParticle.gameObject.SetActive(false);

            // Elemental Damage Bonus
            if (weapon != null)
            {
                if (weapon.fireAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.fireAttack + enemy.fireDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    OnFireDamage(elementalBonus);
                }
                if (weapon.frostAttack > 0)
                {
                    var elementalBonus = Mathf.Clamp(weapon.frostAttack + enemy.frostDamageBonus, 0, Mathf.Infinity);
                    appliedDamage += elementalBonus;

                    OnFrostDamage(elementalBonus);
                }
            }

            this.currentHealth = Mathf.Clamp(currentHealth - appliedDamage, 0f, GetMaxHealth());

            if (enemy.damagedParticle != null)
            {
                Instantiate(enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            // Status Effects
            var bonusDamageToShow = 0f;
            if (weapon != null && weapon.statusEffects != null && weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffectPerHit in weapon.statusEffects)
                {
                    var storedResult = InflictStatusEffect(weaponStatusEffectPerHit.statusEffect, weaponStatusEffectPerHit.amountPerHit);
                    if (storedResult > 0)
                    {
                        bonusDamageToShow += storedResult;
                    }
                }
            }

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(appliedDamage + bonusDamageToShow);
            }


            if (this.currentHealth <= 0)
            {
                Die();
            }
            else
            {
                IncreasePoiseDamage(weapon != null ? weapon.poiseDamageBonus : 1);
            }
        }

        public void TakeProjectileDamage(float damage, Projectile projectile)
        {
            Destroy(projectile.gameObject);

            if (IsBlocking())
            {
                return;
            }

            if (this.currentHealth <= 0)
            {
                return;
            }

            if (isSleeping)
            {
                WakeUp();
            }

            DisableAllWeaponHitboxes();

            float physicalDamage = damage;

            if (IsStunned())
            {
                damage *= enemy.brokenPostureDamageMultiplier;
            }

            // Disable stunned stars on hit
            stunnedParticle.gameObject.SetActive(false);


            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0f, GetMaxHealth());

            if (enemy.damagedParticle != null)
            {
                Instantiate(enemy.damagedParticle, transform.position, Quaternion.identity);
            }

            // Status Effects
            var bonusDamageToShow = 0f;

            if (damageFloatingText != null)
            {
                damageFloatingText.Show(physicalDamage + bonusDamageToShow);
            }


            if (this.currentHealth <= 0)
            {
                Die();
            }
            else
            {
                IncreasePoiseDamage(1);
            }

            // Not in combat
            if (IsInCombat() == false)
            {
                animator.SetBool(hashChasing, true);
            }
        }


        #endregion

        #region Dying

        public void Die()
        {
            StartCoroutine(GiveLoot());

            StartCoroutine(PlayDeathSfx());

            animator.SetTrigger(hashDying);

            if (fogWall != null)
            {
                fogWall.gameObject.SetActive(false);
            }

            if (isBoss)
            {
                HideBossHud();

                if (updateBossSwitchWithRefresh == false)
                {
                    SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(bossSwitchUuid, true);
                }
            }

            if (trackEnemyKill)
            {
                    var playerActiveBuffs = "";
                    foreach (var activeBuff in Player.instance.appliedConsumables)
                    {
                        if (playerActiveBuffs != "")
                        {
                            playerActiveBuffs += ", ";
                        }

                        playerActiveBuffs += activeBuff.consumableEffect.displayName;
                    }

                    AnalyticsService.Instance.CustomData(isBoss ? "boss_killed" : "enemy_killed",
                            new Dictionary<string, object>()
                            {
                                    { isBoss ? "boss" : "enemy", isBoss ? bossName : enemy.name + " (Lv. " + currentLevel + ") on map " + SceneManager.GetActiveScene().name },
                                    { "player_level", FindObjectOfType<PlayerLevelManager>(true).GetCurrentLevel() },
                                    { "player_vitality", Player.instance.vitality },
                                    { "player_endurance", Player.instance.endurance },
                                    { "player_strength", Player.instance.strength },
                                    { "player_dexterity", Player.instance.dexterity },
                                    { "player_weapon", Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.name : "Unarmed" },
                                    { "player_shield", Player.instance.equippedShield != null ? Player.instance.equippedShield.name : "-" },
                                    { "player_helmet", Player.instance.equippedHelmet != null ? Player.instance.equippedHelmet.name : "-" },
                                    { "player_armor", Player.instance.equippedArmor != null ? Player.instance.equippedArmor.name : "-" },
                                    { "player_gauntlets", Player.instance.equippedGauntlets != null ? Player.instance.equippedGauntlets.name : "-" },
                                    { "player_legwear", Player.instance.equippedLegwear != null ? Player.instance.equippedLegwear.name : "-" },
                                    { "player_acessory", Player.instance.equippedAccessory != null ? Player.instance.equippedAccessory.name : "-" },
                                    { "player_buffs", playerActiveBuffs },
                            }
                        );
                }

            // Notify companions that were attacking this enemy

            // Notify active player companions
            CompanionManager[] companionManagers = FindObjectsOfType<CompanionManager>();
            foreach (var companion in companionManagers)
            {
                if (companion.currentEnemy == this)
                {
                    companion.StopCombat();
                }
            }

            DisableAllWeaponHitboxes();

            ClearAllNegativeStatus();

            StartCoroutine(DisengageLockOn());
        }

        protected IEnumerator PlayDeathSfx()
        {
            yield return new WaitForSeconds(0.1f);

            BGMManager.instance.PlaySoundWithPitchVariation(enemy.isMale ? enemy.deathSfx : enemy.femaleDeathSfx, combatAudioSource);

            yield return new WaitForSeconds(1f);

            healthBarSlider.gameObject.SetActive(false);

            postureBarSlider.gameObject.SetActive(false);
            stunnedParticle.gameObject.SetActive(false);
            
            BGMManager.instance.PlayMapMusicAfterKillingEnemy(this);
        }


        IEnumerator DisengageLockOn()
        {
            yield return new WaitForSeconds(1f);

            if (lockOnManager.isLockedOn
                && lockOnRef != null
                && lockOnRef.enabled
                && lockOnManager.nearestLockOnTarget != null
                && lockOnManager.nearestLockOnTarget == lockOnRef)
            {

                if (lockOnManager.rightLockTarget != null && lockOnManager.rightLockTarget != lockOnRef)
                {
                    lockOnManager.SwitchToRightTarget();
                }
                else if (lockOnManager.leftLockTarget != null && lockOnManager.leftLockTarget != lockOnRef)
                {
                    lockOnManager.SwitchToLeftTarget();
                }
                else
                {
                    lockOnManager.DisableLockOn();
                }
            }

            if (isBoss && updateBossSwitchWithRefresh)
            {
                // Update switch with refresh must be at the very last part of all the die flow
                SwitchManager.instance.UpdateSwitch(bossSwitchUuid, true);
            }
        }

        #endregion

        #region Revive


        public void Revive()
        {
            animator.SetBool(hashDying, false);

            this.currentHealth = GetMaxHealth();

            this.transform.position = initialPosition;
            agent.nextPosition = initialPosition;

            var deathCollider = GetComponentInChildren<DeathColliderRef>(true);
            if (deathCollider != null)
            {
                deathCollider.GetComponent<BoxCollider>().enabled = false;
            }

            rigidbody.isKinematic = false;
            GetComponent<CapsuleCollider>().enabled = true;

            agent.enabled = true;

            animator.Play(hashIdle);

            // Remove all negative status
            ClearAllNegativeStatus();

            currentPostureDamage = 0;

            // Clear damage text
            var damageTexts = GetComponentsInChildren<FloatingText>(true);
            foreach (var damageText in damageTexts)
            {
                damageText.gameObject.SetActive(true);
                damageText.Reset();
            }

            // Enable health hitboxes
            EnableHealthHitboxes();

            InitializeEnemyHUD();
            InitializePostureHUD();

        }
        #endregion

        #region Health Hitboxes
        public void EnableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.gameObject.SetActive(true);
            }
        }
        public void DisableHealthHitboxes()
        {
            foreach (var enemyHealthHitbox in enemyHealthHitboxes)
            {
                enemyHealthHitbox.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Boss
        public void InitiateBossBattle()
        {
            ShowBossHud();

            if (fogWall != null)
            {
                fogWall.gameObject.SetActive(true);
            }
        }

        void InitializeBossHUD()
        {
            if (isBoss)
            {
                bossHud = GetComponent<UIDocument>();
                bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;

                HideBossHud();
            }
        }

        public void ShowBossHud()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(false);
            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void HideBossHud()
        {
            healthBarSlider.transform.GetChild(0).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(1).gameObject.SetActive(false);
            healthBarSlider.transform.GetChild(2).gameObject.SetActive(false);
            bossHud.rootVisualElement.style.display = DisplayStyle.None;
        }
        #endregion

        #region Shield
        void InitializeShield()
        {
            if (shield != null)
            {
                shield.gameObject.SetActive(isShieldAlwaysVisible);
            }
        }


        public bool IsBlocking()
        {
            return animator.GetBool(hashIsBlocking);
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void ActivateBlock()
        {
            if (shield != null && !isShieldAlwaysVisible)
            {
                shield.gameObject.SetActive(true);
            }

            DisableHealthHitboxes();
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void DeactivateBlock()
        {
            if (shield != null && !isShieldAlwaysVisible)
            {
                shield.gameObject.SetActive(false);
            }

            EnableHealthHitboxes();
        }

        #endregion

        #region Sleep
        void InitializeSleep()
        {
            if (bed != null) { bed.gameObject.SetActive(false); }
        }

        void UpdateSleep()
        {
            if (isSleeping)
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) <= 1f)
                {
                    isSleeping = false;
                    WakeUp();
                }
            }
        }

        public void OnHourChanged()
        {
            bool shouldSleep = false;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (sleepFrom > sleepUntil)
            {
                shouldSleep = Player.instance.timeOfDay >= sleepFrom && Player.instance.timeOfDay <= 24 || (Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay <= sleepUntil);
            }
            else
            {
                shouldSleep = Player.instance.timeOfDay >= sleepFrom && Player.instance.timeOfDay <= sleepUntil;
            }

            if (shouldSleep)
            {
                if (IsInCombat() == false)
                {
                    // Sleep
                    Sleep();
                }
            }
            else
            {
                if (isSleeping)
                {
                    WakeUp();
                }
            }
        }


        public void Sleep()
        {
            isSleeping = true;
            animator.Play(hashSleeping);
            agent.isStopped = true;

            if (bed != null) { bed.gameObject.SetActive(true); }
        }

        public void WakeUp()
        {
            isSleeping = false;
            animator.SetBool(hashIsSleeping, false);
            agent.isStopped = false;
            if (bed != null) { bed.gameObject.SetActive(false); }
        }

        #endregion

        #region Buffs

        public void PlayEnemyBuffSfx()
        {
            BGMManager.instance.PlaySound(enemyBuffSfx, combatAudioSource);
        }

        void UpdateBuffs()
        {
            if (usedBuffs.Count <= 0)
            {
                return;
            }

            var copiedUsedBuffs = usedBuffs.ToArray();
            foreach (var usedBuff in copiedUsedBuffs)
            {
                usedBuff.currentBuffCooldown += Time.deltaTime;

                if (usedBuff.currentBuffCooldown >= usedBuff.maxBuffCooldown)
                {
                    usedBuffs.Remove(usedBuff);
                }
            }
        }

        public bool CanUseBuff(Buff buff)
        {
            if (IsUsingBuff())
            {
                return false;
            }

            float currentHealthPercentage = currentHealth * 100 / GetMaxHealth();

            if (currentHealthPercentage <= 0)
            {
                return false;
            }

            if (usedBuffs.Contains(buff))
            {
                return false;
            }

            if (currentHealthPercentage < buff.minimumHealthThresholdBeforeUsingBuff || currentHealthPercentage > buff.maximumHealthThresholdBeforeUsingBuff)
            {
                return false;
            }

            var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer > buff.maximumDistanceToUseBuff || distanceToPlayer < buff.minimumDistanceToUseBuff + GetComponent<NavMeshAgent>().stoppingDistance)
            {
                return false;
            }

            return true;
        }

        public bool IsUsingBuff()
        {
            return animator.GetBool(hashIsBuffing);
        }

        public void PrepareBuff(Buff buff)
        {
            buff.currentBuffCooldown = 0;
            this.usedBuffs.Add(buff);

            if (buff.facePlayer)
            {
                facePlayer = true;
            }

            if (buff.stopMovementWhileCasting)
            {
                agent.isStopped = true;
            }

            animator.Play(buff.animationStartName);

            buff.onBuffEventStart.Invoke();

            if (buff.weaponToShow != null)
            {
                StartCoroutine(ShowWeaponAfter(buff));
            }
        }

        IEnumerator ShowWeaponAfter(Buff buff)
        {
            yield return new WaitForSeconds(buff.showWeaponAfter);

            buff.weaponToShow.gameObject.SetActive(true);
        }

        // Animation Event
        public void OnBuffStart()
        {
            if (buffs.Length <= 0)
            {
                return;
            }

            var target = buffs.FirstOrDefault(x => 
                animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationStartName) 
                || animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationLoopName) 
                || animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationEndName));

            if (target == null)
            {
                return;
            }

            target.onBuffEventCast.Invoke();
        }

        public void OnBuffEnd()
        {
            if (buffs.Length <= 0)
            {
                return;
            }

            var target = buffs.FirstOrDefault(x => 
                animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationStartName) 
                || animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationLoopName) 
                || animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationEndName));

            if (target == null)
            {
                return;
            }

            target.onBuffEventEnd.Invoke();

            if (target.facePlayer)
            {
                facePlayer = false;
            }
        }
        #endregion

        #region Waiting
        void UpdateWaiting()
        {
            if (IsWaiting())
            {
                waitingCounter += Time.deltaTime;

                if (waitingCounter >= turnWaitingTime)
                {
                    waitingCounter = 0f;
                    turnWaitingTime = 0f;

                    animator.SetBool(hashIsWaiting, false);
                }
            }
        }

        public bool IsWaiting()
        {
            return animator.GetBool(hashIsWaiting);
        }
        #endregion

        #region Weapon Hitboxes

        public void DisableAllWeaponHitboxes()
        {
            DeactivateLeftHandHitbox();
            DeactivateRightHandHitbox();
            DeactivateLeftLegHitbox();
            DeactivateRightLegHitbox();
            DeactivateAreaOfImpactHitbox();
            DeactivateHeadHitbox();
        }
        public void ActivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.EnableHitbox();
        }
        public void DeactivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.DisableHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.EnableHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.DisableHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.EnableHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.DisableHitbox();
        }

        public void ActivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.EnableHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.DisableHitbox();
        }

        public void ActivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.EnableHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.DisableHitbox();
        }

        public void ActivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            if (areaOfImpactFX != null)
            {
                Instantiate(areaOfImpactFX, areaOfImpactTransform);
            }

            areaOfImpactWeapon.EnableHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            areaOfImpactWeapon.DisableHitbox();
        }
        #endregion

        #region Posture
        void InitializePostureHUD()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = GetMaxPostureDamage() * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(false);
            }

        }

        void UpdatePosture()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(currentPostureDamage > 0);
            }

            if (currentPostureDamage > 0)
            {
                currentPostureDamage -= Time.deltaTime * enemy.postureDecreaseRate;
            }
        }

        public void TakePostureDamage()
        {
            DisableAllWeaponHitboxes();

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + enemy.postureDamagePerParry, 0, GetMaxPostureDamage());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                BreakPosture();
            }
            else
            {
                animator.CrossFade(hashPostureHit, 0.05f);
            }
        }

        void BreakPosture()
        {
            currentPostureDamage = 0f;

            stunnedParticle.gameObject.SetActive(true);
            animator.CrossFade(hashPostureBreak, 0.05f);
        }

        public void RecoverPosture()
        {
            stunnedParticle.gameObject.SetActive(false);
        }

        public bool IsStunned()
        {
            return animator.GetBool(hashIsStunned);
        }

        int GetMaxPostureDamage()
        {
            return Player.instance.CalculateAIPosture(enemy.maxPostureDamage, currentLevel);

        }

        #endregion

        #region Elemental Damage
        public void OnFireDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = fireTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(fireFx, this.transform.position, Quaternion.identity);
        }

        public void OnFrostDamage(float appliedAmount)
        {
            elementalDamageFloatingText.gameObject.SetActive(true);
            elementalDamageFloatingText.GetComponent<TMPro.TextMeshPro>().color = frostTextColor;
            elementalDamageFloatingText.Show(appliedAmount);

            Instantiate(frostFx, this.transform.position, Quaternion.identity);
        }
        #endregion

        #region Negative Status
        void UpdateNegativeStatus()
        {
            if (appliedStatus.Count > 0)
            {
                HandleStatusEffects();
            }
        }

        public float InflictStatusEffect(StatusEffect statusEffect, float amount)
        {
            float valueToReturn = 0f;

            var idx = this.appliedStatus.FindIndex(x => x.appliedStatus.statusEffect == statusEffect);

            if (idx != -1)
            {
                if (this.appliedStatus[idx].appliedStatus.hasReachedTotalAmount)
                {
                    return valueToReturn;
                }

                this.appliedStatus[idx].appliedStatus.currentAmount += amount;

                if (this.appliedStatus[idx].appliedStatus.currentAmount >= this.appliedStatus[idx].maxAmountBeforeDamage)
                {
                    ShowTextAndParticleOnReachingFullAmount(this.appliedStatus[idx].appliedStatus.statusEffect);

                    this.appliedStatus[idx].appliedStatus.hasReachedTotalAmount = true;


                    if (statusEffect.damagePercentualValue > 0)
                    {
                        var percentageOfHealthToTake = statusEffect.damagePercentualValue * GetMaxHealth() / 100;
                        valueToReturn = percentageOfHealthToTake;
                    }

                    return valueToReturn;
                }

                return valueToReturn;
            }

            var negativeStatusResistance = enemy.negativeStatusResistances.FirstOrDefault(x => x.statusEffect == statusEffect);
            var maxAmountBeforeDamage = 100f;
            if (negativeStatusResistance != null)
            {
                maxAmountBeforeDamage = negativeStatusResistance.resistance;
            }

            AppliedStatus appliedStatus = new AppliedStatus();
            appliedStatus.statusEffect = statusEffect;
            appliedStatus.currentAmount = amount;
            appliedStatus.hasReachedTotalAmount = amount >= maxAmountBeforeDamage;

            if (appliedStatus.hasReachedTotalAmount)
            {
                ShowTextAndParticleOnReachingFullAmount(appliedStatus.statusEffect);

                if (appliedStatus.statusEffect.damagePercentualValue > 0)
                {
                    var percentageOfHealthToTake = appliedStatus.statusEffect.damagePercentualValue * GetMaxHealth() / 100;
                    valueToReturn = percentageOfHealthToTake;
                }
            }

            EnemyAppliedStatus enemyAppliedStatus = new EnemyAppliedStatus();
            enemyAppliedStatus.appliedStatus = appliedStatus;
            enemyAppliedStatus.maxAmountBeforeDamage = maxAmountBeforeDamage;
            enemyAppliedStatus.decreaseRateWithDamage = negativeStatusResistance.decreaseRateWithDamage;
            enemyAppliedStatus.decreaseRateWithoutDamage = negativeStatusResistance.decreaseRateWithoutDamage;

            var uiIndicatorInstance = Instantiate(uiStatusPrefab, statusEffectContainer.transform);
            enemyAppliedStatus.fullAmountUIIndicator = uiIndicatorInstance;
            uiIndicatorInstance.background.sprite = enemyAppliedStatus.appliedStatus.statusEffect.spriteIndicator;
            uiIndicatorInstance.fill.sprite = enemyAppliedStatus.appliedStatus.statusEffect.spriteIndicator;

            this.appliedStatus.Add(enemyAppliedStatus);

            return valueToReturn;
        }

        void ShowTextAndParticleOnReachingFullAmount(StatusEffect statusEffect)
        {
            if (statusFloatingText != null)
            {
                statusFloatingText.gameObject.SetActive(true);
                statusFloatingText.GetComponent<TMPro.TextMeshPro>().color = statusEffect.barColor;

                statusFloatingText.ShowText(statusEffect.appliedStatusDisplayName);
            }

            Instantiate(statusEffect.particleOnDamage, this.transform);
        }

        private void HandleStatusEffects()
        {
            List<EnemyAppliedStatus> statusToDelete = new List<EnemyAppliedStatus>();

            foreach (var entry in this.appliedStatus)
            {
                entry.appliedStatus.currentAmount -= (entry.appliedStatus.hasReachedTotalAmount
                    ? entry.decreaseRateWithDamage
                    : entry.decreaseRateWithoutDamage) * Time.deltaTime;

                float uiValue = entry.appliedStatus.currentAmount / entry.maxAmountBeforeDamage;
                entry.fullAmountUIIndicator.UpdateUI(uiValue, entry.appliedStatus.hasReachedTotalAmount);

                if (entry.appliedStatus.hasReachedTotalAmount)
                {
                    EvaluateEffect(entry, statusToDelete);
                }

                if (entry.appliedStatus.currentAmount <= 0 || entry.appliedStatus.hasReachedTotalAmount && entry.appliedStatus.statusEffect.effectIsImmediate)
                {
                    statusToDelete.Add(entry);
                }
            }

            foreach (var status in statusToDelete)
            {
                RemoveAppliedStatus(status);
            }

        }

        public void RemoveAppliedStatus(EnemyAppliedStatus appliedStatus)
        {
            Destroy(appliedStatus.fullAmountUIIndicator.gameObject);

            this.appliedStatus.Remove(appliedStatus);
        }

        public void ClearAllNegativeStatus()
        {
            EnemyAppliedStatus[] negativeStatusArray = new EnemyAppliedStatus[this.appliedStatus.Count];
            this.appliedStatus.CopyTo(negativeStatusArray);

            foreach (var _negativeStatus in negativeStatusArray)
            {
                RemoveAppliedStatus(_negativeStatus);
            }
        }

        void EvaluateEffect(EnemyAppliedStatus enemyAppliedStatus, List<EnemyAppliedStatus> statusToDelete)
        {
            var entry = enemyAppliedStatus.appliedStatus;

            if (entry.statusEffect.statAffected == Stat.Health)
            {
                if (entry.statusEffect.effectIsImmediate)
                {
                    if (entry.statusEffect.usePercentuagelDamage)
                    {
                        var percentageOfHealthToTake = entry.statusEffect.damagePercentualValue * GetMaxHealth() / 100;
                        var newHealth = currentHealth - percentageOfHealthToTake;
                        this.currentHealth = Mathf.Clamp(newHealth, 0, GetMaxHealth());
                    }
                    else
                    {

                    }
                }
                else
                {
                    var newHealth = currentHealth - (entry.statusEffect.damagePerSecond * Time.deltaTime);

                    currentHealth = Mathf.Clamp(newHealth, 0, GetMaxHealth());
                }

                if (currentHealth <= 0)
                {
                    Die();

                    // Remove this status
                    statusToDelete.Add(enemyAppliedStatus);
                }

                return;
            }


            if (entry.statusEffect.statAffected == Stat.Stamina)
            {
                return;
            }
        }

        #endregion

        #region Poise

        void UpdatePoise()
        {
            if (enemy.maxPoiseHits > 1)
            {
                resetPoiseTimer += Time.deltaTime;

                if (resetPoiseTimer >= enemy.maxTimeBeforeResettingPoise)
                {
                    currentPoiseHitCount = 0;
                    resetPoiseTimer = 0f;
                }
            }

            if (cooldownBeforeTakingAnotherHitToPoise < enemy.maxCooldownBeforeTakingAnotherHitToPoise)
            {
                cooldownBeforeTakingAnotherHitToPoise += Time.deltaTime;
            }
        }

        public void IncreasePoiseDamage(int poiseDamage)
        {
            currentPoiseHitCount = Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, enemy.maxPoiseHits);


            if (currentPoiseHitCount >= enemy.maxPoiseHits)
            {
                ActivatePoiseDamage();
            }
            else
            {
                // If enemy was hit, force him into combat
                if (IsInCombat() == false)
                {
                    agent.isStopped = true;
                    animator.SetBool(hashChasing, true);
                }
            }
        }


        public void ActivatePoiseDamage()
        {
            if (cooldownBeforeTakingAnotherHitToPoise < enemy.maxCooldownBeforeTakingAnotherHitToPoise)
            {
                return;
            }

            currentPoiseHitCount = 0;

            if (animator != null)
            {
                cooldownBeforeTakingAnotherHitToPoise = 0f;

                animator.SetTrigger(hashTakingDamage);

                BGMManager.instance.PlaySoundWithPitchVariation(enemy.isMale ? enemy.hurtSfx : enemy.femaleHurtSfx, combatAudioSource);
            }
        }
        #endregion

        #region Weapons
        void ShowWeapons()
        {
            if (leftHandWeapon != null)
            {
                leftHandWeapon.gameObject.SetActive(true);
            }

            if (rightHandWeapon != null)
            {
                rightHandWeapon.gameObject.SetActive(true);
            }
        }

        void HideWeapons()
        {
            if (leftHandWeapon != null)
            {
                leftHandWeapon.gameObject.SetActive(false);
            }

            if (rightHandWeapon != null)
            {
                rightHandWeapon.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Projectile

        void UpdateProjectile()
        {
            if (projectilePrefab == null)
            {
                return;
            }

            if (projectileCooldown <= maxProjectileCooldown)
            {
                projectileCooldown += Time.deltaTime;
            }

            if (IsShooting() == false)
            {
                projectileCooldown += Time.deltaTime;
            }
        }

        public void ShowBow()
        {
            if (bowGraphic == null) { return; }

            HideWeapons();

            bowGraphic.gameObject.SetActive(true);
        }

        public void HideBow()
        {
            if (bowGraphic == null) { return; }

            bowGraphic.gameObject.SetActive(false);

            ShowWeapons();
        }


        public void PrepareProjectile()
        {
            agent.isStopped = true;
            animator.Play(hashShooting);
            projectileCooldown = 0f;
        }

        public bool CanShoot()
        {
            return projectilePrefab != null && projectileCooldown >= maxProjectileCooldown;
        }

        public bool IsShooting()
        {
            return animator.GetBool(hashIsShooting);
        }


        /// <summary>
        ///  Animation Event
        /// </summary>
        public void FireProjectile()
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPointRef.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            projectile.Shoot(playerClimbController.playerHeadRef.transform);
        }
        #endregion

        #region Dodge

        void UpdateDodgeCounter()
        {
            if (dodgeCooldown < maxDodgeCooldown)
            {
                dodgeCooldown += Time.deltaTime;

            }
        }

        public void CheckForDodgeChance()
        {

            // If player is attacking, evaluate if enemy can dodge attack
            if (playerCombatController.isCombatting)
            {
                if (dodgeCooldown >= maxDodgeCooldown)
                {
                    dodgeCooldown = 0f;

                    // Roll dodge dice
                    float chance = UnityEngine.Random.RandomRange(0, 100);

                    if (chance <= dodgeWeight)
                    {
                        Dodge();
                    }
                }
            }
        }

        public void Dodge()
        {
            if (canCircleAround)
            {
                animator.SetBool(hashIsStrafing, false);
            }

            if (dodgeLeftOrRight)
            {
                var playerIsOnTheLeft = PlayerIsOnTheLeft();

                if (playerIsOnTheLeft)
                {

                    NavMeshHit rightHit;
                    NavMesh.SamplePosition(transform.position + transform.right, out rightHit, 10f, NavMesh.AllAreas);

                    if (rightHit.position != null)
                    {
                        DisableHealthHitboxes();
                        animator.Play("Dodge_Right");
                        return;
                    }
                }
                else
                {
                    NavMeshHit leftHit;
                    NavMesh.SamplePosition(transform.position + transform.right, out leftHit, 10f, NavMesh.AllAreas);

                    if (leftHit.position != null)
                    {
                        DisableHealthHitboxes();
                        animator.Play("Dodge_Left");
                        return;
                    }
                }

                return;
            }

            if (customDodgeClips.Length > 0)
            {
                DisableHealthHitboxes();

                var dice = Random.Range(0, customDodgeClips.Length);

                animator.Play(customDodgeClips[dice]);
            }

        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void ActivateDodge()
        {
            if (enemy.dodgeSfx != null && combatAudioSource != null)
            {
                BGMManager.instance.PlaySoundWithPitchVariation(enemy.dodgeSfx, combatAudioSource);
            }

            DisableHealthHitboxes();
        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void DeactivateDodge()
        {
            EnableHealthHitboxes();
        }
        #endregion


        #region Pivoting

        public bool PlayerIsBehind()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle >= 100 && viewableAngle <= 180)
            {
                return true;
            }

            if (viewableAngle <= -101 && viewableAngle >= -180)
            {
                return true;
            }

            return false;
        }
        public bool PlayerIsOnTheLeft()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle >= 0 && viewableAngle <= 100)
            {
                return true;
            }

            return false;
        }

        public bool PlayerIsOnTheRight()
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            if (viewableAngle < 0 && viewableAngle >= -100)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Grounded Logic
        public bool IsGrounded()
        {

            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), animator.transform.up * -1f, out hit, 2f);
            // Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), animator.transform.up * -1f, Color.red);

            return hit.transform != null;
        }

        public void ReenableNavmesh()
        {
            NavMeshHit rightHit;
            NavMesh.SamplePosition(transform.position, out rightHit, 999f, NavMesh.AllAreas);

            agent.enabled = false;
            agent.nextPosition = rightHit.position;

            agent.updatePosition = true;
            agent.enabled = true;

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            if (animator.GetBool(hashIsWaiting) == false)
            {
                animator.Play(hashWaiting);
            }
        }


        #endregion

        #region Companions

        public void UpdateCompanionsFocusAI()
        {
            if (focusedOnCompanionsTimer < maxTimeFocusedOnCompanions)
            {
                focusedOnCompanionsTimer += Time.deltaTime;
            }
            else if (focusedOnCompanionsTimer >= maxTimeFocusedOnCompanions && currentCompanion != null)
            {
                BreakCompanionFocus();
            }
        }

        public void FocusOnCompanion(CompanionManager companionManager)
        {
            var dice = Random.Range(0, 100);

            if (dice > focusOnCompanionWeight)
            {
                return;
            }

            if (this.ignoreCompanions)
            {
                return;
            }

            focusedOnCompanionsTimer = 0f;

            currentCompanion = companionManager;

            agent.SetDestination(currentCompanion.transform.position);
        }

        public void BreakCompanionFocus()
        {
            currentCompanion = null;

            // Refocus on player
            agent.SetDestination(player.transform.position);
        }

        #endregion

    }
}
