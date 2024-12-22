using UnityEngine;
using UnityEngine.UI;

namespace PA
{
    public class UISettingsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button graphicsPanelButton;
        [SerializeField] private Button soundPanelButton;
        [SerializeField] private Button backSettingsMenuButton;

        private UIMainMenu mainMenu;
        private MakeTransitionUI makeTransitionUI;

        private void Awake()
        {
            mainMenu = GetComponentInParent<UIMainMenu>();
            makeTransitionUI = mainMenu.makeTransitionUI;
        }

        public void OnBackSettingsClicked()
        {
            mainMenu.GeneralClick(mainMenu.MainMenuPanel, settingsPanel);
        }

        public void OnGraphicsPanelClicked()
        {
            mainMenu.GeneralClick(mainMenu.GraphicsMenu.GraphicsPanel, settingsPanel);
        }

        public void OnSoundPanelClicked()
        {
            mainMenu.GeneralClick(mainMenu.SoundMenu.SoundPanel, settingsPanel);
        }

        public GameObject SettingsPanel => settingsPanel;
    }
} 