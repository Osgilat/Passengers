using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HybernationSystem : NetworkBehaviour
{

    public GameObject hybernationSphere;
    public ParticleSystem hybernationEffect;

    [SyncVar]
    public bool hybernated = false;

    void VarChanged(bool value)
    {
        hybernated = value;
        
    }


   
    
    public bool isHybernated()
    {
        return hybernated;
    }

    [Command]
    public void CmdRandomHybernation()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<HybernationSystem>().hybernated = true;
              //  CmdHybernate(player);
        }

        int randPlayer = Random.Range(0, players.Length - 1);
        RpcRandomHybernation(randPlayer, players);
        
    }

    [ClientRpc]
    public void RpcRandomHybernation(int randPlayer, GameObject[] players)
    {
        players[randPlayer].GetComponent<HybernationSystem>().hybernated = false;
    }

    private void Update()
    {
        if (hybernated)
        {
            EnableHybernation();
        } else
        {
            DisableHybernation();
        }
    }

    private void EnableHybernation()
    {
        gameObject.GetComponent<UseTeleport>().enabled = false;
        gameObject.GetComponent<ShootAbility>().enabled = false;
        gameObject.GetComponent<PlayerActor>().enabled = false;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        gameObject.GetComponent<PushAbility>().enabled = false;
        gameObject.GetComponent<HealAbility>().enabled = false;
        hybernationSphere.GetComponent<MeshRenderer>().enabled = true;
    }

    private void DisableHybernation()
    {
        gameObject.GetComponent<UseTeleport>().enabled = true;
        gameObject.GetComponent<ShootAbility>().enabled = true;
        gameObject.GetComponent<PlayerActor>().enabled = true;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        gameObject.GetComponent<PushAbility>().enabled = true;
        gameObject.GetComponent<HealAbility>().enabled = true;
        hybernationSphere.GetComponent<MeshRenderer>().enabled = false;
    }

	
}
