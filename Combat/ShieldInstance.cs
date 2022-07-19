using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(SphereCollider))]
    public class ShieldInstance : MonoBehaviour
    {

        SphereCollider collider => GetComponent<SphereCollider>();

        List<Character> characters = new List<Character>();

        private void OnTriggerStay(Collider other)
        {
            Character character = other.gameObject.GetComponent<Character>();
            if (character == null)
            {
                return;
            }

            if (characters.Contains(character))
            {
                return;
            }

            characters.Add(character);
        }

        public void OnEnable()
        {
            collider.enabled = true;
        }

        public void OnDisable()
        {
            collider.enabled = false;
            characters.Clear();
        }

        public Character FindClosestCharacter(Vector3 playerPosition)
        {
            Character closestCharacter = null;


            foreach (Character c in characters)
            {
                if (closestCharacter == null)
                {
                    closestCharacter = c;
                }
                else if (Vector3.Distance(c.transform.position, playerPosition) < Vector3.Distance(closestCharacter.transform.position, playerPosition))
                {
                    closestCharacter = c;
                }
            }

            return closestCharacter;
        }
    }

}