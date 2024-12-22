using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillHolder : MonoBehaviour
{
    [ReadOnly] public float currentCD;
    [ReadOnly] public float coolDownUI; // This will store the cooldown progress (0 to 1)

    [FoldoutGroup("Skill List")]
    public List<GameObject> skillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<GameObject> passiveSkillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<GameObject> activeSkillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<string> SkillIDList, SkillOBJNameList;

    [FoldoutGroup("Current Active Skill")]
    public int currentActiveSkill = 0;
    [FoldoutGroup("Current Active Skill")]
    [ReadOnly] public GameObject activeSkill;
    [FoldoutGroup("Current Active Skill")]
    [ReadOnly] public ISkill currentSkill;

    [FoldoutGroup("Setup")]
    public float SoulRecover = 20, MaxSlot = 6;
    [FoldoutGroup("Setup")]
    public PlayerController _pc;
    [FoldoutGroup("Setup")]
    public List<GameObject> StartSkill;
    [FoldoutGroup("Setup")]
    public Image currentSkillUISprite, CDOverlay;
    [FoldoutGroup("Setup")]
    [CanBeNull] public Button SwitchButton;

    [FoldoutGroup("Setup")] public Sprite defaultUISprite;

    public static SkillHolder Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        _pc = PlayerController.Instance;

        InitStartSkill();

        SwitchButton.gameObject.SetActive((activeSkillList.Count > 0));
    }

    void InitStartSkill()
    {
        // Instantiate and categorize skills based on their type
        foreach (GameObject skillPrefab in StartSkill)
        {
            AddSkill(skillPrefab);
        }

        // Set the initial active skill from the ActiveSkillList
        SetActiveSkill(currentActiveSkill);
    }

    #region Util

    [Button]
    public async void AddSkill(GameObject skillPrefab)
    {
        ISkill skillComponent = skillPrefab.GetComponent<ISkill>();
        string skillID = skillComponent.Name;
        string skillOBJName = skillComponent.name;

        // Check if the skill already exists
        if (SkillIDList.Contains(skillID) || skillList.Count >= MaxSlot)
        {
            //PlayerController.Instance.PlayerProgressData.SoulCollected += SoulRecover;
            PlayerController.Instance.PlayerProgressData.AddCurrency(0, SoulRecover);
            Debug.Log("Skill already exists: " + skillID);
            return;
        }

        // Instantiate the skill and set it up
        GameObject skillInstance = Instantiate(skillPrefab);
        skillInstance.transform.SetParent(this.transform);
        skillInstance.transform.localPosition = Vector3.zero;
        skillInstance.transform.localRotation = Quaternion.identity;

        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        skillComponent.Assign();
        SkillIDList.Add(skillID);  // Add to skill ID list to track uniqueness
        SkillOBJNameList.Add(skillOBJName);

        // Add the skill to the appropriate list based on its type
        if (skillComponent.type == SkillType.ACTIVE)
        {
            activeSkillList.Add(skillInstance);  // Add to active skill list
        }
        else if (skillComponent.type == SkillType.PASSIVE)
        {
            passiveSkillList.Add(skillInstance);  // Add to passive skill list
            skillComponent.Activate();  // Auto-activate passive skills
        }

        skillList.Add(skillInstance);  // Add all skills to the master list

        // Update the switch button's active state if it exists
        if (SwitchButton != null)
        {
            SwitchButton.gameObject.SetActive(activeSkillList.Count > 1);
            if (activeSkillList.Count == 1) SetActiveSkill(0);
        }
    }

    [Button]
    public void AddSkillWithID(string skillID)
    {
        // Validate the input skillID
        if (string.IsNullOrEmpty(skillID))
        {
            Debug.LogWarning("Skill ID is empty or null.");
            return;
        }

        // Retrieve the skill prefab from the SkillDatabase
        GameObject skill = PlayerController.Instance.PlayerProgressData.
            skillDatabase.allSkills.Find(s => s.Skill_ID == skillID).skillPrefab;

        if (skill == null)
        {
            Debug.Log($"Skill with ID '{skillID}' not found in the skill database.");
            return;
        }

        AddSkill(skill);
    }

    public void RemoveSkillSlot(int slot)
    {
        var skillComponent = skillList[slot].GetComponent<ISkill>();

        if (skillComponent.type == SkillType.PASSIVE)
        {
            skillComponent.Deactivate();
        }
        else
        {
            //do remove active stuffs
        }
    }

    #endregion

    void Update()
    {
        UpdateCooldownUI();
    }

    #region Input
    public void ActivateSkill()
    {
        if (activeSkill == null) return;
        currentSkill.Activate();
        AudioManager.Instance.PlaySound("Click");

    }

    public void DeactivateSkill()
    {
        if (activeSkill == null) return;
        currentSkill.Deactivate();
    }

    public void NextSkill()
    {
        if (_pc == null) _pc = PlayerController.Instance;
        //if (_pc.blockInput) return;
        Debug.Log("next");

        if (currentActiveSkill + 1 < activeSkillList.Count)
            currentActiveSkill += 1;
        else
            currentActiveSkill = 0;

        SetActiveSkill(currentActiveSkill);
    }


    [Button]
    void SetActiveSkill(int slot)
    {
        if (slot >= 0 && slot < activeSkillList.Count)
        {
            currentActiveSkill = slot;
            activeSkill = activeSkillList[slot];
            currentSkill = activeSkill.GetComponent<ISkill>();
            currentSkillUISprite.sprite = currentSkill.Icon;

            // Set initial cooldown values
            currentCD = currentSkill.currentCD;
            coolDownUI = (currentSkill.Cooldown > 0) ? Mathf.Clamp01(currentSkill.currentCD / currentSkill.Cooldown) : 0;
        }
    }
    #endregion

    #region Timer
    public void Timer()
    {
        if (currentCD >= 0) currentCD -= Time.deltaTime;
    }

    public void TimerAdd(float addTime)
    {
        currentCD += addTime;
    }

    private void UpdateCooldownUI()
    {
        if (currentSkill == null) return;

        // Update current cooldown and calculate cooldown percentage (0 to 1)
        currentCD = currentSkill.currentCD;
        coolDownUI = (currentSkill.Cooldown > 0) ? Mathf.Clamp01(currentSkill.currentCD / currentSkill.Cooldown) : 0;

        // update the UI image or any other visual indicator using coolDownUI here
        if (CDOverlay != null)
            CDOverlay.fillAmount = coolDownUI;
    }
    #endregion
}
