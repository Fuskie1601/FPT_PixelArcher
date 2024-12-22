using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PA
{
    public class LobbyUI : MonoBehaviour
    {
        public TMP_Text goldText;
        private PlayerStats playerStats;

        private async void Start()
        {
            await UniTask.DelayFrame(1);
            playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
            UpdateGoldText(playerStats.playerSoul);
        }

        public void UpdateGoldText(int gold)
        {
            goldText.text = gold.ToString();
        }
    }
}
