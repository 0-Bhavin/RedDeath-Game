using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class winscreen : MonoBehaviour
{
    public GameObject winUI;
    public Image background;
    public Color gloomyColor = new Color(0.1f, 0f, 0.1f, 0.8f);
    
    void Start()
    {
        background.color = gloomyColor;
    }

    public void ShowWinScreen()
    {
        winUI.SetActive(true);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
