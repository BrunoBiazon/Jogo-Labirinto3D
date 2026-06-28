using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class scriptPC : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 8f;
    public float mouseSensitivity = 15f;
    
    [Header("Áudio")]
    public AudioClip somColeta;
    public AudioClip somMorte;
    public AudioClip somComerFantasma;
    
    [Header("Referências")]
    public Transform cameraTransform;

    private CharacterController controller;
    private float rotationX = 0f;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private bool jogoFinalizado = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
    }

    void OnEnable()
    {
        scriptGameManager.OnResetarRodada += ResetarJogador;
        scriptGameManager.OnGameOver += BloquearEntrada;
    }

    void OnDisable()
    {
        scriptGameManager.OnResetarRodada -= ResetarJogador;
        scriptGameManager.OnGameOver -= BloquearEntrada;
    }

    void Update()
    {
        if (jogoFinalizado) return;

        HandleMouseLook();
        Move();
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null || cameraTransform == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
        
        rotationX -= mouseY; 
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    private void Move()
    {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;

        if (moveDirection != Vector3.zero)
        {
            Vector3 velocity = moveDirection * moveSpeed;
            velocity.y = Physics.gravity.y;
            
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            Vector3 velocity = Vector3.zero;
            velocity.y = Physics.gravity.y;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pellet"))
        {
            TocarSomColeta(other.transform.position);

            if (scriptGameManager.Instancia != null)
            {
                scriptGameManager.Instancia.AdicionarPontos(5); 
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerUp"))
        {
            TocarSomColeta(other.transform.position);

            if (scriptGameManager.Instancia != null)
            {
                scriptGameManager.Instancia.ColetarPowerUp();
                scriptGameManager.Instancia.AdicionarPontos(10);
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Ghost"))
        {
            ProcessarColisaoFantasma(other);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ghost"))
        {
            ProcessarColisaoFantasma(hit.collider);
        }
    }

    private void ProcessarColisaoFantasma(Collider ghostCollider)
    {
        scriptGhost ghost = ghostCollider.GetComponent<scriptGhost>();
        if (ghost == null)
        {
            ghost = ghostCollider.GetComponentInParent<scriptGhost>();
        }

        if (ghost != null)
        {
            bool GMExiste = scriptGameManager.Instancia != null;
            bool fantasmasVulneraveis = GMExiste && scriptGameManager.Instancia.GhostsVulneraveis;

            if (fantasmasVulneraveis)
            {
                if (somComerFantasma != null)
                {
                    AudioSource.PlayClipAtPoint(somComerFantasma, ghostCollider.transform.position);
                }
                else
                {
                    TocarSomColeta(ghostCollider.transform.position);
                }
                ghost.RetornarParaBase();
                scriptGameManager.Instancia.AdicionarPontos(200);
            }
            else
            {
                if (scriptGameManager.Instancia != null)
                {
                    scriptGameManager.Instancia.PerderVida();
                }
            }
        }
    }

    private void TocarSomColeta(Vector3 posicao)
    {
        if (somColeta != null)
        {
            AudioSource.PlayClipAtPoint(somColeta, posicao);
        }
    }

    private void ResetarJogador()
    {
        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(somMorte, transform.position);
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        rotationX = 0f;

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.identity;
        }

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    private void BloquearEntrada()
    {
        jogoFinalizado = true;
        Cursor.lockState = CursorLockMode.None;

        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(somMorte, transform.position);
        }
    }
}