using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public static class Utils
    {

        public static Vector3 GetNearestNavMeshPoint(Vector3 reference)
        {
            return Utils.GetNearestNavMeshPoint(reference, NavMesh.AllAreas);
        }

        public static Vector3 GetNearestNavMeshPoint(Vector3 reference, int area)
        {
            // Teleport near player
            NavMesh.SamplePosition(reference, out NavMeshHit hit, Mathf.Infinity, area);

            if (hit.position == null)
            {
                return reference;
            }

            return hit.position;
        }

        public static void AvoidInvalidPaths(NavMeshAgent agent)
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(agent.transform.position, agent.transform.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {

                NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas);

                if (!float.IsNaN(hit.position.x) && !float.IsInfinity(hit.position.x) &&
                    !float.IsNaN(hit.position.y) && !float.IsInfinity(hit.position.y) &&
                    !float.IsNaN(hit.position.z) && !float.IsInfinity(hit.position.z))
                {
                    // It's a valid position, so assign it to nextPosition
                    agent.nextPosition = hit.position != null ? hit.position : agent.transform.position;
                    agent.updatePosition = true;
                }
                else
                {
                    // Handle the case where the position is invalid
                    Debug.LogError("Invalid positionWithLocalOffset: " + hit.position);
                }

            }
        }

        public static void UpdateTransformChildren(Transform transformTarget, bool isActive)
        {
            if (transformTarget.childCount <= 0)
            {
                return;
            }

            foreach (Transform transformChild in transformTarget)
            {
                transformChild.gameObject.SetActive(isActive);
            }
        }

        public static string GetItemPath(Item item)
        {
            var prefix = "Items/";

            var subFolder = "";

            if (item is Accessory)
            {
                subFolder = "Accessories/";
            }
            else if (item is CraftingMaterial)
            {
                subFolder = "Alchemy/";
            }
            else if (item is UpgradeMaterial)
            {
                subFolder = "Upgrade Materials/";
            }
            else if (item is Arrow)
            {
                subFolder = "Arrows/";
            }
            else if (item is Consumable)
            {
                subFolder = "Consumables/";
            }
            else if (item is ConsumableProjectile)
            {
                subFolder = "Consumables/";
            }
            else if (item is ConsumableProjectile)
            {
                subFolder = "Consumables/";
            }
            else if (item is Helmet)
            {
                subFolder = "Helmets/";
            }
            else if (item is Armor)
            {
                subFolder = "Armors/";
            }
            else if (item is Gauntlet)
            {
                subFolder = "Gauntlets/";
            }
            else if (item is Legwear)
            {
                subFolder = "Legwears/";
            }
            else if (item is Shield)
            {
                subFolder = "Shields/";
            }
            else if (item is Spell)
            {
                subFolder = "Spells/";
            }
            else if (item is Weapon)
            {
                subFolder = "Weapons/";
            }
            else if (item is KeyItem || item is Item)
            {
                subFolder = "Key Items/";
            }

            return prefix + subFolder + item.name;
        }
    }
}
