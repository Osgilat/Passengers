using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerActor : NetworkBehaviour
{

    [SyncVar]
    public float speed; //Control speed of player
	[SyncVar]
    public float rotationSpeed; // Control rotation speed of player

	//public GameObject greetLightPrefab; //Light effect for greeting other players
	public Transform lightSpawn;		//Spawn point for a light effect
	public float waitBeforeEffect = .1f; //Delay after greeting to shoot a light effect
	public float lightTime = 0.6f;


	private	UnityEngine.AI.NavMeshAgent navmeshAgent; //Reference to player's navmesh agent
	private Animator animator;						//Reference to an Animator component
	private AudioSync audioSync;					//Reference to player's audioSync script

	//used to toggle greet button active state
	public static bool sayHelpMe = false;

    public static bool sayThank = false;


    public static List<GameObject> players;

    public static GameObject localPlayerGameObject;

    public Material localPlayerMaterial;

    public GameObject objectToPaint;

    private bool mLock;




    public override void OnStartLocalPlayer()
    {
        localPlayerGameObject = gameObject;
        objectToPaint.GetComponent<MeshRenderer>().material = localPlayerMaterial;
    }




    //Initialize on start
    void Start()
    {

        navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		animator = GetComponentInChildren<Animator> ();
		audioSync = GetComponent<AudioSync> ();
		
		
		GameObject.FindGameObjectWithTag ("Greet").GetComponentInChildren<Text> ().text = "ASK";
        GameObject.FindGameObjectWithTag("ThankButton").GetComponentInChildren<Text>().text = "THANK";
    }

    //Used to control button's behavior
    public void OnGreetHello()
    {
        ResetUI(UseTeleport.askButton);
        if (!sayHelpMe)
        {
            sayHelpMe = true;
            UseTeleport.askButton.GetComponent<Image>().color = Color.clear;
            /*
            
            PushAbility.pushed = false;
            UseTeleport.kickButton.GetComponent<Image>().color = Color.white;

             UseTeleport.thankButton.GetComponent<Image>().color = Color.white;
             sayThank = false;
            

          
             HealAbility.healTrigger = false;
             UseTeleport.healButton.GetComponent<Image>().color = Color.white;
            

           
             WakeUpAbility.wakeUp = false;
             UseTeleport.wakeUpButton.GetComponent<Image>().color = Color.white;

  
             ShootAbility.pointed = false;
             UseTeleport.targetButton.GetComponent<Image>().color = Color.white;
             */

        }
        else
        {
            sayHelpMe = false;
            UseTeleport.askButton.GetComponent<Image>().color = Color.white;
        }
    }

    //Used to control button's behavior
    public void OnThankClick()
    {
        ResetUI(UseTeleport.thankButton);
        
        if (!sayThank)
        {
            sayThank = true;
            UseTeleport.thankButton.GetComponent<Image>().color = Color.clear;
            /*
           
            PushAbility.pushed = false;
            UseTeleport.kickButton.GetComponent<Image>().color = Color.white;

            if (GameObject.FindGameObjectWithTag("Greet") != null)
            {
                UseTeleport.askButton.GetComponent<Image>().color = Color.white;
                sayThank = false;
            }

            if (GameObject.FindGameObjectWithTag("HealButton") != null)
            {
                HealAbility.healTrigger = false;
                UseTeleport.healButton.GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("WakeUpButton") != null)
            {
                WakeUpAbility.wakeUp = false;
                UseTeleport.wakeUpButton.GetComponent<Image>().color = Color.white;
            }
            */
        }
        else
        {
            sayThank = false;
            UseTeleport.thankButton.GetComponent<Image>().color = Color.white;
        }
        
    }

    
    public void ResetUI(GameObject resetForButton)
    {

        Hashtable buttons = new Hashtable();

        buttons.Add(UseTeleport.askButton, PlayerActor.sayHelpMe);
        buttons.Add(UseTeleport.kickButton, PushAbility.pushed);
        buttons.Add(UseTeleport.healButton, HealAbility.healTrigger);
        buttons.Add(UseTeleport.wakeUpButton, WakeUpAbility.wakeUp);
        buttons.Add(UseTeleport.targetButton, ShootAbility.pointed);
        buttons.Add(UseTeleport.thankButton, PlayerActor.sayThank);
        buttons.Add(UseTeleport.saveButton, UseTeleport.saveOther);


        buttons.Remove(resetForButton);

        List<string> keys = new List<string>();

        foreach (DictionaryEntry entry in buttons)
        {
            ((GameObject)entry.Key).GetComponent<Button>().GetComponent<Image>().color = Color.white;
            keys.Add(entry.Key.ToString());
        }

        foreach (string key in keys)
        {
            switch (key.ToString())
            {
                case "Ask (UnityEngine.GameObject)": PlayerActor.sayHelpMe = false; break;
                case "Kick (UnityEngine.GameObject)": PushAbility.pushed = false; break;
                case "Heal (UnityEngine.GameObject)": HealAbility.healTrigger = false; break;
                case "Target (UnityEngine.GameObject)": ShootAbility.pointed = false; break;
                case "WakeUp (UnityEngine.GameObject)": WakeUpAbility.wakeUp = false; break;
                case "Thank (UnityEngine.GameObject)": PlayerActor.sayThank = false; break;
                case "Save (UnityEngine.GameObject)": UseTeleport.saveOther = false; break;

            }
        }
        
         /*
         


         
         /*
         foreach (var key in keys)
         {
             buttons[key] = false;
             Debug.Log(key.ToString() + " " + buttons[key].ToString());
         }
         */


    }
    

    void FixedUpdate()
    {
		//Control only by local player
		if (!isLocalPlayer) {
			return;
		}


        if (Input.GetMouseButtonUp(0))
        {
            mLock = false;
        }

        //if left clicked
        if (!mLock && Input.GetMouseButtonDown(0)) {

            mLock = true;
            //Clicked place
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                
				//if clicked on a ground or one of the teleports		
				if (hit.transform.CompareTag ("Ground") || hit.transform.CompareTag ("Platform_1")
				    || hit.transform.CompareTag ("Platform_2")) {

					//Log movement information
					Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "moved" + "," + this.name + "," + hit.point, gameObject);
                    //Set destination for agent
                    navmeshAgent.destination = hit.point;
				} 
				//if greetButton active and clicked on a player
				if(sayHelpMe && hit.transform.CompareTag ("Player") && hit.transform.gameObject != gameObject){

					//Calculate vector3 to a clicked player
					Vector3 newDir = Vector3.RotateTowards (transform.forward, hit.transform.position - this.transform.position
						, 1000 * Time.deltaTime, 0.0F);

					//Rotate player
					transform.rotation = Quaternion.LookRotation (newDir);

					//Trigger animation
					animator.SetTrigger("SayHello");

					//Wait time before spawning light effect
					//StartCoroutine (WaitBeforeLightning ());

					//Flicker effect to a server and clients
					CmdFlickerLight ("ASK");

					//Bunch of ifs to control who and how greet
					if (hit.transform.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)")
                    {
						audioSync.PlaySound (0);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);
                    }
                    else if (hit.transform.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)")
                    {
						audioSync.PlaySound (1);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);
                    }
                    else if (hit.transform.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)")
                    {
						audioSync.PlaySound (2);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);
						
						//hello for an AI to recognize players greet
						Cmd_Hello (this.gameObject, hit.transform.gameObject);

					}
                    else if (hit.transform.name == "PlayerMod_D(Clone)" && this.name != "PlayerMod_D(Clone)")
                    {
                        audioSync.PlaySound(13);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);

                    }

                    //Toggle button to inactive state
                    sayHelpMe = false;
                    GameObject.FindGameObjectWithTag("Greet").GetComponent<Image>().color = Color.white;
                }
                else
                //if greetButton active and clicked on a player
                if (sayThank && hit.transform.CompareTag("Player") && hit.transform.gameObject != gameObject)
                {

                    //Calculate vector3 to a clicked player
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, hit.transform.position - this.transform.position
                        , 1000 * Time.deltaTime, 0.0F);

                    //Rotate player
                    transform.rotation = Quaternion.LookRotation(newDir);

                    //Trigger animation
                    animator.SetTrigger("SayHello");

                    //Wait time before spawning light effect
                    //StartCoroutine (WaitBeforeLightning ());

                    //Flicker effect to a server and clients
                    CmdFlickerLight("THANK");

                    //Bunch of ifs to control who and how greet
                    if (hit.transform.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)")
                    {
                        audioSync.PlaySound(8);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);
                    }
                    else if (hit.transform.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)")
                    {
                        audioSync.PlaySound(9);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);
                    }
                    else if (hit.transform.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)")
                    {
                        audioSync.PlaySound(10);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);

                    }
                    else if (hit.transform.name == "PlayerMod_D(Clone)" && this.name != "PlayerMod_D(Clone)")
                    {
                        audioSync.PlaySound(14);

                        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + hit.transform.position + "," + hit.transform.name);

                        //hello for an AI to recognize players greet
                        Cmd_Hello(this.gameObject, hit.transform.gameObject);

                    }

                    //Toggle button to inactive state
                    sayThank = false;
                    GameObject.FindGameObjectWithTag("ThankButton").GetComponent<Image>().color = Color.white;
                }
            }
		}
    }

	//Wait for slow rotation
	IEnumerator WaitBeforeLightning(string type){
        lightSpawn.GetComponent<Light>().enabled = true;
        if (type == "THANK")
        {
            lightSpawn.GetComponent<Light>().color = new Color(0, 255, 0);
        }
        else if (type == "ASK")
        {
            lightSpawn.GetComponent<Light>().color = new Color(0, 0, 255);
        }
       
        yield return new WaitForSeconds (waitBeforeEffect);
        lightSpawn.GetComponent<Light>().enabled = false;
    }

	//Flicker Light for a server and clients
	[Command]
	void CmdFlickerLight(string type){
        //Create greetLight from prefab
        /*
		var greetLight = (GameObject)Instantiate (greetLightPrefab, 
			lightSpawn.position, 
			lightSpawn.rotation);

		//Spawn on network
		NetworkServer.Spawn (greetLight);	

		//Destroy after
		Destroy (greetLight, lightTime);
        */
        StartCoroutine(WaitBeforeLightning(type));
        RpcFlickerLight(type);
        
    }

    //Flicker Light for a server and clients
    [ClientRpc]
    void RpcFlickerLight(string type)
    {
        //Create greetLight from prefab
        /*
		var greetLight = (GameObject)Instantiate (greetLightPrefab, 
			lightSpawn.position, 
			lightSpawn.rotation);

		//Spawn on network
		NetworkServer.Spawn (greetLight);	

		//Destroy after
		Destroy (greetLight, lightTime);
        */

        StartCoroutine(WaitBeforeLightning(type));

    }



    [Command]
	void Cmd_Hello(GameObject actor, GameObject target){
        //Calling on clients
        
		Rpc_Hello (actor, target);
	}

	//For each client make event active
	[ClientRpc]
	void Rpc_Hello(GameObject actor, GameObject target)
    {
        /*
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
			if(player.GetComponent<Behaviour>().enabled)
				player.GetComponent<Behaviour>().ListenerActionAvatar("Greet", actor, target);
        }
        */
        gameObject.GetComponent<UseTeleport>().NotifyBots("Greet", actor, target);

    }

	//Used by AI to greet other players
	public void AI_AskPlayer(GameObject ai_target){

		Transform player = ai_target.transform;
		Vector3 targetDir = player.position - this.transform.position;
		float step = rotationSpeed * Time.deltaTime;

		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0F);
		Debug.DrawRay (transform.position, newDir, Color.red);
		transform.rotation = Quaternion.LookRotation (newDir);

		animator.SetTrigger("SayHello");
        CmdFlickerLight("ASK");

		if (player.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)") {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
            audioSync.PlaySound (0);
            Cmd_Hello(this.gameObject, ai_target);
		} else if (player.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)") {
			audioSync.PlaySound (1);
            Cmd_Hello(this.gameObject, ai_target);
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
        } else if (player.name == "PlayerMod_C" && this.name != "PlayerMod_C(Clone)") {
			audioSync.PlaySound (2);
            Cmd_Hello(this.gameObject, ai_target);
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
		}else if (player.name == "PlayerMod_D(Clone)" && this.name != "PlayerMod_D(Clone)")
		{
			audioSync.PlaySound(13);
			Cmd_Hello(this.gameObject, ai_target);
			Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ASK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
		}


		//sayHelpMe = false;


	}

    public void AI_ThankPlayer(GameObject ai_target)
    {

        Transform player = ai_target.transform;
        Vector3 targetDir = player.position - this.transform.position;
        float step = rotationSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        Debug.DrawRay(transform.position, newDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);

        animator.SetTrigger("SayHello");
        CmdFlickerLight("THANK");

		if (player.name == "PlayerMod_A(Clone)" && this.name != "PlayerMod_A(Clone)")
        {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
            audioSync.PlaySound(8);
            Cmd_Hello(this.gameObject, ai_target);
        }
		else if (player.name == "PlayerMod_B(Clone)" && this.name != "PlayerMod_B(Clone)")
        {
            audioSync.PlaySound(9);
            Cmd_Hello(this.gameObject, ai_target);
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
        }
		else if (player.name == "PlayerMod_C(Clone)" && this.name != "PlayerMod_C(Clone)")
        {
            audioSync.PlaySound(10);
            Cmd_Hello(this.gameObject, ai_target);
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
		} else if (player.name == "PlayerMod_D(Clone)" && this.name != "PlayerMod_D(Clone)")
		{
			audioSync.PlaySound(14);
			Cmd_Hello(this.gameObject, ai_target);
			Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "THANK" + "," + this.name + "," + ai_target.transform.position + "," + ai_target.transform.name);
		}


    //    sayHelpMe = false;


    }

}