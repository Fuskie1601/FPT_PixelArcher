using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MakeTransitionUI : MonoBehaviour
{
    public GameObject maskA;
    public GameObject maskB;

    public void ConfigTwoGameObjectOfTransition(GameObject newChild, GameObject oldChild)
    {
        var tempA = maskA.transform.GetChild(0);
        var tempB = maskB.transform.GetChild(0);
        //clear parent
        // tempA.SetParent(null);
        // tempB.SetParent(null);
        // Ensure both new and old child have FixPosition component
        if (!newChild.GetComponent<FixPosition>())
        {
            newChild.AddComponent<FixPosition>();
        }

        if (!oldChild.GetComponent<FixPosition>())
        {
            oldChild.AddComponent<FixPosition>();
        }

        //false mean left to right
        //true mean right to left
        if (!toggle)
        {
            newChild.transform.SetParent(maskA.transform);
            oldChild.transform.SetParent(maskB.transform);
        }
        else
        {
            newChild.transform.SetParent(maskB.transform);
            oldChild.transform.SetParent(maskA.transform);
        }
    }


    public GameObject WipeSlider;
    public float transitionSpeed = 2000f;
    public float percentScreenEdgeOffset = 0.1f; // Thêm offset để slider đi ra ngoài màn hình
    public float duration;
    public bool toggle = false;
    public Canvas mainCanvas;
    private void SetUIInteractable(bool interactable)
    {
        if (mainCanvas != null)
        {
            CanvasGroup canvasGroup = mainCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }
    }

    public async void MakeTransition()
    {
        // Disable UI interaction at start
        SetUIInteractable(false);

        RectTransform wipeRect = WipeSlider.GetComponent<RectTransform>();
        float startX, targetX;
        float halfScreenWidth = mainCanvas.GetComponent<RectTransform>().rect.width * (1+percentScreenEdgeOffset) / 2f; // Half screen width with offset
        if (!toggle)
        {
            // Left to right
            startX = -halfScreenWidth;
            targetX = halfScreenWidth;
            toggle = true;
        }
        else
        {
            // Right to left
            startX = halfScreenWidth;
            targetX = -halfScreenWidth;
            toggle = false;
        }

        // Set initial position
        wipeRect.anchoredPosition = new Vector2(startX, wipeRect.anchoredPosition.y);

        float elapsedTime = 0f;
        duration = Mathf.Abs(targetX - startX) / transitionSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentX = Mathf.Lerp(startX, targetX, t);
            wipeRect.anchoredPosition = new Vector2(currentX, wipeRect.anchoredPosition.y);          
            await UniTask.Yield();
        }

        wipeRect.anchoredPosition = new Vector2(targetX, wipeRect.anchoredPosition.y);
        
        // Re-enable UI interaction after transition
        SetUIInteractable(true);
    }  

    public async void MakeFadeTransition()
    {
        // Disable UI interaction at the start of the transition
        SetUIInteractable(false);

        float elapsedTime = 0f;
        CanvasGroup canvasGroup = fadeBlackImage.GetComponent<CanvasGroup>();

        while (elapsedTime < duration)
        {
            // Calculate the fade progress using Lerp
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(); // Yield execution to ensure smooth frame updates
        }

        // Ensure the final alpha value is set to 1
        canvasGroup.alpha = 1f;

        // Re-enable UI interaction after transition
        SetUIInteractable(true);
    }

    public GameObject fadeBlackImage;

}