using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTotem : MonoBehaviour
{
    public bool toggle = true; // Toggle to turn on/off the totem's effect
    public float range = 10f;  // Range within which the totem affects bots
    public float buffTime = 1f;  
    public bool rayCheck = false; // If true, perform a raycast check before applying the effect
    public LayerMask ignoreLayers; // Layers to ignore during the raycast

    private List<BotMain> botsInRange = new List<BotMain>();

    void Update()
    {
        if (!toggle) return; // Guard clause for toggling totem's effect

        FindBotsInRange();
        ApplyTotemEffect();
    }

    // Function to find all bots within the specified range
    private void FindBotsInRange()
    {
        botsInRange.Clear();

        // Find all colliders within the range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider hitCollider in hitColliders)
        {
            BotMain bot = hitCollider.GetComponent<BotMain>();
            if (bot == null) continue; // Guard clause if no BotMain component found

            botsInRange.Add(bot);
        }
    }

    // Function to apply the totem effect to all bots in range, with optional raycast validation
    private void ApplyTotemEffect()
    {
        foreach (BotMain bot in botsInRange)
        {
            if (bot == null) continue;// || bot.Health == null) continue; // Guard clause for null bot or health component

            if (rayCheck)
            {
                Vector3 directionToBot = bot.transform.position - transform.position;
                RaycastHit hit;

                // Perform raycast towards the bot, ignoring specified layers
                if (!Physics.Raycast(transform.position, directionToBot, out hit, range, ~ignoreLayers)) continue; // Guard clause if raycast doesn't hit
                if (hit.collider.gameObject != bot.gameObject) continue; // Guard clause if raycast doesn't hit the bot itself
            }

            bot.Health.Invincible(buffTime); // Apply the invincible effect
        }
    }

    // Optional: Visualize the range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
