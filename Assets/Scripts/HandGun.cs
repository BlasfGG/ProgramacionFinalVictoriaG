using Fusion;
using UnityEngine;

public class HandGun : Weapon
{
   
    private float nextTimeToFire = 0f; // Tiempo para el próximo disparo permitido

    [Networked] public PlayerRef MiArma { get; set; }

    public override void Spawned()
    {
        MiArma = Object.InputAuthority;  // Inicializar el dueño del arma cuando spawneé
    }

    public override void ModoRigidBody()
    {
        if (!Object.HasInputAuthority) return;
        if (Time.time >= nextTimeToFire)
        {
            RpcRequestRigibodyShoot(puntoDisparo.position, puntoDisparo.rotation);
            nextTimeToFire = Time.time + (1f / cadencia);
        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)] // RpcTargets.StateAuthority se usa para spawnear objetos en el servidor
    private void RpcRequestRigibodyShoot(Vector3 pos, Quaternion rot)
    {
        NetworkObject _bullet = Runner.Spawn(balaPrefab, pos, rot);

        if (_bullet.TryGetComponent(out Bullet bullet))
        {
            bullet.dañoDeBala = daño;
            bullet.MiBala = MiArma;

        }

    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All)] // RpcTargets.All se usa para objetos que no necesitan spawnear en el servidor
    public override void RpcRaycast()
    {
        if (Time.time >= nextTimeToFire)
        {
            if (Physics.Raycast(cameraPos.position, cameraPos.forward, out hitInfo, alcance, hitLayers))
            {
                Debug.Log("Has golpeado a: " + hitInfo.collider.name);
            }

            if (hitInfo.collider.TryGetComponent<Salud>(out Salud salud))
            {
                salud.Rpc_TakeDamage(daño, MiArma);
            }

            nextTimeToFire = Time.time + (1f / cadencia);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public override void RpcRecargar() // Recarga del arma
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPos.position, cameraPos.forward * alcance);
    }
}
