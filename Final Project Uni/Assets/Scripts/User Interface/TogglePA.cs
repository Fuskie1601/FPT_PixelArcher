using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA{

    public class TogglePA : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Toggle uiToggle;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private void Start()
        {
            if (uiToggle == null)
            {
                uiToggle = GetComponent<UnityEngine.UI.Toggle>();
            }

            if (uiToggle != null)
            {
                uiToggle.onValueChanged.AddListener(OnToggleValueChanged);
                UpdateToggleSprite(uiToggle.isOn);
            }
            else
            {
                Debug.LogError("Toggle component not found!");
            }
        }

        private void OnToggleValueChanged(bool isOn)
        {
            UpdateToggleSprite(isOn);
        }

        private void UpdateToggleSprite(bool isOn)
        {
            if (uiToggle.targetGraphic is UnityEngine.UI.Image toggleImage)
            {
                toggleImage.sprite = isOn ? onSprite : offSprite;
            }
        }

        private void OnDestroy()
        {
            if (uiToggle != null)
            {
                uiToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }
    }

}