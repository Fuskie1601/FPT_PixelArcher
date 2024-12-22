using UnityEngine;

public enum EventType
{
    None, SkillChoose
}
[CreateAssetMenu(fileName = "Floor", menuName = "Gen/ExpeditionEvent")]
public class ExpeditionEvent : ScriptableObject
{
    public string name;
    public EventType eventType;

}
