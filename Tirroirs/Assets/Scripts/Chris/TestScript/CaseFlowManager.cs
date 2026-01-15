using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CaseFlowManager : MonoBehaviour
{
    public static CaseFlowManager Instance;

    public enum GamePhase
    {
        WaitingPoliceStart,      // policier interactable, porte non
        InvestigationTimer,      // 10 min, porte interactable, policier non
        Teleported_ShowTimeUp,   // image "temps écoulé" 3 sec
        WaitingPoliceAnswer,     // 5 min pour revenir parler au policier
        AccusationChoice,        // UI 3 suspects
        Victory,
        Defeat
    }

    [Header("Références scène")]
    public DialogueUI dialogueUI;
    public Transform player;
    public CharacterController playerCC;

    [Header("Objets interactables (GameObjects qui portent le collider/tag)")]
    public GameObject policemanInteractGO;
    public GameObject doorInteractGO;

    [Header("Tags")]
    public string interactableTag = "Interactable";
    public string disabledTag = "Untagged";

    [Header("Timers")]
    public float investigationDurationSeconds = 600f; // 10 min
    public float answerWindowSeconds = 300f;          // 5 min

    [Header("UI Timer (en haut droite)")]
    public TMP_Text timerText;

    [Header("UI Temps écoulé (3 sec)")]
    public GameObject timeUpCanvasRoot;  // image "temps écoulé"
    public float timeUpDuration = 3f;

    [Header("Téléportation fin 10 min")]
    public Transform teleportDestination;

    [Header("Accusation UI")]
    public GameObject accusationCanvasRoot;
    public Button suspect1Button;
    public Button suspect2Button;
    public Button suspect3Button;

    [Tooltip("Index du coupable (1,2,3)")]
    public int correctSuspect = 2;

    [Header("UI Victoire / Défaite")]
    public GameObject victoryCanvasRoot;
    public GameObject defeatCanvasRoot;
    public Button victoryMainMenuButton;
    public Button defeatMainMenuButton;

    [Header("Bloquer contrôles pendant UI (accusation/victoire/défaite)")]
    public GameObject controlsRoot;
    public List<string> scriptTypeNamesToDisable = new List<string>();
    private readonly List<MonoBehaviour> disabledDuringUI = new List<MonoBehaviour>();

    private Coroutine timerRoutine;
    private GamePhase phase = GamePhase.WaitingPoliceStart;

    void Awake()
    {
        Instance = this;

        if (dialogueUI == null) dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (playerCC == null && player != null) playerCC = player.GetComponent<CharacterController>();

        if (controlsRoot == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) controlsRoot = p;
            else if (Camera.main != null) controlsRoot = Camera.main.gameObject;
        }


        SafeSetActive(timeUpCanvasRoot, false);
        SafeSetActive(accusationCanvasRoot, false);
        SafeSetActive(victoryCanvasRoot, false);
        SafeSetActive(defeatCanvasRoot, false);

        if (suspect1Button != null) suspect1Button.onClick.AddListener(() => ChooseSuspect(1));
        if (suspect2Button != null) suspect2Button.onClick.AddListener(() => ChooseSuspect(2));
        if (suspect3Button != null) suspect3Button.onClick.AddListener(() => ChooseSuspect(3));
        
        if (victoryMainMenuButton != null) victoryMainMenuButton.onClick.AddListener(() => Debug.Log("Main Menu (Victory)"));
        if (defeatMainMenuButton != null) defeatMainMenuButton.onClick.AddListener(() => Debug.Log("Main Menu (Defeat)"));


        SetPhase(GamePhase.WaitingPoliceStart);
    }

    void SafeSetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }

    void SetInteractable(GameObject go, bool canInteract)
    {
        if (go == null) return;
        go.tag = canInteract ? interactableTag : disabledTag;
    }

    void SetPhase(GamePhase newPhase)
    {
        phase = newPhase;


        SafeSetActive(accusationCanvasRoot, false);
        SafeSetActive(victoryCanvasRoot, false);
        SafeSetActive(defeatCanvasRoot, false);


        if (phase == GamePhase.AccusationChoice || phase == GamePhase.Victory || phase == GamePhase.Defeat)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisableControlsByName();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            RestoreControls();
        }
        
        switch (phase)
        {
            case GamePhase.WaitingPoliceStart:
                SetInteractable(policemanInteractGO, true);
                SetInteractable(doorInteractGO, false);
                SetTimerText("");
                break;

            case GamePhase.InvestigationTimer:
                SetInteractable(policemanInteractGO, false);
                SetInteractable(doorInteractGO, true);
                break;

            case GamePhase.Teleported_ShowTimeUp:
                SetInteractable(policemanInteractGO, false);
                SetInteractable(doorInteractGO, false);
                break;

            case GamePhase.WaitingPoliceAnswer:
                SetInteractable(policemanInteractGO, true);
                SetInteractable(doorInteractGO, false);
                break;

            case GamePhase.AccusationChoice:
                SetInteractable(policemanInteractGO, false);
                SetInteractable(doorInteractGO, false);
                break;

            case GamePhase.Victory:
            case GamePhase.Defeat:
                SetInteractable(policemanInteractGO, false);
                SetInteractable(doorInteractGO, false);
                SetTimerText("");
                break;
        }
    }

    void SetTimerText(string s)
    {
        if (timerText != null) timerText.text = s;
    }

    string FormatTime(float seconds)
    {
        if (seconds < 0) seconds = 0;
        int total = Mathf.CeilToInt(seconds);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }
    
    public void OnPolicemanInteracted(DialogueData startDialogue, DialogueData answerDialogue)
    {
        if (DialogueUI.AnyDialoguePlaying) return;

        if (phase == GamePhase.WaitingPoliceStart)
        {
            StartCoroutine(PoliceStartRoutine(startDialogue));
        }
        else if (phase == GamePhase.WaitingPoliceAnswer)
        {
            StartCoroutine(PoliceAnswerRoutine(answerDialogue));
        }
    }

    IEnumerator PoliceStartRoutine(DialogueData dialogue)
    {

        if (dialogueUI != null && dialogue != null)
            yield return StartCoroutine(dialogueUI.PlayDialogue(dialogue));


        SetPhase(GamePhase.InvestigationTimer);
        StartInvestigationTimer();
    }

    IEnumerator PoliceAnswerRoutine(DialogueData dialogue)
    {

        if (dialogueUI != null && dialogue != null)
            yield return StartCoroutine(dialogueUI.PlayDialogue(dialogue));


        SetPhase(GamePhase.AccusationChoice);
        SafeSetActive(accusationCanvasRoot, true);
    }

    public void OnDoorInteracted()
    {
        if (phase != GamePhase.InvestigationTimer) return;
        Debug.Log("Porte utilisée pendant investigation (si tu veux faire quelque chose ici).");
    }
    

    void StartInvestigationTimer()
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(InvestigationTimerRoutine());
    }

    IEnumerator InvestigationTimerRoutine()
    {
        float t = investigationDurationSeconds;

        while (t > 0f)
        {
            SetTimerText(FormatTime(t));
            yield return null;
            t -= Time.deltaTime;
        }

        SetTimerText("00:00");
        
        SetPhase(GamePhase.Teleported_ShowTimeUp);
        StartCoroutine(TimeUpTeleportAndStartAnswerWindow());
    }

    IEnumerator TimeUpTeleportAndStartAnswerWindow()
    {

        TeleportPlayer();


        SafeSetActive(timeUpCanvasRoot, true);
        yield return new WaitForSeconds(timeUpDuration);
        SafeSetActive(timeUpCanvasRoot, false);


        SetPhase(GamePhase.WaitingPoliceAnswer);
        StartAnswerWindowTimer();
    }

    void TeleportPlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player null");
            return;
        }
        if (teleportDestination == null)
        {
            Debug.LogError("TeleportDestination non assignée");
            return;
        }

        if (playerCC == null) playerCC = player.GetComponent<CharacterController>();

        if (playerCC != null) playerCC.enabled = false;
        player.position = teleportDestination.position;
        player.rotation = teleportDestination.rotation;
        if (playerCC != null) playerCC.enabled = true;
    }



    void StartAnswerWindowTimer()
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(AnswerWindowRoutine());
    }

    IEnumerator AnswerWindowRoutine()
    {
        float t = answerWindowSeconds;

        while (t > 0f)
        {
            SetTimerText(FormatTime(t));
            yield return null;
            t -= Time.deltaTime;


            if (phase == GamePhase.AccusationChoice) yield break;
        }


        SetTimerText("00:00");
        TriggerDefeat();
    }



    void ChooseSuspect(int suspectIndex)
    {
        SafeSetActive(accusationCanvasRoot, false);

        if (suspectIndex == correctSuspect) TriggerVictory();
        else TriggerDefeat();
    }

    void TriggerVictory()
    {
        SetPhase(GamePhase.Victory);
        SafeSetActive(victoryCanvasRoot, true);
    }

    void TriggerDefeat()
    {
        SetPhase(GamePhase.Defeat);
        SafeSetActive(defeatCanvasRoot, true);
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

