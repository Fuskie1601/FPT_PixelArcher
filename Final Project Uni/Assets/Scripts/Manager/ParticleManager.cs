using Sirenix.OdinInspector;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleManager : SerializedMonoBehaviour
{
    #region Variables
    [FoldoutGroup("Settings")]
    public static ParticleManager Instance { get; private set; }

    [FoldoutGroup("Settings")]
    public bool PrefabManager;

    [FoldoutGroup("Settings")] 
    //[SerializeField] private EffectList effectList;
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<string, List<GameObject>> particleDictionary;
    #endregion

    #region UnityMethod
    private void Awake()
    {
        if (Instance == null && !PrefabManager) Instance = this;
    }
    #endregion

    #region Set Transform
    public GameObject SpawnParticle(string particleName, Vector3 position, Quaternion rotation)
    {
        if (!particleDictionary.TryGetValue(particleName, out List<GameObject> particlePrefabs) || particlePrefabs == null || particlePrefabs.Count == 0)
        {
            Debug.LogError($"Particle '{particleName}' not found or has no available prefabs.");
            return null;
        }

        GameObject particlePrefab = particlePrefabs[0];
        return PoolManager.Instance.Spawn(particlePrefab, position, rotation);
    }

    public void SetParticlePosition(GameObject particleObject, Vector3 newPosition)
    {
        if (particleObject == null) return;
        particleObject.transform.position = newPosition;
    }

    public void RotateParticle(GameObject particleObject, float rotateX, float rotateY, float rotateZ)
    {
        if (particleObject == null) return;

        Quaternion rotation = Quaternion.Euler(rotateX, rotateY, rotateZ);
        particleObject.transform.rotation = rotation;
    }
    #endregion

    #region Ults
    [FoldoutGroup("Event")] [Button]
    public void SetParticlePositionAndRotation(GameObject particleObject, Vector3 newPosition, Quaternion newRotation)
    {
        if (particleObject == null) return;
        SetParticlePosition(particleObject, newPosition);
        RotateParticle(particleObject, newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
    }

    [FoldoutGroup("Event")] [Button]
    public GameObject SpawnOppositeParticle(string particleName, Vector3 bulletDirection)
    {
        if (!particleDictionary.TryGetValue(particleName, out List<GameObject> particlePrefabs) || particlePrefabs == null || particlePrefabs.Count == 0)
        {
            Debug.LogError($"Particle '{particleName}' not found or has no available prefabs.");
            return null;
        }

        Vector3 oppositeDirection = -bulletDirection.normalized;
        GameObject particlePrefab = particlePrefabs[Random.Range(0, particlePrefabs.Count)];
        return SpawnParticle(particleName, oppositeDirection, Quaternion.identity);
    }

    public ParticleSystem GetParticleSystem(string particleName)
    {
        if (!particleDictionary.TryGetValue(particleName, out List<GameObject> particlePrefabs) || particlePrefabs == null || particlePrefabs.Count == 0)
        {
            Debug.LogError($"Particle '{particleName}' not found or has no available prefabs.");
            return null;
        }

        foreach (var particlePrefab in particlePrefabs)
        {
            if (particlePrefab.TryGetComponent(out ParticleSystem particleSystem))
            {
                return particleSystem;
            }
        }

        Debug.LogError($"No ParticleSystem component found on any of the '{particleName}' prefabs.");
        return null;
    }

    [Button]
    public void PlayAssignedParticle(string particleName)
    {
        if (!particleDictionary.TryGetValue(particleName, out List<GameObject> particlePrefabs) || particlePrefabs == null || particlePrefabs.Count == 0)
        {
            Debug.Log($"Particle '{particleName}' not found or has no available prefabs.");
            return;
        }

        foreach (var particlePrefab in particlePrefabs)
        {
            var particleSystem = particlePrefab.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            else
            {
                Debug.Log($"No ParticleSystem component found on prefab '{particlePrefab.name}' for '{particleName}'.");
            }
        }
    }
    
    public void StopAssignedParticle(string particleName)
    {
        if (!particleDictionary.TryGetValue(particleName, out List<GameObject> particlePrefabs) || particlePrefabs == null || particlePrefabs.Count == 0)
        {
            Debug.Log($"Particle '{particleName}' not found or has no available prefabs.");
            return;
        }

        foreach (var particlePrefab in particlePrefabs)
        {
            var particleSystem = particlePrefab.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
            else
            {
                Debug.Log($"No ParticleSystem component found on prefab '{particlePrefab.name}' for '{particleName}'.");
            }
        }
    }

    [Button]
    public void RemoveParticle(string particleName)
    {
        if (particleDictionary.ContainsKey(particleName))
        {
            particleDictionary.Remove(particleName);
            Debug.Log($"Particle '{particleName}' removed from the dictionary.");
        }
        else
        {
            Debug.LogError($"Particle '{particleName}' not found in the dictionary.");
        }
    }
    #endregion
}
