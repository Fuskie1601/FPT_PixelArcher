using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpeditionReportUI : MonoBehaviour
{
    public GameObject ERUIPrefab;
    public Transform ERListContent;
    [ReadOnly] public ExpeditionReport _expeditionReport;

    [FoldoutGroup("UI")] 
    public Sprite defaultSprite;
    [FoldoutGroup("UI")]
    public TMP_Text EnemyDefeat,KnowledgeLevel,GoldCollected,SoulCollected;


    private void OnEnable()
    {
        doUpdate();
    }

    [Button("Update")]
    public void doUpdate()
    {
        _expeditionReport = ExpeditionReport.Instance;
        if (_expeditionReport == null)
        {
            Debug.Log("ExpeditionReport instance not found.");
            return;
        }

        UpdateExpeditionReport();
    }
    
    void UpdateExpeditionReport()
    {
        // Clear existing UI elements
        foreach (Transform child in ERListContent)
        {
            Destroy(child.gameObject);
        }

        if(EnemyDefeat != null) EnemyDefeat.text = _expeditionReport.EnemyDefeated.ToString();
        if(KnowledgeLevel != null) KnowledgeLevel.text = _expeditionReport.KnowledgeLevelCollected.ToString();
        if(GoldCollected != null) GoldCollected.text = _expeditionReport.GoldCollected.ToString();
        if(SoulCollected != null) SoulCollected.text = _expeditionReport.SoulCollected.ToString();

        // Populate the list with notifications
        foreach (var element in _expeditionReport.ReportElements)
        {
            GameObject notifUIObj = Instantiate(ERUIPrefab, ERListContent);
            
            var elementUI = notifUIObj.GetComponent<EReportElementUI>(); // Assuming ERUIPrefab uses a NotificationUI script
            if (elementUI != null)
            {
                elementUI.notificationText.text = element.text;
                if(elementUI.icon != null) elementUI.icon.sprite = element.icon;
                else elementUI.icon.sprite = defaultSprite;
            }
        }
    }
    
    public async void OnNewGameClicked()
    {
        GameManager.Instance.fadeInAnim.Invoke();
            
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        GameManager.Instance.StartLobby();
    }
}
