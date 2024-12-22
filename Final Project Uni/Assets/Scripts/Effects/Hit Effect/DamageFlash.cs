using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{

    #region Variables
    [SerializeField] public Material FlashMaterial;

    [SerializeField] public float flashTime = 0.25f;
    
    public List<GameObject> skippedObjects = new List<GameObject>();

    private SkinnedMeshRenderer[] _meshSkinnedRenderers;
    private MeshRenderer[] _meshRenderers;
    private List<Material[]> _oldMaterials = new List<Material[]>();
    private List<Material[]> _oldMeshMaterials = new List<Material[]>();
    private Coroutine _damageFlashCoroutine;

    //[SerializeField, CanBeNull] private HitStop _hitStop;

    #endregion
    
    #region UnityMethods

    private void Awake()
    {
        _meshSkinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);

        // Filter out renderers from skipped objects
        _meshSkinnedRenderers = FilterSkippedObjects(_meshSkinnedRenderers);
        _meshRenderers = FilterSkippedObjects(_meshRenderers);

        Init();
    }

    #endregion

    #region Events

    private void Init()
    {
        //assign mesh renderer materials to _materials
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _meshSkinnedRenderers)
        {
            Material[] materials = new Material[skinnedMeshRenderer.materials.Length];
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                materials[i] = skinnedMeshRenderer.materials[i];
            }
            _oldMaterials.Add(materials);
        }

        foreach (var meshRenderer in _meshRenderers)
        {
            Material[] materials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                materials[i] = meshRenderer.materials[i];
            }
            _oldMeshMaterials.Add(materials);
        }
        
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());

        //FindObjectOfType<HitStop>().Stop();
    }
    public IEnumerator DamageFlasher()
    {
        ChangeMaterial();

        yield return new WaitForSeconds(flashTime);

        RevertMaterial();
    }

    public void ChangeMaterial()
    {
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in _meshSkinnedRenderers)
        {
            Material[] flashMaterials = skinnedMeshRenderer.materials;
            for (int i = 0; i < flashMaterials.Length; i++)
            {
                flashMaterials[i] = FlashMaterial;
            }
            skinnedMeshRenderer.materials = flashMaterials;
        }
        
        foreach (var meshRenderer in _meshRenderers)
        {
            Material[] flashMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                flashMaterials[i] = FlashMaterial;
            }
            meshRenderer.materials = flashMaterials;
        }
        //Debug.Log("Flashed");
    }

    public void RevertMaterial()
    {
        for (int i = 0; i < _meshSkinnedRenderers.Length; i++)
        {
            _meshSkinnedRenderers[i].materials = _oldMaterials[i];
        }

        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            _meshRenderers[i].materials = _oldMeshMaterials[i];
        }
        //Debug.Log("Reverted");
    }

    #endregion

    #region Ults

    [FoldoutGroup("Event Test")]
    [Button]
    public void DoDamageFlash()
    {
        CallDamageFlash();
        
        //if(_hitStop != null) _hitStop.Stop();
    }

    private T[] FilterSkippedObjects<T>(T[] renderers) where T : Renderer
    {
        List<T> filteredList = new List<T>();
        foreach (T renderer in renderers)
        {
            if (!skippedObjects.Contains(renderer.gameObject))
            {
                filteredList.Add(renderer);
            }
        }
        return filteredList.ToArray();
    }
    
    #endregion
}
