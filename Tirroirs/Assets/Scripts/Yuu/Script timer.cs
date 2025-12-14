using UnityEngine;
using System.Collections;

public class TimerCanvasTeleport : MonoBehaviour
{
    [Header("Temps")]
    public float timerBeforeCanvas = 5f;
    public float canvasDuration = 3f;

    [Header("Canvas")]
    public GameObject canvasToShow;

    [Header("Téléportation")]
    public Transform teleportDestination;

    private void Start()
    {
        if (canvasToShow != null)
            canvasToShow.SetActive(false);

        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        // le temps quand tu spawn
        yield return new WaitForSeconds(timerBeforeCanvas);

        if (canvasToShow != null)
            canvasToShow.SetActive(true);

        // le temp du canva
        yield return new WaitForSeconds(canvasDuration);

        if (canvasToShow != null)
            canvasToShow.SetActive(false);

        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("ya pas le tag player)");
            return;
        }

        if (teleportDestination == null)
        {
            Debug.LogError("je trouve po la destination");
            return;
        }

        // reprise teleporter walking sim
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = new Vector3(
                teleportDestination.position.x,
                teleportDestination.position.y,
                teleportDestination.position.z
            );
            cc.enabled = true;
        }

        Debug.Log("C censé fonctionner mais ça veut pas ahiii");
    }
}