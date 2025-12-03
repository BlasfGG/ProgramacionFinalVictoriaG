using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private Weapon armaActual;

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;
        {
            if (GetInput(out NetworkInfoData input))
            {
                if (input.buttons.IsSet((NetworkInfoData.BotonDisparo)))
                {
                   TipoDeArma();
                }
            }
        }
    }


    private void TipoDeArma()
    {
        switch (armaActual.TipoDeDisparo)
        {
            case ShootMode.RigidBody:
                armaActual.ModoRigidBody();
                break;
            case ShootMode.Raycast:
                armaActual.RpcRaycast();
                break;
        }
    }

}
