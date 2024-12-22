using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomEditorTools;
using UnityEngine.Events;

[CustomEditor(typeof(AchievementManager))]
public class AchievementManagerEditor : Editor
{
    AchievementManager MyTarget;
    int SelectedTab;
    string searchQuery = ""; 

    #region Styles
    GUIStyle ManageBackground = new GUIStyle();
    GUIStyle Border = new GUIStyle();
    GUIStyle ManageInsideBackground = new GUIStyle();
    GUIStyle RowButton = new GUIStyle();
    GUIStyle ProgressBar = new GUIStyle();
    
    [SerializeField] public UnityEvent<AchievementInfromation> completeAchievement;
    #endregion

    List<bool> Hidden = new List<bool>();
    bool HideAll = true;

    Vector2 scrollPos; // For scroll view

    void Awake()
    {
        MyTarget = (AchievementManager)target;
        MyTarget.LoadAchievementState();

        for (int i = 0; i < MyTarget.AchievementList.Count; i++)
        {
            Hidden.Add(false);
        }

        bool LightMode = !EditorGUIUtility.isProSkin;
        #region Editor Styles
        ManageBackground.normal.background = CET.MakeEditorBackgroundColor(LightMode ?
            new Color(0.805f, 0.805f, 0.813f) :
            new Color(0.4f, 0.4f, 0.4f)
        );
        ManageBackground.padding = new RectOffset(5, 5, 5, 5);

        ManageInsideBackground.normal.background = CET.MakeEditorBackgroundColor(LightMode ?
            new Color(0.629f, 0.629f, 0.629f) :
            new Color(0.5f, 0.5f, 0.5f)
        );
        ManageInsideBackground.padding = new RectOffset(0, 0, 5, 5);

        RowButton.padding = new RectOffset();
        RowButton.fixedHeight = 20;
        RowButton.fixedWidth = 20;
        RowButton.margin = new RectOffset(0, 5, 0, 0);

        ProgressBar.normal.background = CET.MakeEditorBackgroundColor(Color.blue);
        ProgressBar.margin = new RectOffset(0, 5, 0, 0);

        Border.normal.background = CET.MakeEditorBackgroundColor(new Color(0.2f, 0.2f, 0.2f));
        Border.padding = new RectOffset(3, 3, 3, 3);
        Border.margin = new RectOffset(0, 0, 0, 10);
        #endregion
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(10);
        SelectedTab = GUILayout.Toolbar(SelectedTab, new string[] { "Settings", "Achievement List" });
        GUILayout.Space(10);

        switch (SelectedTab)
        {
            case 0:
                DrawSettings();
                break;
            case 1:
                DrawAchievementList();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawAchievementList()
    {
        // Search bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchQuery = EditorGUILayout.TextField(searchQuery);
        GUILayout.EndHorizontal();

        // Button to toggle hiding all
        if (GUILayout.Button(HideAll ? "Show All" : "Hide All", GUILayout.Width(70)))
        {
            for (int i = 0; i < Hidden.Count; i++)
            {
                Hidden[i] = HideAll;
            }
            HideAll = !HideAll;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); // Start scroll view

        for (int i = 0; i < MyTarget.AchievementList.Count; i++)
        {
            var achievement = MyTarget.AchievementList[i];

            // Check if the achievement matches the search query
            if (!string.IsNullOrEmpty(searchQuery) &&
                !achievement.DisplayName.ToLower().Contains(searchQuery.ToLower()) &&
                !achievement.Key.ToLower().Contains(searchQuery.ToLower()))
            {
                continue; // Skip if it doesn't match
            }

            DrawAchievement(serializedObject.FindProperty("AchievementList").GetArrayElementAtIndex(i), i);
        }

        EditorGUILayout.EndScrollView(); // End scroll view

        GUILayout.Space(10);

        if (GUILayout.Button("Add"))
        {
            MyTarget.AchievementList.Add(new AchievementInfromation());
            MyTarget.States.Add(new AchievementState());
            MyTarget.SaveAchievementState();
            Hidden.Add(false);
        }

        CET.HorizontalLine();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFinalAchievement"));
        GUILayout.Label("Define an achievement which will be unlocked once all others have been completed");
        if (MyTarget.UseFinalAchievement)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FinalAchievementKey"));
        }
        CET.HorizontalLine();
    }

    public void DrawAchievement(SerializedProperty Achievement, int Index)
    {
        GUILayout.BeginVertical(Border);
        Hidden[Index] = EditorGUILayout.Foldout(Hidden[Index], $"({Index}) {MyTarget.AchievementList[Index].DisplayName}", true);

        if (Hidden[Index])
        {
            GUILayout.BeginVertical(ManageBackground);

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("Key"));
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("DisplayName"));
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("Description"));

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("LockedIcon"));
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("LockOverlay"));
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("AchievedIcon"));
            GUILayout.Space(10);

            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("Spoiler"));
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("Progression"));
            if (Achievement.FindPropertyRelative("Progression").boolValue)
            {
                EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("ProgressGoal"));
                EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("NotificationFrequency"));
                EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("ProgressSuffix"));
            }
            EditorGUILayout.PropertyField(Achievement.FindPropertyRelative("CompleteEvent"));

            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();
    }

    public void DrawSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOnScreen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayAchievements"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SpoilerAchievementMessage"));
        if (MyTarget.DisplayAchievements)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StackLocation"));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowExactProgress"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSave"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProgressMadeSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AchievedSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("completeAchievement"));
        CET.HorizontalLine();

        GUILayout.BeginVertical(ManageBackground);
        for (int i = 0; i < MyTarget.AchievementList.Count; i++)
        {
            GUILayout.BeginHorizontal(ManageInsideBackground);
            GUILayout.Label($"[{i}] {MyTarget.AchievementList[i].DisplayName} ({MyTarget.AchievementList[i].Key})", GUILayout.Width(250));

            if (MyTarget.AchievementList[i].Progression)
            {
                float progress = MyTarget.States[i].Progress / MyTarget.AchievementList[i].ProgressGoal;
                Rect progressBarRect = EditorGUILayout.BeginHorizontal(ProgressBar, GUILayout.ExpandWidth(true));
                GUILayout.Label("");
                EditorGUILayout.EndHorizontal();
                EditorGUI.ProgressBar(progressBarRect, progress, $"{MyTarget.States[i].Progress} / {MyTarget.AchievementList[i].ProgressGoal} ({progress * 100}%)");
            }
            else
            {
                GUILayout.Label(MyTarget.States[i].Achieved ? "True" : "False");
            }
            
            if (MyTarget.UseFinalAchievement && MyTarget.FinalAchievementKey.Equals(MyTarget.AchievementList[i].Key))
            {
                GUILayout.Box(Resources.Load<Texture2D>("CompleteAllIcon"), RowButton);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        GUILayout.EndVertical();
        CET.HorizontalLine();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Manual Save"))
        {
            MyTarget.SaveAchievementState();
        }
        if (GUILayout.Button("Reset All States"))
        {
            if (EditorUtility.DisplayDialog("Reset Confirmation", "Are you sure you want to reset all achievement states?", "Reset", "Cancel"))
            {
                MyTarget.ResetAchievementState();
            }
        }
        GUILayout.EndHorizontal();
        CET.HorizontalLine();
    }
}
