using Fusion;
using System.Threading.Tasks;
using UnityEngine;


public class Bullet : NetworkBehaviour
{

    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] public int dañoDeBala;

    //[SerializeField] private NetworkPrefabRef decal;
    // [SerializeField] private NetworkPrefabRef particulas;
    private Rigidbody rb;

    [Networked] public PlayerRef MiBala { get; set; } // Creo Un Networked PlayerRef para almacenar el dueño de la bala  

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        if (Object.HasStateAuthority) rb.linearVelocity = speed * transform.forward;
        DespawnAfterTime();
    }

    private async void DespawnAfterTime()
    {
        await Task.Delay((int)(lifetime * 1000));
        if (Object != null && Object.HasStateAuthority) Runner.Despawn(Object);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ColisionValida()) return;

        ContactPoint impacto = collision.GetContact(0);

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (collision.gameObject.TryGetComponent<Salud>(out Salud salud))
            {
                RpcDañoEnemigo(MiBala, salud.Object, dañoDeBala);

                // RpcSpawnParticulas(impacto.point);
                Runner.Despawn(Object);
            }

            else if (collision.gameObject.CompareTag("Pared"))
            {
                Vector3 spawnPos = impacto.point + impacto.normal * 0.01f;
                Quaternion decalRotation = Quaternion.LookRotation(-impacto.normal);

                // RpcSpawnDecal(spawnPos, decalRotation);
                Runner.Despawn(Object);
            }
        }
    }

    private bool ColisionValida()
    {
        return Object != null && Object.HasStateAuthority;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void RpcDañoEnemigo(PlayerRef jugador, NetworkObject enemigo, int daño)
    {
        if (enemigo != null && enemigo.TryGetComponent<Salud>(out Salud salud))
        {
            salud.Rpc_TakeDamage(daño, jugador);
        }
    }

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RpcSpawnParticulas(Vector3 position)
    //{
    //    if (particulas.IsValid)
    //    {
    //        var particulas = Runner.Spawn(this.particulas, position, Quaternion.identity);
    //        if (particulas != null)
    //        {
    //            var ps = particulas.GetComponent<ParticleSystem>();
    //            if (ps != null) ps.Play();
    //        }
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RpcSpawnDecal(Vector3 posicion, Quaternion rotacion)
    //{
    //    if (decal.IsValid)
    //    {
    //        var decal = Runner.Spawn(this.decal, posicion, rotacion);
    //    }
    //}



}