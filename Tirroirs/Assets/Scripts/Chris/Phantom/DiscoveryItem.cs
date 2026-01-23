using UnityEngine;

public class DiscoveryItem : MonoBehaviour
{
    [Tooltip("Combien cet objet ajoute au compteur")]
    public int discoveryValue = 1;

    [Tooltip("Empêche de compter plusieurs fois")]
    public bool countOnlyOnce = true;

    [Tooltip("Option : désactive l'interaction après découverte")]
    public bool disableInteractAfterDiscovery = true;

    private bool counted = false;

    void Interacting()
    {
        if (countOnlyOnce && counted) return;

        counted = true;

        if (DiscoveryCounter.Instance != null)
            DiscoveryCounter.Instance.AddDiscovery(discoveryValue);

        if (disableInteractAfterDiscovery)
            gameObject.tag = "Untagged";
    }
}