using UnityEngine;

public class PolicemanInteractable : MonoBehaviour
{
    [TextArea] public string hoverPrompt = "Appuyez sur E pour parler";
    public DialogueData startDialogue;
    public DialogueData answerDialogue;

    private Interact inter;

    void Awake()
    {
        inter = Object.FindFirstObjectByType<Interact>();
    }

    void Hovering(Vector3 hitPoint)
    {
        if (DialogueUI.AnyDialoguePlaying) return;
        if (inter != null) inter.message = hoverPrompt;
    }

    void UnHover()
    {
        if (inter != null) inter.message = "";
    }

    void Interacting()
    {
        if (CaseFlowManager.Instance != null)
            CaseFlowManager.Instance.OnPolicemanInteracted(startDialogue, answerDialogue);
    }
}