using UnityEngine;
using TMPro;
using UnityEngine.UI;

using System.Collections.Generic;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance;

    [Header("UI")]
    public GameObject notebookRoot;   // le panel/canvas root à activer/désactiver
    public TMP_Text noteText; // texte affiché sur la feuille
    public Button closeButton;        // optionnel

    [Header("Touche")]
    public KeyCode toggleKey = KeyCode.N;

    [Header("Bloquer contrôles pendant le carnet")]
    public GameObject controlsRoot;
    public List<string> scriptTypeNamesToDisable = new List<string>();

    private bool open = false;
    private List<MonoBehaviour> disabledDuringUI = new List<MonoBehaviour>();

    // On stocke les notes collectées (sans doublons)
    private readonly HashSet<string> collectedNotes = new HashSet<string>();
    private string lastShown = "";

    void Awake()
    {
        Instance = this;

        if (notebookRoot != null) notebookRoot.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Close);

        if (controlsRoot == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) controlsRoot = p;
            else if (Camera.main != null) controlsRoot = Camera.main.gameObject;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (open) Close();
            else Open();
        }
    }

    public void Open()
    {
        if (open) return;
        open = true;

        if (notebookRoot != null) notebookRoot.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisableControlsByName();

        // Affiche la dernière note si dispo
        if (noteText != null)
            noteText.text = string.IsNullOrEmpty(lastShown) ? "(Aucune note pour le moment)" : lastShown;
    }

    public void Close()
    {
        if (!open) return;
        open = false;

        if (notebookRoot != null) notebookRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        RestoreControls();
    }

    public void AddNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note)) return;

        // Sans doublons
        if (collectedNotes.Add(note))
        {
            lastShown = note; // on affiche la nouvelle note
        }
        else
        {
            // si déjà collectée, on peut quand même la montrer
            lastShown = note;
        }

        // Si carnet ouvert, update direct
        if (open && noteText != null)
            noteText.text = lastShown;
    }

    void DisableControlsByName()
    {
        disabledDuringUI.Clear();
        if (controlsRoot == null || scriptTypeNamesToDisable == null || scriptTypeNamesToDisable.Count == 0)
            return;

        var all = controlsRoot.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var mb in all)
        {
            if (mb == null) continue;
            string typeName = mb.GetType().Name;

            for (int i = 0; i < scriptTypeNamesToDisable.Count; i++)
            {
                if (typeName == scriptTypeNamesToDisable[i] && mb.enabled)
                {
                    mb.enabled = false;
                    disabledDuringUI.Add(mb);
                }
            }
        }
    }

    void RestoreControls()
    {
        foreach (var mb in disabledDuringUI)
        {
            if (mb != null) mb.enabled = true;
        }
        disabledDuringUI.Clear();
    }
}
