using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class scriptGhost : MonoBehaviour
{
    public Transform pc; 
    public Transform[] waypoints; 

    public float distanciaPerseguicao = 10f;
    public float distMin = 1.5f;

    [Header("Configurações de Velocidade")]
    public float velocidadeNormal = 3.5f;
    public float velocidadeVulneravel = 1.75f;

    [Header("Visual de Vulnerabilidade")]
    public Color corVulneravel = Color.blue;

    [Header("Áudio de Perseguição")]
    public AudioClip somPerseguicao;
    private AudioSource audioSourcePerseguicao;

    private NavMeshAgent agente;
    private int indice = 0;
    private bool persegue = false;
    private Transform destinoAtual;

    private Vector3 posicaoInicial;
    private Quaternion rotationInicial;
    private bool estaVulneravel = false;
    private Renderer ghostRenderer;
    private Color corOriginal;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        ghostRenderer = GetComponentInChildren<Renderer>();
        if (ghostRenderer != null)
        {
            corOriginal = ghostRenderer.material.color;
        }

        if (somPerseguicao != null)
        {
            audioSourcePerseguicao = gameObject.AddComponent<AudioSource>();
            audioSourcePerseguicao.clip = somPerseguicao;
            audioSourcePerseguicao.loop = true;
            audioSourcePerseguicao.playOnAwake = false;
            audioSourcePerseguicao.spatialBlend = 1.0f;
            audioSourcePerseguicao.minDistance = 1f;
            audioSourcePerseguicao.maxDistance = distanciaPerseguicao * 1.5f;
        }
    }

    void Start()
    {
        posicaoInicial = transform.position;
        rotationInicial = transform.rotation;
        agente.speed = velocidadeNormal;

        if (waypoints != null && waypoints.Length > 0)
        {
            destinoAtual = waypoints[0];
            agente.SetDestination(destinoAtual.position);
        }
    }

    void OnEnable()
    {
        scriptGameManager.OnVulnerabilidadeAlterada += DefinirVulnerabilidade;
        scriptGameManager.OnResetarRodada += ResetarGhost;
        scriptGameManager.OnGameOver += PararAgente;
    }

    void OnDisable()
    {
        scriptGameManager.OnVulnerabilidadeAlterada -= DefinirVulnerabilidade;
        scriptGameManager.OnResetarRodada -= ResetarGhost;
        scriptGameManager.OnGameOver -= PararAgente;
    }

    void Update()
    {
        if (pc == null) return; 

        if (estaVulneravel)
        {
            if (audioSourcePerseguicao != null && audioSourcePerseguicao.isPlaying)
            {
                audioSourcePerseguicao.Stop();
            }
            Patrulhar();
            return;
        }

        float distanciaParaPC = Vector3.Distance(transform.position, pc.position);

        if (distanciaParaPC < distanciaPerseguicao)
        {
            persegue = true;
        }
        else if (distanciaParaPC > distanciaPerseguicao * 1.5f)
        {
            persegue = false;
        }

        if (persegue)
        {
            agente.SetDestination(pc.position);
            if (audioSourcePerseguicao != null && !audioSourcePerseguicao.isPlaying)
            {
                audioSourcePerseguicao.Play();
            }
        }
        else
        {
            if (audioSourcePerseguicao != null && audioSourcePerseguicao.isPlaying)
            {
                audioSourcePerseguicao.Stop();
            }
            Patrulhar();
        }
    }

    private void Patrulhar()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (Vector3.Distance(transform.position, destinoAtual.position) < distMin)
        {
            Proximo();
        }
        else
        {
            agente.SetDestination(destinoAtual.position);
        }
    }

    private void Proximo()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        indice = (indice + 1) % waypoints.Length;
        destinoAtual = waypoints[indice];
        agente.SetDestination(destinoAtual.position);
    }

    private void DefinirVulnerabilidade(bool vulneravel)
    {
        estaVulneravel = vulneravel;
        agente.speed = vulneravel ? velocidadeVulneravel : velocidadeNormal;
        


        if (ghostRenderer != null)
        {
            ghostRenderer.material.color = vulneravel ? corVulneravel : corOriginal;
        }
    }

    public void RetornarParaBase()
    {

        ResetarGhost();
    }

    private void ResetarGhost()
    {
        if (agente != null) agente.enabled = false;

        transform.position = posicaoInicial;
        transform.rotation = rotationInicial;

        if (agente != null)
        {
            agente.enabled = true;
            agente.speed = velocidadeNormal;
        }

        persegue = false;
        estaVulneravel = false;

        if (audioSourcePerseguicao != null && audioSourcePerseguicao.isPlaying)
        {
            audioSourcePerseguicao.Stop();
        }

        if (ghostRenderer != null)
        {
            ghostRenderer.material.color = corOriginal;
        }

        if (waypoints != null && waypoints.Length > 0)
        {
            indice = 0;
            destinoAtual = waypoints[0];
            if (agente.enabled) agente.SetDestination(destinoAtual.position);
        }
    }

    private void PararAgente()
    {
        if (agente != null && agente.enabled)
        {
            agente.isStopped = true;
        }
        if (audioSourcePerseguicao != null && audioSourcePerseguicao.isPlaying)
        {
            audioSourcePerseguicao.Stop();
        }
    }
}