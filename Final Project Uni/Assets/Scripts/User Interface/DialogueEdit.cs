using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEdit : MonoBehaviour
{
    public List<Dialogue> dialogues = new List<Dialogue>(); // List of dialogues
    public int requireKnowledgeLevel;
    public float fontSize = -1;
    public bool Mask;

    private int currentDialogueIndex = 0;
    private CancellationTokenSource cancellationTokenSource;

    public void ReadDialogue()
    {
        AssignNextButtonListener(); // Assign click listener
        currentDialogueIndex = 0; // Start from the first dialogue
        
        ToggleDialogue(); // Show the dialogue UI
        EditText(currentDialogueIndex);
        DisplayDialogueWithAutoAdvance().Forget(); // Start auto-advancing
    }

    private void AssignNextButtonListener()
    {
        if (PlayerController.Instance.dialogUI.DialogueNext == null) return;
        PlayerController.Instance.dialogUI.DialogueNext.onClick.AddListener(NextDialogue);
    }

    private void RemoveNextButtonListener()
    {
        if (PlayerController.Instance.dialogUI.DialogueNext != null)
            PlayerController.Instance.dialogUI.DialogueNext.onClick.RemoveListener(NextDialogue);
    }

    public void NextDialogue()
    {
        // Cancel the auto-advance for the current dialogue if it’s running
        cancellationTokenSource?.Cancel();

        // Move to the next dialogue or close if it's the last
        currentDialogueIndex++;
        if (currentDialogueIndex >= dialogues.Count)
        {
            ForcedDownDialogue(); // Close the dialogue UI
        }
        else
        {
            EditText(currentDialogueIndex); // Display the next dialogue
            DisplayDialogueWithAutoAdvance().Forget(); // Auto-advance for the next dialogue
        }
    }

    private async UniTaskVoid DisplayDialogueWithAutoAdvance()
    {
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogues.Count)
            return;

        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        // Set up a new cancellation token for this dialogue
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            // Wait for the specified read time or cancellation from a button click
            await UniTask.Delay(TimeSpan.FromSeconds(currentDialogue.readTime), cancellationToken: cancellationTokenSource.Token);
            NextDialogue(); // Automatically move to the next dialogue if not canceled
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation gracefully if the dialogue is skipped
        }
    }

    public void EditText(int index)
    {
        // Toggle Mask visibility
        PlayerController.Instance.dialogUI.SetActiveMask(Mask);
        
        if (index < 0 || index >= dialogues.Count) return;

        Dialogue dialogue = dialogues[index];

        // Update dialogue text and font size
        PlayerController.Instance.dialogUI.ChangeText(dialogue.text, fontSize);

        // Check Player Knowledge Level to read with original or hidden font
        if (requireKnowledgeLevel <= PlayerController.Instance._stats.knowledgeLevel)
            PlayerController.Instance.dialogUI.ShowOriginalText();
        else
            PlayerController.Instance.dialogUI.ChangeTextFontHidden();
    }

    public void ToggleDialogue()
    {
        PlayerController.Instance.dialogUI.toggleScale.ScaleToggle();
    }

    public void ForcedDownDialogue()
    {
        // Remove the listener when the dialogue is closed
        RemoveNextButtonListener();
        PlayerController.Instance.dialogUI.toggleScale.ForceScaleDown();
    }

    public void ToggleMask(bool toggle)
    {
        Mask = toggle;
    }
}
