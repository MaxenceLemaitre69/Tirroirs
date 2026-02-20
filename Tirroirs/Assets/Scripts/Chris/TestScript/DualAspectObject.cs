using UnityEngine;
using System.Collections;

public class DualAspectObject : MonoBehaviour
{
    [Header("Données (SO)")]
    public ObjectStateData data;

    [Header("Révélation d'objet(s)")]
    [Tooltip("Objet à révéler quand on interagit (doit être désactivé au départ).")]
    public GameObject objectToReveal;

    [Tooltip("Si true, on active aussi tous les enfants du parent (utile si ton objet est un groupe).")]
    public bool revealChildrenToo = false;

    [Tooltip("Si true, ne révèle l'objet qu'une seule fois.")]
    public bool revealOnlyOnce = true;

    [Header("Comportement à l'interaction")]
    public bool addNoteOnInteract = true;
    public bool playDialogueOnInteract = true;

    [Header("Verrouillage")]
    [Tooltip("Si true, on ne peut interagir qu'une seule fois avec CET objet.")]
    public bool interactOnlyOnce = true;

    private bool alreadyInteracted = false;

    private Interact inter;
    private DialogueUI dialogueUI;

    void Awake()
    {
        inter = Object.FindFirstObjectByType<Interact>();
        dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
    }

    // ====== messages envoyés par ton Interact ======

    void Hovering(Vector3 hitPoint)
    {
        if (DialogueUI.AnyDialoguePlaying) return;
        if (inter == null || data == null) return;

        if (interactOnlyOnce && alreadyInteracted)
        {
            inter.message = ""; // ou "Rien de nouveau"
            return;
        }

        inter.message = data.hoverPromptState1;
    }

    void UnHover()
    {
        if (inter == null) return;
        inter.message = "";
    }

    void Interacting()
    {
        if (DialogueUI.AnyDialoguePlaying) return;

        if (interactOnlyOnce && alreadyInteracted)
            return;

        alreadyInteracted = true;

        // 1) Révéler l’objet caché
        RevealObject();

        // 2) Note carnet
        if (addNoteOnInteract && data != null && NotebookManager.Instance != null)
        {
            if (!string.IsNullOrWhiteSpace(data.noteState1))
                NotebookManager.Instance.AddNote(data.noteState1);
        }

        // 3) Dialogue
        if (playDialogueOnInteract && data != null && dialogueUI != null)
        {
            if (data.dialogueState1 != null)
                StartCoroutine(dialogueUI.PlayDialogue(data.dialogueState1));
        }

        // 4) Nettoyer le prompt
        if (inter != null) inter.message = "";
    }

    void RevealObject()
    {
        if (objectToReveal == null) return;

        if (revealOnlyOnce && objectToReveal.activeSelf)
            return;

        objectToReveal.SetActive(true);

        if (revealChildrenToo)
        {
            for (int i = 0; i < objectToReveal.transform.childCount; i++)
                objectToReveal.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
