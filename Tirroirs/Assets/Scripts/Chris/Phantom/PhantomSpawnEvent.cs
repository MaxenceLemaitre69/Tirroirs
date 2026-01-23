using UnityEngine;

public class PhantomSpawnEvent : MonoBehaviour
{
    [Header("Condition")]
    [Tooltip("Nombre d'objets découverts nécessaires")]
    public int requiredDiscoveries = 5;

    [Header("Phantom")]
    public GameObject phantom;           // ton fantôme dans la scène (désactivé au départ)
    public Transform spawnPoint;         // optionnel (si tu veux le placer)
    public bool setPositionOnSpawn = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spawnSfx;

    [Header("Options")]
    public bool triggerOnlyOnce = true;

    private bool triggered = false;

    void Start()
    {
        // Si le phantom doit être caché au départ
        if (phantom != null) phantom.SetActive(false);

        // Auto setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // S'abonner au compteur
        if (DiscoveryCounter.Instance != null)
            DiscoveryCounter.Instance.onCountChanged += OnCountChanged;
        else
            Debug.LogWarning("DiscoveryCounter.Instance introuvable (ajoute DiscoveryCounter dans la scène).");
    }

    void OnDestroy()
    {
        if (DiscoveryCounter.Instance != null)
            DiscoveryCounter.Instance.onCountChanged -= OnCountChanged;
    }

    void OnCountChanged(int newCount)
    {
        if (triggerOnlyOnce && triggered) return;

        if (newCount >= requiredDiscoveries)
        {
            TriggerSpawn();
        }
    }

    void TriggerSpawn()
    {
        if (triggerOnlyOnce && triggered) return;
        triggered = true;

        if (phantom == null)
        {
            Debug.LogWarning("[PhantomSpawnEvent] Phantom non assigné.");
            return;
        }

        if (setPositionOnSpawn && spawnPoint != null)
        {
            phantom.transform.position = spawnPoint.position;
            phantom.transform.rotation = spawnPoint.rotation;
        }

        phantom.SetActive(true);

        if (audioSource != null && spawnSfx != null)
            audioSource.PlayOneShot(spawnSfx);

        Debug.Log("[PhantomSpawnEvent] Phantom apparu !");
    }
}
