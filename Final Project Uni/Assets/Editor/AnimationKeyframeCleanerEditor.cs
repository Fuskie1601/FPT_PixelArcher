#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class AnimationKeyframeCleanerEditor : EditorWindow
{
    public Animator targetAnimator;
    public List<AnimationClip> animationClips = new List<AnimationClip>();

    [MenuItem("Tools/AnimationKeyframeCleanerEditor")]
    public static void ShowWindow()
    {
        GetWindow<AnimationKeyframeCleanerEditor>("AnimationKeyframeCleanerEditor");
    }

    private SerializedObject serializedObject;
    private SerializedProperty animationClipsProperty;
    private SerializedProperty targetAnimatorProperty;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        animationClipsProperty = serializedObject.FindProperty("animationClips");
        targetAnimatorProperty = serializedObject.FindProperty("targetAnimator");
    }

    public void OnGUI()
    {
        serializedObject.Update();

        // Input field for Animator
        EditorGUILayout.PropertyField(targetAnimatorProperty, new GUIContent("Target Animator"));
        EditorGUILayout.PropertyField(animationClipsProperty, new GUIContent("Animation Clips"), true);

        serializedObject.ApplyModifiedProperties();

        // Guard clause for necessary inputs
        if (targetAnimator == null || animationClips.Count == 0)
        {
            GUILayout.Label("Please assign an Animator and Animation Clips.");
            return;
        }

        // Button to remove missing keyframes
        if (GUILayout.Button("Remove Missing Keyframes"))
            ConfirmRemoval("missing keyframes", RemoveMissingKeyframes);

        // Button to remove keyframes with only start and end frame having the same value
        if (GUILayout.Button("Remove Duplicate Start and End Keyframes"))
            ConfirmRemoval("duplicate start and end keyframes", RemoveDuplicateStartEndKeyframes);
    }

    private void ConfirmRemoval(string actionDescription, System.Action removalMethod)
    {
        bool userConfirmed = EditorUtility.DisplayDialog(
            "Confirm Action",
            $"This will permanently remove {actionDescription} and cannot be undone. Are you sure you want to proceed?",
            "I know what I'm doing", // Accept button
            "Cancel" // Cancel button
        );

        if (userConfirmed)
        {
            removalMethod.Invoke(); // Invoke the removal method
        }
    }

    [Button]
    public void RemoveMissingKeyframes()
    {
        if (animationClips == null || animationClips.Count == 0 || targetAnimator == null)
        {
            Debug.LogWarning("Please ensure Animator and animation clips are assigned.");
            return;
        }

        foreach (AnimationClip clip in animationClips)
        {
            RemoveMissingKeyframesFromClip(clip);
        }

        Debug.Log("Completed removing missing keyframes.");
    }

    public void RemoveMissingKeyframesFromClip(AnimationClip clip)
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in bindings)
        {
            // Check if the binding is missing based on the target Animator's hierarchy
            if (IsBindingMissing(binding))
            {
                // Remove the curve associated with the missing binding
                AnimationUtility.SetEditorCurve(clip, binding, null);
                Debug.Log($"Removed missing keyframe from: {binding.path}, property: {binding.propertyName}");
            }
        }
    }

    public bool IsBindingMissing(EditorCurveBinding binding)
    {
        // Get the GameObject associated with the Animator
        GameObject targetGameObject = targetAnimator.gameObject;

        // Get the Transform corresponding to the binding's path
        Transform targetTransform = targetGameObject.transform.Find(binding.path);

        // Return true if the target object is missing
        return targetTransform == null;
    }

    [Button]
    public void RemoveDuplicateStartEndKeyframes()
    {
        if (animationClips == null || animationClips.Count == 0 || targetAnimator == null)
        {
            Debug.LogWarning("Please ensure Animator and animation clips are assigned.");
            return;
        }

        foreach (AnimationClip clip in animationClips)
        {
            RemoveDuplicateStartEndKeyframesFromClip(clip);
        }

        Debug.Log("Completed removing duplicate start and end keyframes.");
    }

    public void RemoveDuplicateStartEndKeyframesFromClip(AnimationClip clip)
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in bindings)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve == null || curve.length < 2)
                continue; // Skip if there are no keyframes or less than 2

            // Get the first and last keyframes
            Keyframe startKeyframe = curve[0];
            Keyframe endKeyframe = curve[curve.length - 1];

            // Check if they have the same value
            if (startKeyframe.value == endKeyframe.value)
            {
                // Remove the entire curve associated with the binding
                AnimationUtility.SetEditorCurve(clip, binding, null);
                Debug.Log($"Removed curve from: {binding.path}, property: {binding.propertyName} because start and end keyframes are equal.");
            }
        }
    }
}

#endif
