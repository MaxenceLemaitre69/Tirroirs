using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [TextArea] public string hoverPrompt = "Appuyez sur E pour utiliser la porte";
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
        CaseFlowManager.Instance?.OnDoorInteracted();
    }
}