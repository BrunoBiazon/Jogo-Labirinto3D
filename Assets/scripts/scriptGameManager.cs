using UnityEngine;
using TMPro;
using System.Collections;

public class scriptGameManager : MonoBehaviour
{
    public static scriptGameManager Instancia { get; private set; }

    [Header("Configurações de Pontuação")]
    public int pontuacaoAtual = 0;
    public TextMeshProUGUI textoPontuacao;

    [Header("Configurações de Vidas")]
    public int vidasMaximas = 3;
    private int vidasAtuais;
    public TextMeshProUGUI textoVidas;

    [Header("Configurações de PowerUp")]
    public float duracaoPowerUp = 10f;
    private bool ghostsVulneraveis = false;
    public AudioClip somPowerUpActive;
    private AudioSource audioSourcePowerUpActive;

    public bool GhostsVulneraveis => ghostsVulneraveis;

    private Coroutine corrotinaPowerUp;

    public static event System.Action<bool> OnVulnerabilidadeAlterada;
    public static event System.Action OnResetarRodada;
    public static event System.Action OnGameOver;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (somPowerUpActive != null)
        {
            audioSourcePowerUpActive = gameObject.AddComponent<AudioSource>();
            audioSourcePowerUpActive.clip = somPowerUpActive;
            audioSourcePowerUpActive.loop = true;
            audioSourcePowerUpActive.playOnAwake = false;
        }
    }

    void Start()
    {
        vidasAtuais = vidasMaximas;
        
        AtualizarHUD();
    }

    public void AdicionarPontos(int valor)
    {
        pontuacaoAtual += valor;
        AtualizarHUD(); 
    }

    public void PerderVida()
    {
        vidasAtuais--;
        AtualizarHUD();

        if (audioSourcePowerUpActive != null && audioSourcePowerUpActive.isPlaying)
        {
            audioSourcePowerUpActive.Stop();
        }

        if (vidasAtuais <= 0)
        {
            GameOver();
        }
        else
        {
            OnResetarRodada?.Invoke();
        }
    }

    public void ColetarPowerUp()
    {
        if (corrotinaPowerUp != null)
        {
            StopCoroutine(corrotinaPowerUp);
        }
        corrotinaPowerUp = StartCoroutine(TemporizadorPowerUp(duracaoPowerUp));
    }

    private IEnumerator TemporizadorPowerUp(float duracao)
    {
        ghostsVulneraveis = true;
        OnVulnerabilidadeAlterada?.Invoke(true);

        if (audioSourcePowerUpActive != null && !audioSourcePowerUpActive.isPlaying)
        {
            audioSourcePowerUpActive.Play();
        }

        yield return new WaitForSeconds(duracao);

        ghostsVulneraveis = false;
        OnVulnerabilidadeAlterada?.Invoke(false);

        if (audioSourcePowerUpActive != null)
        {
            audioSourcePowerUpActive.Stop();
        }

        corrotinaPowerUp = null;
    }

    private void GameOver()
    {
        if (audioSourcePowerUpActive != null)
        {
            audioSourcePowerUpActive.Stop();
        }
        OnGameOver?.Invoke();
    }

    private void AtualizarHUD()
    {
        if (textoPontuacao != null)
        {
            textoPontuacao.text = "Pontos: " + pontuacaoAtual;
        }

        if (textoVidas != null)
        {
            textoVidas.text = "Vidas: " + vidasAtuais;
        }
    }
}