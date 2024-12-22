using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    [SerializeField]
    private float scaleDuration = 0.3f; // Duration for scaling

    [SerializeField]
    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Smooth scale curve

    public bool IsScaledUp; // Track whether the object is scaled up

    public event Action OnScaleDownComplete; // Event triggered when scale down is complete
    public event Action OnScaleUpComplete; // Event triggered when scale up is complete

    // Method to scale up or down based on the current state
    [Button]
    public void ScaleToggle()
    {
        if (IsScaledUp)
            ScaleDownAsync().Forget();
        else
            ScaleUpAsync().Forget();
    }

    // Force scale down (used when the player leaves the range)
    public void ForceScaleDown()
    {
        if (IsScaledUp) 
            ScaleDownAsync().Forget();
    }

    // Scale up the dialog
    public async UniTask ScaleUpAsync()
    {
        await ScaleAsync(0, 1); // Scale from 0 to 1 (appear)
        OnScaleUpComplete?.Invoke(); // Trigger event when complete
        IsScaledUp = true; // Mark as scaled up
    }

    // Scale down the dialog
    public async UniTask ScaleDownAsync()
    {
        await ScaleAsync(1, 0); // Scale from 1 to 0 (disappear)
        OnScaleDownComplete?.Invoke(); // Trigger event when complete
        IsScaledUp = false; // Mark as scaled down
    }

    // Helper method to handle scaling animation
    private async UniTask ScaleAsync(float startScale, float endScale)
    {
        float elapsedTime = 0f;
        Vector3 startScaleVector = Vector3.one * startScale;
        Vector3 endScaleVector = Vector3.one * endScale;

        while (elapsedTime < scaleDuration)
        {
            float t = elapsedTime / scaleDuration;
            float curveValue = scaleCurve.Evaluate(t);
            transform.localScale = Vector3.Lerp(startScaleVector, endScaleVector, curveValue);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }

        transform.localScale = endScaleVector;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero; // Start with scale at 0 when enabled
    }
}
