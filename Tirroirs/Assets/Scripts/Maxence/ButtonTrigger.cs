using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    [Header("Référence vers l'objet à changer")]
    public ObjectStateController targetObject;

    [Header("Touche d'activation")]
    public KeyCode activationKey = KeyCode.E;

    private bool canInteract = false;

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(activationKey))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (targetObject != null)
        {
            targetObject.SwitchToAfterEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}