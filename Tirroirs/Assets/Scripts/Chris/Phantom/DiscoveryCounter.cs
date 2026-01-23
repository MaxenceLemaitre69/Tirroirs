using UnityEngine;
using System;

public class DiscoveryCounter : MonoBehaviour
{
    public static DiscoveryCounter Instance;

    public int Count { get; private set; } = 0;

    // Event : notifie quand le compteur change
    public event Action<int> onCountChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionnel si tu veux garder entre scènes :
        // DontDestroyOnLoad(gameObject);
    }

    public void AddDiscovery(int amount = 1)
    {
        Count += amount;
        onCountChanged?.Invoke(Count);
        Debug.Log($"[DiscoveryCounter] Count = {Count}");
    }

    public void ResetCount()
    {
        Count = 0;
        onCountChanged?.Invoke(Count);
    }
}