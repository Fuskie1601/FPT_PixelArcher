namespace DAM
{
    using UnityEngine;

    public static class DebugHelper
    {
        public static void Print(MonoBehaviour script , string message)
        {
            string objectName = $"<color=#00FF00>{script.gameObject.name}</color>";
            string scriptName = $"<color=#FFA500>{script.GetType().Name}</color>";
            Debug.Log($"{objectName} - {scriptName}: {message}");
        }
    }

}
