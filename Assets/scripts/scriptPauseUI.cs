using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptPauseUI : MonoBehaviour
{
    public string nomeCenaMenu = "Jogo";
    public string nomeCenaPausa = "Menu";

    public void Retomar()
    {
        scriptGameUI gameUI = FindObjectOfType<scriptGameUI>();
        if (gameUI != null)
        {
            gameUI.RetomarJogo();
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.UnloadSceneAsync(nomeCenaPausa);
        }
    }

    public void IrParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }
}