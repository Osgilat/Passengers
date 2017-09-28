using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealAbility : NetworkBehaviour
{
    public bool healOnceBool = false;
    private const float playerSpeed = 1.5f;
    private const float timeForHeal = 7.0f;
    public static bool healTrigger = false;
    public GameObject targetToHeal = null;



    public void ClickToHeal()
    {
       // healTrigger = !healTrigger;
        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.healButton);
        if (!healTrigger)
        {
            healTrigger = true;
            GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.clear;
            /*
            PushAbility.pushed = false;
            GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;

            if (GameObject.FindGameObjectWithTag("Greet") != null)
            {
                PlayerActor.sayHelpMe = false;
                GameObject.FindGameObjectWithTag("Greet").GetComponent<Image>().color = Color.white;
            }
            if (GameObject.FindGameObjectWithTag("ThankButton") != null)
            {
                PlayerActor.sayThank = false;
                GameObject.FindGameObjectWithTag("ThankButton").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("WakeUpButton") != null)
            {
                WakeUpAbility.wakeUp = false;
                GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("TargetButton") != null)
            {
                ShootAbility.pointed = false;
                GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.white;
            }
            */
        }

        else
        {
            healTrigger = false;
            GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.white;
        }

    }

    public void AI_Heal(GameObject playerToHeal)
    {
        healTrigger = true;
        CmdParticles(playerToHeal);
    }

    void FixedUpdate()
    {
        //Control only by local player
        if (!isLocalPlayer)
        {
            return;
        }

        if (healOnceBool)
        {
            UseTeleport.healButton.SetActive(false);
        }

        //if left clicked
        if (Input.GetMouseButtonDown(0))
        {
            //Clicked place
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {

                //if greetButton active and clicked on a player
                if (healTrigger && hit.transform.CompareTag("Player")
                    && hit.transform.gameObject.GetComponent<ShootAbility>().stunned
                    && hit.transform.gameObject != gameObject
                    && !gameObject.GetComponent<ShootAbility>().stunned
                    && !healOnceBool)
                {
                    healOnceBool = true;

                    CmdParticles(hit.transform.gameObject);
                }
            }
        }
    }

    [Command]
    void CmdParticles(GameObject targetPlayer)
    {

        //Calling on clients
        targetPlayer.GetComponent<ShootAbility>().stunned = false;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = playerSpeed;
        targetToHeal = targetPlayer;
        targetPlayer.GetComponentInChildren<Animator>().SetBool("Died", false);
        healTrigger = false;
        RpcParticles(targetPlayer);
    }

    //For each client make event active
    [ClientRpc]
    void RpcParticles(GameObject targetPlayer)
    {
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID
            + "," + this.gameObject.GetComponent<UseTeleport>().playerID + ","
            + "Heal" + "," + this.name + "," + this.gameObject.transform.position + "," + targetPlayer.name);
        //For AI vision
        /*
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<Main>().enabled)
                player.GetComponent<Main>().ListenerActionAvatar("Heal", this.gameObject, targetPlayer);
        }
        */
        targetPlayer.GetComponent<ShootAbility>().stunned = false;
        targetPlayer.GetComponent<ShootAbility>().timeForHeal = timeForHeal;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        targetPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = playerSpeed;
        targetToHeal = targetPlayer;
        targetPlayer.GetComponentInChildren<Animator>().SetBool("Died", false);
        healTrigger = false;
    }

}
