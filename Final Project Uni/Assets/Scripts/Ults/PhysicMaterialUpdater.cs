using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just to fix unity dumb shiet
public class PhysicMaterialUpdater : MonoBehaviour
{
    private Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();
        if (collider == null || collider.sharedMaterial == null)
            return;

        collider.material = new PhysicMaterial()
        {
            dynamicFriction = collider.sharedMaterial.dynamicFriction,
            staticFriction = collider.sharedMaterial.staticFriction,
            bounciness = collider.sharedMaterial.bounciness,
            frictionCombine = collider.sharedMaterial.frictionCombine,
            bounceCombine = collider.sharedMaterial.bounceCombine
        };
    }
}
