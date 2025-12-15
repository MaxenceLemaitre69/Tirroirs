using UnityEngine;
using System.Collections.Generic;

public class ObjectStateController : MonoBehaviour
{
    public ObjectStateData data;
    public Transform spawnPoint;

    [Header("Etat")]
    [SerializeField] private bool isAfterEvent = false;

    [Header("Réactions en chaîne")]
    [Tooltip("Quand cet objet est interagi (ou quand TriggerEvent est appelé), ces objets passent en état 2.")]
    public List<ObjectStateController> switchTheseToAfterEvent = new List<ObjectStateController>();

    [Tooltip("Si true, cet objet passe aussi en état 2 quand TriggerEvent est appelé.")]
    public bool switchSelfToAfterEvent = false;

    [Tooltip("Si true, ajoute une note au carnet quand TriggerEvent est appelé.")]
    public bool addNoteOnTrigger = false;

    private GameObject currentInstance;

    private void Start()
    {
        SpawnFromCurrentState();
    }

    void SpawnFromCurrentState()
    {
        if (data == null)
        {
            Debug.LogWarning($"{name} : ObjectStateData manquant.");
            return;
        }

        // Nettoie au cas où
        if (currentInstance != null) Destroy(currentInstance);

        var prefab = isAfterEvent ? data.afterEventPrefab : data.initialPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"{name} : prefab manquant pour l'état {(isAfterEvent ? "2" : "1")}.");
            return;
        }

        currentInstance = Instantiate(
            prefab,
            spawnPoint != null ? spawnPoint.position : transform.position,
            spawnPoint != null ? spawnPoint.rotation : transform.rotation,
            transform
        );
    }

    public void SwitchToAfterEvent()
    {
        if (isAfterEvent) return;
        isAfterEvent = true;
        SpawnFromCurrentState();
    }

    public void SwitchToInitial()
    {
        if (!isAfterEvent) return;
        isAfterEvent = false;
        SpawnFromCurrentState();
    }

    // ✅ À appeler depuis SimpleInteractable.onInteractUnity
    public void TriggerEvent()
    {
        // Optionnel : se switch soi-même
        if (switchSelfToAfterEvent)
            SwitchToAfterEvent();

        // Switch les autres objets
        for (int i = 0; i < switchTheseToAfterEvent.Count; i++)
        {
            var target = switchTheseToAfterEvent[i];
            if (target != null)
                target.SwitchToAfterEvent();
        }

        // Optionnel : ajouter une note au carnet (selon l'état actuel de CET objet)
        if (addNoteOnTrigger)
            GiveNoteToNotebook();
    }

    public void GiveNoteToNotebook()
    {
        if (data == null) return;

        string note = isAfterEvent ? data.noteState2 : data.noteState1;
        if (NotebookManager.Instance != null)
            NotebookManager.Instance.AddNote(note);
    }

    // (Optionnel) si tu veux pouvoir lire l'état ailleurs
    public bool IsAfterEvent => isAfterEvent;
}
