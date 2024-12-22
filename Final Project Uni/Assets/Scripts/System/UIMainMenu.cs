using System;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PA
{
    public class UIMainMenu : MonoBehaviour
    {
        public static UIMainMenu Instance { get; private set; }

        [Header("Main Menu UI")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button optionButton;

        [Header("References")]
        public UIFadeSelfAnim fadeUI;
        public MakeTransitionUI makeTransitionUI;
        [SerializeField] private UISettingsMenu settingsMenu;
        [SerializeField] private UIGraphicsMenu graphicsMenu;
        [SerializeField] private UISoundMenu soundMenu;

        public SceneLoader _sceneLoader;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AudioManager.Instance.ChangeMusic("MainMenuBGM");
        }

        public void GeneralClick(GameObject newChild, GameObject oldChild)
        {
            newChild.SetActive(true);
            oldChild.SetActive(true);
            makeTransitionUI.ConfigTwoGameObjectOfTransition(newChild, oldChild);
            makeTransitionUI.MakeTransition();

            DelayedDeactivationAsync().Forget();

            async UniTaskVoid DelayedDeactivationAsync()
            {
                await UniTask.Delay(TimeSpan.FromSeconds(makeTransitionUI.duration));
                oldChild.SetActive(false);
            }
        }
        
        public async void OnContinueClicked()
        {
            Debug.Log("continue");
            
            fadeUI.doFadeIn();
            await UniTask.Delay(TimeSpan.FromSeconds(makeTransitionUI.duration));
            
            if(_sceneLoader != null)
                _sceneLoader.LoadScene(2);
            else
                SceneManager.LoadScene("TestGenMap");
            
            gameObject.SetActive(false);
            await UniTask.WaitUntil(() => ExpeditionManager.Instance != null);
            ExpeditionManager.Instance.LoadProgress();
            
            //Destroy(gameObject);
        }

        public async void OnNewGameClicked()
        {           
            //makeTransitionUI.MakeFadeTransition();            
            fadeUI.doFadeIn();
            await UniTask.Delay(TimeSpan.FromSeconds(makeTransitionUI.duration));
            
            
            if(_sceneLoader != null)
                _sceneLoader.LoadScene(1);
            else
                SceneManager.LoadScene("Lobby Optimized");
            
            
            return;
            
            await UniTask.Delay(TimeSpan.FromSeconds(makeTransitionUI.duration));
            makeTransitionUI.fadeBlackImage.GetComponent<CanvasGroup>().alpha = 1;
            
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }

        public void OnSettingsClicked()
        {
            GeneralClick(settingsMenu.SettingsPanel, mainMenuPanel);
        }


        // Properties for accessing panels
        public GameObject MainMenuPanel => mainMenuPanel;
        public UISettingsMenu SettingsMenu => settingsMenu;
        public UIGraphicsMenu GraphicsMenu => graphicsMenu;
        public UISoundMenu SoundMenu => soundMenu;
    }
}