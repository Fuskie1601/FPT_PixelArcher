using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    [CanBeNull] public GameObject Player;
    [CanBeNull] public GameObject ManagerObj;
    [CanBeNull] public GenerationManager genManager;
    [CanBeNull] public SceneLoader _sceneLoader;

    // Scene Address (Using buttons for easier scene path selection)
    [FoldoutGroup("Scene Address")]
    [SerializeField] public string ExpeditionPath;
    [FoldoutGroup("Scene Address")]
    [SerializeField] public string LobbyPath;

    [FoldoutGroup("Event")]
    public UnityEvent fadeInAnim, fadeOutAnim;
    public UnityEvent<Color> changeColorTransition;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (Player != null) DontDestroyOnLoad(Player);
            if (ManagerObj != null) DontDestroyOnLoad(ManagerObj);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region SceneLogic

    async void LoadExpedition()
    {
        fadeInAnim.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        if (_sceneLoader != null)
            _sceneLoader.LoadScene(2);
        else
            SceneManager.LoadScene(ExpeditionPath);

        //genManager.gameObject.SetActive(true);
        
        await UniTask.WaitUntil(() => ExpeditionManager.Instance != null);
        ExpeditionManager.Instance.NewRun();
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        fadeOutAnim.Invoke();
    }

    IEnumerator LoadLobby()
    {
        fadeInAnim.Invoke();
        yield return new WaitForSeconds(1);

        if (_sceneLoader != null)
        {
            _sceneLoader.gameObject.SetActive(true);
            _sceneLoader.LoadScene(1);
        }
        else
            SceneManager.LoadScene(LobbyPath);

        //genManager.gameObject.SetActive(false);
        genManager.Clear();
        
        yield return new WaitForSeconds(1f);
        fadeOutAnim.Invoke();
    }

    async void LoadMenu()
    {
        fadeInAnim.Invoke();

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        Debug.Log("quit check");

        if (_sceneLoader != null)
        {
            Debug.Log("scene loader");
            _sceneLoader.LoadScene(0);
        }
        else
        {
            Debug.Log("scene load direct");
            SceneManager.LoadScene("UI Main Menu");
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        fadeOutAnim.Invoke();
    }

    public void StartExpedition()
    {
        Debug.Log("Starting new run...");
        //PlayerController.Instance.PlayerProgressData.SaveClaimReward();
        LoadExpedition();
    }

    public void StartLobby()
    {
        StartCoroutine(LoadLobby());
    }

    public void QuitMenu()
    {
        //Debug.Log("fade in plz ?");
        LoadMenu();
    }

    #endregion

    // Editor-only methods to pick scene paths from file explorer
#if UNITY_EDITOR
    [Button("Choose Expedition Scene")]
    private void ChooseExpeditionScene()
    {
        ExpeditionPath = EditorUtility.OpenFilePanel("Select Expedition Scene", "Assets/Scenes", "unity");
        if (!string.IsNullOrEmpty(ExpeditionPath))
        {
            ExpeditionPath = FileUtil.GetProjectRelativePath(ExpeditionPath);
        }
    }

    [Button("Choose Lobby Scene")]
    private void ChooseLobbyScene()
    {
        LobbyPath = EditorUtility.OpenFilePanel("Select Lobby Scene", "Assets/Scenes", "unity");
        if (!string.IsNullOrEmpty(LobbyPath))
        {
            LobbyPath = FileUtil.GetProjectRelativePath(LobbyPath);
        }
    }

    public void FadeIn()
    {
        PlayerController.Instance._UIContainer.FadeIn();
    }

    public void FadeOut()
    {
        PlayerController.Instance._UIContainer.FadeOut();
    }
#endif
}
