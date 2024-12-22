using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuickParticlePlayer : SerializedMonoBehaviour
{
    // Dictionary to map a string key (like a particle system name or tag) to a List of ParticleSystem components.
    [SerializeField] private Dictionary<string, List<ParticleSystem>> particleDictionary;

    #region Play Method
    // Play all ParticleSystems in the dictionary for a given key.
    public void Play(string particleKey)
    {
        if (!particleDictionary.TryGetValue(particleKey, out List<ParticleSystem> particleSystems)) return;

        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
    }
    #endregion

    #region Stop Method
    // Stop all ParticleSystems in the dictionary for a given key.
    public void Stop(string particleKey)
    {
        if (!particleDictionary.TryGetValue(particleKey, out List<ParticleSystem> particleSystems)) return;

        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }
    }
    #endregion
}