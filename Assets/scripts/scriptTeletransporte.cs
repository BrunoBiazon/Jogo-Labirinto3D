using UnityEngine;
using UnityEngine.AI;

public class scriptTeletransporte : MonoBehaviour
{
    public Transform destino;
    public bool inverterEixoX = true;
    public bool inverterEixoZ = false;

    private static float ultimoTeleporteTime;
    private static float cooldown = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - ultimoTeleporteTime < cooldown) return;

        scriptPC pc = other.GetComponent<scriptPC>();
        if (pc == null)
        {
            pc = other.GetComponentInParent<scriptPC>();
        }

        scriptGhost ghost = other.GetComponent<scriptGhost>();
        if (ghost == null)
        {
            ghost = other.GetComponentInParent<scriptGhost>();
        }

        if (pc != null || ghost != null)
        {
            ultimoTeleporteTime = Time.time;
            Vector3 novaPosicao;

            if (destino != null)
            {
                novaPosicao = destino.position;
            }
            else
            {
                novaPosicao = other.transform.position;
                if (inverterEixoX)
                {
                    novaPosicao.x = -novaPosicao.x;
                }
                if (inverterEixoZ)
                {
                    novaPosicao.z = -novaPosicao.z;
                }
            }

            if (pc != null)
            {
                CharacterController cc = pc.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                pc.transform.position = novaPosicao;
                if (cc != null) cc.enabled = true;
            }
            else if (ghost != null)
            {
                NavMeshAgent agent = ghost.GetComponent<NavMeshAgent>();
                if (agent != null) agent.enabled = false;
                ghost.transform.position = novaPosicao;
                if (agent != null) agent.enabled = true;
            }
        }
    }
}