using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptMainMenuUI : MonoBehaviour
{
    public string nomeCenaJogo = "Jogo";
    public string nomeCenaMenu = "Menu";

    public void Jogar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaJogo);
    }

    public void IrParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    public void Sair()
    {
        Application.Quit();

        UnityEditor.EditorApplication.isPlaying = false;

    }
}