using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if(IsServer) StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(4);
        GetComponent<NetworkObject>().Despawn(this);
    }
}
