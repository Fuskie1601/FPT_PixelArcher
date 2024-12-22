using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PA
{
    public class UISoundMenu : MonoBehaviour
    {
        [SerializeField] private GameObject soundPanel;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private TMP_Text musicVolumeTextBG;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Text musicVolumeTextSFX;
        [SerializeField] private Button backSoundSettingsButton;

        [CanBeNull] private UIMainMenu mainMenu;
        private MakeTransitionUI makeTransitionUI;

        private void Awake()
        {
            mainMenu = GetComponentInParent<UIMainMenu>();
            if (mainMenu != null) makeTransitionUI = mainMenu.makeTransitionUI;
        }

        private void Start()
        {
            InitializeUI();
            LoadSettings();
        }

        private void InitializeUI()
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        private void LoadSettings()
        {
            GameSettings settings = GameSettings.Instance;
            musicVolumeSlider.value = settings.musicVolume;
            sfxVolumeSlider.value = settings.sfxVolume;
        }

        public void OnBackSoundSettingsClicked()
        {
            if(mainMenu != null)
            mainMenu.GeneralClick(mainMenu.SettingsMenu.SettingsPanel, soundPanel);
        }

        public void OnMusicVolumeChanged(float value)
        {
            GameSettings.Instance.musicVolume = value;
            SoundManager.Instance.SetMusicVolume(value);
            musicVolumeTextBG.text = value.ToString();
        }

        public void OnSFXVolumeChanged(float value)
        {
            GameSettings.Instance.sfxVolume = value;
            SoundManager.Instance.SetSFXVolume(value);
            musicVolumeTextSFX.text = value.ToString();
        }

        public GameObject SoundPanel => soundPanel;
    }
} 
 