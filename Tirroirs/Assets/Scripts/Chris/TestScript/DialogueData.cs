using UnityEngine;

[CreateAssetMenu(menuName = "Game/Dialogue Sequence")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea] public string text;
        public AudioClip audio;
    }

    public DialogueLine[] lines;
}