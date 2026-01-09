using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DualAspectObject : MonoBehaviour
{
    [Header("Données (SO)")]
    public ObjectStateData data;

    [Header("Visuels en scène")]
    public GameObject aspect1Root;
    public GameObject aspect2Root;

    [Header("Etat")]
    [SerializeField] private bool isState2 = false;

    [Header("Réactions en chaîne")]
    public List<DualAspectObject> targetsToSwitchToState2 = new List<DualAspectObject>();

    [Header("Comportement à l'interaction")]
    public bool switchSelfToState2 = false;
    public bool addNoteOnInteract = true;
    public bool playDialogueOnInteract = true;

    //  Anti re-interaction par état
    [Header("Verrouillage")]
    [SerializeField] private bool usedState1 = false;
    [SerializeField] private bool usedState2 = false;

    private Interact inter;
    private DialogueUI dialogueUI;

    void Awake()
    {
        inter = Object.FindFirstObjectByType<Interact>();
        dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        ApplyVisualState();
    }

    void ApplyVisualState()
    {
        if (aspect1Root != null) aspect1Root.SetActive(!isState2);
        if (aspect2Root != null) aspect2Root.SetActive(isState2);
    }

    public void SetState2()
    {
        if (isState2) return;
        isState2 = true;
        ApplyVisualState();
    }

    public void SetState1()
    {
        if (!isState2) return;
        isState2 = false;
        ApplyVisualState();
    }

    bool IsCurrentStateAlreadyUsed()
    {
        return isState2 ? usedState2 : usedState1;
    }

    void MarkCurrentStateAsUsed()
    {
        if (isState2) usedState2 = true;
        else usedState1 = true;
    }

    // ====== messages envoyés par ton Interact ======

    void Hovering(Vector3 hitPoint)
    {
        if (DialogueUI.AnyDialoguePlaying) return;
        if (inter == null || data == null) return;

        //  Si déjà utilisé dans cet état, on ne montre pas "Press E"
        if (IsCurrentStateAlreadyUsed())
        {
            inter.message = ""; // ou "Rien de nouveau"
            return;
        }

        inter.message = isState2 ? data.hoverPromptState2 : data.hoverPromptState1;
    }

    void UnHover()
    {
        if (inter == null) return;
        inter.message = "";
    }

    void Interacting()
    {
        if (DialogueUI.AnyDialoguePlaying) return;

        //  Bloque si déjà interagi dans cet état
        if (IsCurrentStateAlreadyUsed())
            return;

        // Marque comme utilisé pour cet état
        MarkCurrentStateAsUsed();

        // 1) changer soi-même si demandé
        if (switchSelfToState2)
            SetState2();

        // 2) changer les cibles
        for (int i = 0; i < targetsToSwitchToState2.Count; i++)
        {
            var target = targetsToSwitchToState2[i];
            if (target != null)
                target.SetState2();
        }

        // 3) Note carnet selon état actuel (après switchSelf si tu veux)
        if (addNoteOnInteract && data != null && NotebookManager.Instance != null)
        {
            string note = isState2 ? data.noteState2 : data.noteState1;
            NotebookManager.Instance.AddNote(note);
        }

        // 4) Dialogue selon état via DialogueUI + DialogueData
        if (playDialogueOnInteract && data != null && dialogueUI != null)
        {
            DialogueData dlg = isState2 ? data.dialogueState2 : data.dialogueState1;
            if (dlg != null)
                StartCoroutine(dialogueUI.PlayDialogue(dlg));
        }
    }
}


