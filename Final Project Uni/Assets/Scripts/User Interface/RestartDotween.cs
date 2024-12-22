using DAM;
using UnityEngine;
using DG.Tweening;

public class RestartDotween : MonoBehaviour
{
    private DOTweenAnimation[] dotweenAnimations;

    private void Awake()
    {
        // Get all DOTweenAnimation components on this GameObject and its children
        dotweenAnimations = GetComponentsInChildren<DOTweenAnimation>(true);
    }

    private void OnEnable()
    {
        RestartAllTweens();
    }

    private void RestartAllTweens()
    {
        //DebugHelper.Print(this,$"dotweenAnimations: {dotweenAnimations.Length}, run"); 
        foreach (DOTweenAnimation animation in dotweenAnimations)
        {
            if (animation != null && animation.gameObject.activeInHierarchy)
            {
                animation.DORestart();
            }
        }
    }

    // This method can be called when this object or any of its parents are re-activated
    public void RestartTweensOnReactivation()
    {
        if (gameObject.activeInHierarchy)
        {
            RestartAllTweens();
            
            // Restart DOTweens for all child objects
            DOTweenAnimation[] childAnimations = GetComponentsInChildren<DOTweenAnimation>(true);
            foreach (DOTweenAnimation childAnimation in childAnimations)
            {
                if (childAnimation != null && childAnimation.gameObject.activeInHierarchy)
                {
                    childAnimation.DORestart();
                }
            }
        }
    }
}
