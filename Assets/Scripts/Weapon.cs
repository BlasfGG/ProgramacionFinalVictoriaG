using Fusion;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected ShootMode tipoDeDisparo;

    public ShootMode TipoDeDisparo => tipoDeDisparo;

    [Header("Raycast")]
    [SerializeField] protected LayerMask hitLayers;
    [SerializeField] protected Transform cameraPos;

    [Header("RigidBody")]
    [SerializeField] protected NetworkObject balaPrefab;
    [SerializeField] public Transform puntoDisparo;

    [Header("Estadisticas")]
    [SerializeField] protected int daño;
    [SerializeField] protected float cadencia;
    [SerializeField] protected float alcance;
    [SerializeField] protected float tiempoDeRecarga;
    protected RaycastHit hitInfo;

    public abstract void ModoRigidBody();

    public virtual void RpcRaycast()
    {

    }
    public virtual void RpcRecargar()
    {

    }

}
