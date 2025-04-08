using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class gameover : MonoBehaviour
{
    public GameObject gameOverUI;
    public Image background;
    public Color gloomyColor = new Color(0.05f, 0f, 0f, 0.8f);
    
    void Start()
    {
        background.color = gloomyColor;
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}