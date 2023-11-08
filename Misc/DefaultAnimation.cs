using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DefaultAnimation : MonoBehaviour
    {
        Animator animator => GetComponent<Animator>();

        public string animationName;

        [Header("Drink Gesture")]
        public bool canDrink = false;
        public string drinkAnimation = "";
        public float drinkInterval = 5f;
        public bool hideDrinkOnIdle = true;
        public float drinkDuration = 1f;
        public GameObject drinkGraphic;
        float drinkCooldown = Mathf.Infinity;

        private void Start()
        {
            if (animator != null)
            {
                animator.Play(animationName);
            }

            if (hideDrinkOnIdle && drinkGraphic != null)
            {
                drinkGraphic.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (animator != null)
            {
                animator.Play(animationName);
            }
        }

        private void Update()
        {
            if (canDrink)
            {
                HandleDrink();
            }
        }

        void HandleDrink()
        {
            if (drinkCooldown >= drinkInterval)
            {
                DecideIfWillDrink();
            }
            else
            {
                drinkCooldown += Time.deltaTime;
            }
        }

        void DecideIfWillDrink()
        {
            drinkCooldown = 0f;
            int randomChance = Random.Range(0, 100);
            if (randomChance > 50)
            {
                Drink();
            }
        }

        void Drink()
        {
            if (drinkGraphic != null)
            {
                drinkGraphic.gameObject.SetActive(true);
            }

            animator.Play(drinkAnimation);

            StartCoroutine(HideDrink());
        }

        IEnumerator HideDrink()
        {
            yield return new WaitForSeconds(drinkDuration);

            if (hideDrinkOnIdle)
            {
                drinkGraphic.gameObject.SetActive(false);
            }

            animator.Play(animationName);
        }

    }

}
