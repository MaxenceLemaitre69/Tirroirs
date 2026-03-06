using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Bouton Jouer
    public void PlayGame()
    {
        SceneManager.LoadScene("SceneTestChris"); 
        // Remplace "GameScene" par le nom de ta scène de jeu
    }

    // Bouton Quitter
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }
}