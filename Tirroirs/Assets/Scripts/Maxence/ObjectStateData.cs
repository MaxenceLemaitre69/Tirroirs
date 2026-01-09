using UnityEngine;

[CreateAssetMenu(fileName = "ObjectStateData", menuName = "Game/Object State Data")]
public class ObjectStateData : ScriptableObject
{
    [Header("Prompt")]
    [TextArea(2,4)] public string hoverPromptState1 = "Appuyez sur E";
    [TextArea(2,4)] public string hoverPromptState2 = "Appuyez sur E";

    [Header("Dialogue")]
    public DialogueData dialogueState1;
    public DialogueData dialogueState2;

    [Header("Notes du carnet")]
    [TextArea(5, 12)] public string noteState1;
    [TextArea(5, 12)] public string noteState2;
}

