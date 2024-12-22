using UnityEngine;
using UnityEngine.UI;

namespace PA{

     public static class QualitySettings
    {
        public static void SetQualityLevel(int qualityIndex)
        {
            UnityEngine.QualitySettings.SetQualityLevel(qualityIndex);
        }

        public static void SetVSync(bool isEnabled)
        {
            UnityEngine.QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        }

        public static void SetAntiAliasing(bool isEnabled)
        {
            UnityEngine.QualitySettings.antiAliasing = isEnabled ? 8 : 0;
        }

        public static void SetShadows(bool isEnabled)
        {
            UnityEngine.QualitySettings.shadows = isEnabled ? ShadowQuality.All : ShadowQuality.Disable;
        }

        public static void SetBloom(bool isEnabled)
        {
            //UnityEngine.QualitySettings.bloom = isEnabled;
        }
        
        public static void SetFPS(int value)
        {
            if (value == 0)
            {
                Application.targetFrameRate = 30;
            }
            else if (value == 1)
            {
                Application.targetFrameRate = 60;
            }
            else if (value == 2)
            {
                Application.targetFrameRate = 90;
            }
        }

        public static void ApplySettings(int graphicsQuality, bool isVSyncEnabled, bool isAntiAliasingEnabled, bool isShadowsEnabled, bool isBloomEnabled)
        {
            SetQualityLevel(graphicsQuality);
            SetVSync(isVSyncEnabled);
            SetAntiAliasing(isAntiAliasingEnabled);
            SetShadows(isShadowsEnabled);
            SetBloom(isBloomEnabled);
        }
    }
}