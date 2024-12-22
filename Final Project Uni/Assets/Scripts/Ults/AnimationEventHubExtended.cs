using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[Serializable]
public struct ParameterSet
{
    public List<int> intValue;
    public List<bool> boolValue;
    public List<float> floatValue;
    public List<string> stringValue;
}

[Serializable]
public struct eventwithParam
{
    public UnityEvent<ParameterSet> Event;
    public ParameterSet param;
}

public class AnimationEventHubExtended : SerializedMonoBehaviour
{
    [FoldoutGroup("Default")]
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<string, UnityEvent> eventDic;

    [FoldoutGroup("Dynamic Param")]
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<string, eventwithParam> eventDicDynamic;

    [FoldoutGroup("Default")]
    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
    public void CallEventByID(string idInput)
    {
        if (eventDic.ContainsKey(idInput))
            eventDic[idInput].Invoke();
        else
            Debug.LogWarning($"No event found with AnimEventID: {idInput}");
    }

    [FoldoutGroup("Dynamic Param")]
    [Button(ButtonSizes.Large), GUIColor(1f, 0.6f, 0.4f)]
    public void CallEventWithParamsByID(string idInput)
    {
        if (eventDicDynamic.ContainsKey(idInput))
            eventDicDynamic[idInput].Event.Invoke(eventDicDynamic[idInput].param);
        else
            Debug.LogWarning($"No event found with AnimEventID and Params: {idInput}");
    }

    public void Test(ParameterSet param)
    {
        Debug.Log("yayyyy " + param.intValue[0]);
    }
}
