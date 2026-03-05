//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using System.Collections.Generic;
//
//public class NotebookManager : MonoBehaviour
//{
//    public static NotebookManager Instance;
//
//    [Header("UI")]
//    public GameObject notebookRoot;   // le panel/canvas root à activer/désactiver
//    public TMP_Text noteText;         // texte affiché sur la feuille
//    public Button closeButton;        // optionnel
//
//    [Header("Touche")]
//    public KeyCode toggleKey = KeyCode.N;
//
//    [Header("Bloquer contrôles pendant le carnet")]
//    public GameObject controlsRoot;
//    public List<string> scriptTypeNamesToDisable = new List<string>();
//
//    private bool open = false;
//    private List<MonoBehaviour> disabledDuringUI = new List<MonoBehaviour>();
//
//    // ✅ Historique complet des notes (dans l'ordre)
//    private readonly List<string> notesInOrder = new List<string>();
//
//    // ✅ Empêche les doublons exacts (même texte)
//    private readonly HashSet<string> collectedNotes = new HashSet<string>();
//
//    // ✅ Texte complet affiché
//    private string fullText = "";
//
//    void Awake()
//    {
//        Instance = this;
//
//        if (notebookRoot != null) notebookRoot.SetActive(false);
//        if (closeButton != null) closeButton.onClick.AddListener(Close);
//
//        if (controlsRoot == null)
//        {
//            var p = GameObject.FindWithTag("Player");
//            if (p != null) controlsRoot = p;
//            else if (Camera.main != null) controlsRoot = Camera.main.gameObject;
//        }
//    }
//
//    void Update()
//    {
//        if (Input.GetKeyDown(toggleKey))
//        {
//            if (open) Close();
//            else Open();
//        }
//        
//        if (open)
//        {
//            Cursor.lockState = CursorLockMode.None;
//            Cursor.visible = true;
//        }
//    }
//
//    public void Open()
//    {
//        if (open) return;
//        open = true;
//
//        if (notebookRoot != null) notebookRoot.SetActive(true);
//
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//
//        DisableControlsByName();
//
//        // ✅ Pas de "pas de note": si rien, c'est juste vide
//        if (noteText != null)
//            noteText.text = fullText;
//    }
//
//    public void Close()
//    {
//        if (!open) return;
//        open = false;
//
//        if (notebookRoot != null) notebookRoot.SetActive(false);
//
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//
//        RestoreControls();
//    }
//
//    public void AddNote(string note)
//    {
//        if (string.IsNullOrWhiteSpace(note)) return;
//
//        // ✅ on empêche uniquement les doublons exacts
//        if (!collectedNotes.Add(note))
//            return;
//
//        notesInOrder.Add(note);
//
//        // ✅ on reconstruit le texte affiché (simple et fiable)
//        fullText = BuildNotebookText();
//
//        // ✅ update direct si le carnet est ouvert
//        if (open && noteText != null)
//            noteText.text = fullText;
//    }
//
//    string BuildNotebookText()
//    {
//        // Format : chaque note sur un bloc, séparé par une ligne vide
//        // (tu peux changer la mise en forme ici)
//        System.Text.StringBuilder sb = new System.Text.StringBuilder();
//
//        for (int i = 0; i < notesInOrder.Count; i++)
//        {
//            if (i > 0) sb.Append("\n\n");
//            sb.Append("• ");
//            sb.Append(notesInOrder[i]);
//        }
//
//        return sb.ToString();
//    }
//
//    void DisableControlsByName()
//    {
//        disabledDuringUI.Clear();
//        if (controlsRoot == null || scriptTypeNamesToDisable == null || scriptTypeNamesToDisable.Count == 0)
//            return;
//
//        var all = controlsRoot.GetComponentsInChildren<MonoBehaviour>(true);
//
//        foreach (var mb in all)
//        {
//            if (mb == null) continue;
//            string typeName = mb.GetType().Name;
//
//            for (int i = 0; i < scriptTypeNamesToDisable.Count; i++)
//            {
//                if (typeName == scriptTypeNamesToDisable[i] && mb.enabled)
//                {
//                    mb.enabled = false;
//                    disabledDuringUI.Add(mb);
//                }
//            }
//        }
//    }
//
//    void RestoreControls()
//    {
//        foreach (var mb in disabledDuringUI)
//        {
//            if (mb != null) mb.enabled = true;
//        }
//        disabledDuringUI.Clear();
//    }
//}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance;

    [Header("UI")]
    public GameObject notebookRoot;     // panel/canvas root
    public TMP_Text noteText;           // TMP_Text placé DANS Content du ScrollView
    public Button closeButton;          // optionnel
    public ScrollRect scrollRect;       // ✅ le ScrollRect du ScrollView (à drag)

    [Header("Touche")]
    public KeyCode toggleKey = KeyCode.N;

    [Header("Bloquer contrôles pendant le carnet")]
    public GameObject controlsRoot;
    public List<string> scriptTypeNamesToDisable = new List<string>();

    [Header("Comportement")]
    public bool preventExactDuplicates = true; // empêche doublons exacts
    public bool autoScrollToBottom = true;     // ✅ auto scroll quand nouvelle note

    private bool open = false;
    private List<MonoBehaviour> disabledDuringUI = new List<MonoBehaviour>();

    private readonly List<string> notesInOrder = new List<string>();
    private readonly HashSet<string> collectedNotes = new HashSet<string>();

    private string fullText = "";

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

        // sécurité : si ouvert, on force le curseur visible
        if (open)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

        RefreshText();

        if (autoScrollToBottom)
            StartCoroutine(ScrollToBottomNextFrame());
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

        if (preventExactDuplicates)
        {
            if (!collectedNotes.Add(note)) return; // déjà présent => ignore
        }

        notesInOrder.Add(note);
        fullText = BuildNotebookText();

        if (open)
        {
            RefreshText();

            if (autoScrollToBottom)
                StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    void RefreshText()
    {
        if (noteText != null)
            noteText.text = fullText;
    }

    string BuildNotebookText()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < notesInOrder.Count; i++)
        {
            if (i > 0) sb.Append("\n\n");
            sb.Append("• ");
            sb.Append(notesInOrder[i]);
        }

        return sb.ToString();
    }

    System.Collections.IEnumerator ScrollToBottomNextFrame()
    {
        // attendre que Unity recalcul la taille du Content
        yield return null;
        yield return null;

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f; // 0 = bas
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