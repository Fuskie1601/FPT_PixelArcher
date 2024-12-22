using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class TrailEffect : MonoBehaviour
{
    public TrailRenderer _trailRenderer;

    [Button]
    public void TrailToggle(bool toggle)
    {
        _trailRenderer.emitting = toggle;
    }
}
