﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WakeUpAbility : NetworkBehaviour
{

 
    public float rotationSpeed;
    public ParticleSystem kickEffect;

    public static bool wakeUp = false; //Used to control button's behavior

    private Vector3 targetDir; 
    private AudioSync audioSync;    //Reference to a audioSync script
    private static GameObject wakeUpButton;   //Reference to a button
    private Animator animator;		//Reference to animator

    public void WakeUpOnClick()
    {
        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.wakeUpButton);
        if (!wakeUp)
        {
            wakeUp = true;
            GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.clear;

            /*
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

            if (GameObject.FindGameObjectWithTag("Kick") != null)
            {
                PushAbility.pushed = false;
                GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("HealButton") != null)
            {
                HealAbility.healTrigger = false;
                GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.white;
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
            wakeUp = false;
            GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;
        }



    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSync = GetComponent<AudioSync>();
        

    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButton(0) && wakeUp)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //if hitted player
                if (hit.transform.CompareTag("Player"))
                {
                    //Kick across network
                    Cmd_CollidersIdentify(this.transform.position, 2.5f, hit.transform.gameObject);

                }
            }
        }
    }

    //Function to AI control
    public void AI_WakeUp(GameObject ai_target)
    {

        Cmd_CollidersIdentify(this.transform.position, 3f, ai_target);

    }

    [Command]
    void Cmd_CollidersIdentify(Vector3 center, float radius, GameObject target)
    {
        //Take all coliders in a radius
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //If player in radius
            if (hitColliders[i].tag == "Player" && hitColliders[i].gameObject != gameObject)
            {
                //then move player across network
                GameObject player = hitColliders[i].gameObject;
                Rpc_WakeUp(player, target);
                return;
            }
            i++;
        }
    }

    //Move player across network
    [ClientRpc]
    void Rpc_WakeUp(GameObject player, GameObject target)
    {
        //Calculate push vector
        targetDir = target.transform.position - this.transform.position;
        float step = rotationSpeed * Time.deltaTime;

        //Rotate towards a goal
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);

        //Set animation, sound and deactivate button
        // kickEffect.Play();

        animator.SetTrigger("TriggerAnger");
       // audioSync.PlaySound(3);
        wakeUp = false;
        GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;

        /*
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<Main>().enabled)
                p.GetComponent<Main>().ListenerActionAvatar("Kick", player, target);
        }
        */

        //Log about kick
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID
            + "," + "WakeUp" + "," + this.name + "," + this.gameObject.transform.position + "," + player.name);

        player.GetComponent<HybernationSystem>().hybernated = false;
            //CmdDisableHybernate(player);

       
    }

    
}
