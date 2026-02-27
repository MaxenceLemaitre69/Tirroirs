using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public AudioSource audioSource;
    public CanvasGroup canvasGroup;

    [Header("Contrôles")]
    public KeyCode advanceKey = KeyCode.E;

    public Action<int, DialogueData.DialogueLine> onLineStarted;
    public int CurrentLineIndex { get; private set; } = -1;
    public bool IsPlaying { get; private set; } = false;

    public static bool AnyDialoguePlaying { get; private set; } = false;

    // évite que le même appui sur E (celui qui a déclenché l'interaction) passe direct la 1ère ligne
    private bool waitRelease = false;

    public IEnumerator PlayDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
            yield break;

        IsPlaying = true;
        AnyDialoguePlaying = true;

        CurrentLineIndex = -1;
        if (canvasGroup != null) canvasGroup.alpha = 1;

        // on attend que E soit relâché si le joueur l'avait déjà enfoncé
        waitRelease = Input.GetKey(advanceKey);
        while (waitRelease)
        {
            if (!Input.GetKey(advanceKey)) waitRelease = false;
            yield return null;
        }

        for (int i = 0; i < dialogue.lines.Length; i++)
        {
            CurrentLineIndex = i;
            var line = dialogue.lines[i];

            onLineStarted?.Invoke(i, line);

            if (textUI != null) textUI.text = line.text ?? "";

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = line.audio;
                if (audioSource.clip != null) audioSource.Play();
            }

            // attendre appui sur E
            yield return WaitForAdvancePress();
        }

        // Fin
        if (textUI != null) textUI.text = "";
        if (canvasGroup != null) canvasGroup.alpha = 0;

        if (audioSource != null) audioSource.Stop();

        IsPlaying = false;
        AnyDialoguePlaying = false;
        CurrentLineIndex = -1;
    }

    IEnumerator WaitForAdvancePress()
    {
        // attendre que la touche soit relâchée (anti double)
        while (Input.GetKey(advanceKey))
            yield return null;

        // attendre appui
        while (!Input.GetKeyDown(advanceKey))
            yield return null;

        yield return null; // laisse passer une frame
    }
}