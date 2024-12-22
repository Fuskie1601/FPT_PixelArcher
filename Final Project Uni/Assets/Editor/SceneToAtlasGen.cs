using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

public class CreateAndPopulateAtlas : EditorWindow
{
    private string targetFolderPath = "Assets";
    private string atlasName = "NewSpriteAtlas";
    private List<Sprite> spriteList = new List<Sprite>();
    private ReorderableList reorderableList;

    private bool duplicateFound = false; // Track duplicate state
    private SpriteAtlas duplicateAtlas = null; // Store duplicate atlas

    [MenuItem("Tools/Create New Sprite Atlas")]
    public static void ShowWindow()
    {
        GetWindow<CreateAndPopulateAtlas>("Create Sprite Atlas");
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(spriteList, typeof(Sprite), true, true, true, true)
        {
            drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Collected Sprites"); },
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                spriteList[index] = (Sprite)EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    spriteList[index],
                    typeof(Sprite),
                    false);
            },
            onAddCallback = list => { spriteList.Add(null); },
            onRemoveCallback = list => { spriteList.RemoveAt(list.index); }
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Create and Populate Sprite Atlas", EditorStyles.boldLabel);

        // Folder path input
        GUILayout.Label("Target Folder Path:");
        GUILayout.BeginHorizontal();
        targetFolderPath = EditorGUILayout.TextField(targetFolderPath);
        if (GUILayout.Button("Choose Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                targetFolderPath = GetRelativeAssetPath(path);
            }
        }
        GUILayout.EndHorizontal();

        // Atlas name input
        GUILayout.Label("Sprite Atlas Name:");
        atlasName = EditorGUILayout.TextField(atlasName);

        // Warning message if duplicate found
        if (duplicateFound)
        {
            EditorGUILayout.HelpBox("Duplicate found! Do you want to destroy the old atlas and create a new one that includes its data?", MessageType.Warning);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Old Data"))
            {
                ReplaceAtlas(false);
            }
            if (GUILayout.Button("Combine Data"))
            {
                ReplaceAtlas(true);
            }
            if (GUILayout.Button("Cancel"))
            {
                duplicateFound = false; // Reset duplicate state
            }
            GUILayout.EndHorizontal();
        }

        // Buttons
        if (GUILayout.Button("Add Sprites from Scene"))
        {
            AddSpritesFromScene();
        }

        if (GUILayout.Button("Create Sprite Atlas"))
        {
            CheckAndCreateAtlas();
        }

        GUILayout.Label("Manage Sprites:", EditorStyles.boldLabel);
        reorderableList.DoLayoutList();
    }

    private void AddSpritesFromScene()
    {
        HashSet<Sprite> usedSprites = new HashSet<Sprite>();

        // Collect sprites from SpriteRenderers
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                usedSprites.Add(spriteRenderer.sprite);
            }

            // Collect sprites from UI Images
            var image = obj.GetComponent<UnityEngine.UI.Image>();
            if (image != null && image.sprite != null)
            {
                usedSprites.Add(image.sprite);
            }
        }

        foreach (var sprite in usedSprites)
        {
            if (!spriteList.Contains(sprite))
            {
                spriteList.Add(sprite);
            }
        }

        Debug.Log($"Collected {usedSprites.Count} sprites from the scene.");
    }


    private void CheckAndCreateAtlas()
    {
        string atlasPath = $"{targetFolderPath}/{atlasName}.spriteatlas";
        duplicateAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

        if (duplicateAtlas != null)
        {
            duplicateFound = true;
        }
        else
        {
            CreateNewAtlas();
        }
    }

    private void ReplaceAtlas(bool combineData)
    {
        if (combineData)
        {
            // Merge old sprites into the new list
            Object[] packedObjects = duplicateAtlas.GetPackables();
            foreach (Object obj in packedObjects)
            {
                if (obj is Sprite sprite && !spriteList.Contains(sprite))
                {
                    spriteList.Add(sprite);
                }
            }
        }

        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(duplicateAtlas));
        CreateNewAtlas();
        duplicateFound = false; // Reset duplicate state
    }

    private void CreateNewAtlas()
    {
        if (string.IsNullOrEmpty(targetFolderPath) || !AssetDatabase.IsValidFolder(targetFolderPath))
        {
            Debug.LogError("Invalid folder path: " + targetFolderPath);
            return;
        }

        string atlasPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolderPath}/{atlasName}.spriteatlas");
        SpriteAtlas newAtlas = new SpriteAtlas();
        AssetDatabase.CreateAsset(newAtlas, atlasPath);

        if (spriteList.Count > 0)
        {
            SpriteAtlasExtensions.Add(newAtlas, spriteList.ToArray());
        }

        SpriteAtlasUtility.PackAtlases(new[] { newAtlas }, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Successfully created new Sprite Atlas: {atlasName}.");
    }

    private string GetRelativeAssetPath(string absolutePath)
    {
        if (absolutePath.StartsWith(Application.dataPath))
        {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }
        return absolutePath;
    }
}
