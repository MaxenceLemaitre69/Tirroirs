using UnityEngine;

public class ObjectStateController : MonoBehaviour
{
    public ObjectStateData data;
    public Transform spawnPoint;

    private GameObject currentInstance;
    private bool isAfterEvent = false;

    private void Start()
    {
        SpawnInitial();
    }

    private void SpawnInitial()
    {
        currentInstance = Instantiate(
            data.initialPrefab,
            spawnPoint != null ? spawnPoint.position : transform.position,
            spawnPoint != null ? spawnPoint.rotation : transform.rotation,
            transform
        );

        isAfterEvent = false;
    }

    public void SwitchToAfterEvent()
    {
        if (isAfterEvent) return;

        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = Instantiate(
            data.afterEventPrefab,
            spawnPoint != null ? spawnPoint.position : transform.position,
            spawnPoint != null ? spawnPoint.rotation : transform.rotation,
            transform
        );

        isAfterEvent = true;
    }
}