using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class scriptGameUI : MonoBehaviour
{
    public string nomeCenaGameOver = "GameOver";
    public string nomeCenaPausa = "Pause";

    private bool jogoPausado = false;
    private bool jogoFinalizado = false;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        scriptGameManager.OnGameOver += CarregarCenaGameOver;
    }

    void OnDisable()
    {
        scriptGameManager.OnGameOver -= CarregarCenaGameOver;
    }

    void Update()
    {
        if (jogoFinalizado) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (jogoPausado)
            {
                RetomarJogo();
            }
            else
            {
                PausarJogo();
            }
        }
    }

    public void PausarJogo()
    {
        jogoPausado = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nomeCenaPausa, LoadSceneMode.Additive);
    }

    public void RetomarJogo()
    {
        jogoPausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.UnloadSceneAsync(nomeCenaPausa);
    }

    private void CarregarCenaGameOver()
    {
        jogoFinalizado = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nomeCenaGameOver);
    }
}