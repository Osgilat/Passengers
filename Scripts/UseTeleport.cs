using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class UseTeleport : NetworkBehaviour {

    //PlayerID for identification between sessions
    [SyncVar]
    public string playerID = "noPlayerID";

    [SyncVar]
    public int timesSaved = 0;
   
    public static bool hasFixCharge = true;

	//References to buttons
	public static GameObject askButton;
    public static GameObject kickButton;
	public static GameObject shootButton;
	public static GameObject activateButton;
	public static GameObject takeOffButton;
	public static GameObject saveButton;
	public static GameObject fixButton;
    public static GameObject healButton;
    public static GameObject hybernateButton;
    public static GameObject wakeUpButton;
    public static GameObject targetButton;
    public static GameObject thankButton;
    public static GameObject activateGeneratorButton;

    //References to HUD 
    public GameObject announcementText;
	public GameObject canvas;

	//References to tower positions 
	public GameObject pos_1;
	public GameObject pos_2;

	//Trigger flag
	public static bool trigger = false;

	[SyncVar]
	private GameObject objectID;
	private NetworkIdentity objNetId;

	//Reference to audioSync script
	private AudioSync audioSync;

	//Reference to teleport system 
	private static GameObject thisTeleport;
	private static GameObject otherTeleport;

	//Flags for player's states
	private static bool activateTrigger = false;
	public static bool takeOffTrigger = false; 
	public static bool saveOther = false;
	public static bool escapeTrigger = false;

    public static bool fixTrigger = false;
    public static bool hybernateTrigger = false;
    public static bool activeGeneratorTrigger = false;


    //Initialiize variables
    void Start(){

       	InstantiateButtons (canvas);
		audioSync = GetComponent<AudioSync> ();
   	}

	//Function for buttons and announcement
	void InstantiateButtons(GameObject canvas){

		//Only for local player
		if (!isLocalPlayer) {
			return;
		}

		//Instantiate canvas and get references to it's buttons and announcement text
		Transform thisCanvas = Instantiate (canvas).transform;
		askButton =  thisCanvas.GetChild (0).gameObject; 
		kickButton =  thisCanvas.GetChild (1).gameObject; 
		activateButton = thisCanvas.GetChild (2).gameObject; 
		takeOffButton = thisCanvas.GetChild (3).gameObject; 
		saveButton = thisCanvas.GetChild (4).gameObject; 
		fixButton = thisCanvas.GetChild (5).gameObject;
		announcementText = thisCanvas.GetChild (6).gameObject;
		shootButton = thisCanvas.GetChild (7).gameObject;
        healButton = thisCanvas.GetChild(8).gameObject;
        hybernateButton = thisCanvas.GetChild(9).gameObject;
        wakeUpButton = thisCanvas.GetChild(10).gameObject;
        targetButton = thisCanvas.GetChild(11).gameObject;
        thankButton = thisCanvas.GetChild(12).gameObject;
        activateGeneratorButton = thisCanvas.GetChild(13).gameObject;


        //Deactivate all buttons 
        activateButton.SetActive(false);
		takeOffButton.SetActive(false);
		saveButton.SetActive(false);
		fixButton.SetActive(false);
        hybernateButton.SetActive(false);
        activateGeneratorButton.SetActive(false);

		
		announcementText.GetComponent<Text>().text = "You are player " + gameObject.GetComponent<Text>().text;

		//Set announcement active for a first round
		announcementText.SetActive(true);
	}




    public static bool inGeneratorTrigger = false;

    void Update () {
		//Only for local player 
		if (isLocalPlayer) {

            if (inGeneratorTrigger && (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().m_TriggerList).Contains(gameObject.GetComponent<Collider>()) && hasFixCharge)
            {
                fixButton.SetActive(true);
            } else
            {
                fixButton.SetActive(false);
            }

            if ((GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().m_TriggerList).Contains(gameObject.GetComponent<Collider>()) && GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>().repaired)
            {
                hybernateButton.SetActive(true);
                activateGeneratorButton.SetActive(true);
                saveButton.SetActive(false);
            }



            if (fixTrigger && hasFixCharge)
            {
                fixTrigger = false;
                hasFixCharge = false;
                UseTeleport.fixButton.SetActive(false);
                CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
            }

            if(activeGeneratorTrigger)
            {
                activeGeneratorTrigger = false;
                UseTeleport.activateGeneratorButton.SetActive(false);
                CmdActivateGenerator(GameObject.FindGameObjectWithTag("Generator"));
            }


            //if pushed take off
            if (takeOffTrigger && !trigger)
            {
				trigger = true;
				Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "TakeOff" + "," + this.gameObject.name + "," + this.gameObject.transform.position);
                /*
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetComponent<Main>().enabled)
                        player.GetComponent<Main>().ListenerActionAvatar("TakeOff", this.gameObject, this.gameObject);
                }
                */
                //Transform player to first position
                Cmd_TransformPlayer (this.transform.gameObject, pos_1.transform.position, pos_1.transform.rotation, false);
				//Deactivate all buttons except save and escape
				askButton.SetActive (false);
				kickButton.SetActive (false);
				saveButton.SetActive (true);
				//fixButton.SetActive (true);
			}

            //if hybernate button pushed
            if (hybernateTrigger)
            {
                if(gameObject.GetComponent<HybernationSystem>().hybernationEffect.isStopped)
                gameObject.GetComponent<HybernationSystem>().hybernationEffect.Play();
                //if clicked 
                if (Input.GetMouseButton(0))
                {

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    
                    if (Physics.Raycast(ray, out hit))
                    {
                        
                        //if hitted player
                        if (hit.transform.CompareTag("Player") && hit.transform.gameObject != gameObject)
                        {
                            //Log saved 
                            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + ","
                                + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Hybernate" + "," + this.gameObject.name + ","
                                + hit.transform.position + "," + hit.transform.gameObject.name);

                            /*
                            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (player.GetComponent<Main>().enabled)
                                    player.GetComponent<Main>().ListenerActionAvatar("Saved", this.gameObject, hit.transform.gameObject);
                            }
                            */
                            //Transform hitted player
                            GameObject[] spawnPoints = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().spawnPoints;

                            RespawnPlayers(hit.transform.gameObject, spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position, 
                                spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.rotation, false);

                            //hit.transform.gameObject.GetComponent<HybernationSystem>().
                                CmdHybernate(hit.transform.gameObject);

                            gameObject.GetComponent<HybernationSystem>().hybernationEffect.Stop();
                           // hybernateButton.SetActive(false);
                            hybernateTrigger = false;

                            /*
							//Deactivate tower buttons
							saveButton.SetActive (false);
							escapeButton.SetActive (false);
                            */
                            //Reset triggers
                            saveOther = false;
                            escapeTrigger = false;
                        }
                    }
                }
            } else
            {
                gameObject.GetComponent<HybernationSystem>().hybernationEffect.Stop();
            }

            //if save button pushed
            if (saveOther) {
				//if clicked 
				if (Input.GetMouseButton (0)) {

					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

					if (Physics.Raycast (ray, out hit)) {

                        //if hitted player
                        if (hit.transform.CompareTag("Player")
                            &&
                            !hit.transform.gameObject.GetComponent<HybernationSystem>().isHybernated()
                            
                            ) {
							//Log saved info
							//Debug.Log (this.gameObject.name + " saved " + hit.transform.gameObject.name 
							//	+ " " + hit.transform.position + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID);
                            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Saved" + "," + this.gameObject.name + "," + hit.transform.position + "," + hit.transform.gameObject.name);
                            /*
                            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (player.GetComponent<Main>().enabled)
                                    player.GetComponent<Main>().ListenerActionAvatar("Saved", this.gameObject, hit.transform.gameObject);
                            }
                            */

                            //Transform hitted player
                            Cmd_TransformPlayer (hit.transform.gameObject, pos_2.transform.position, pos_2.transform.rotation, false);
                            /*
							//Deactivate tower buttons
							saveButton.SetActive (false);
							escapeButton.SetActive (false);
                            */
                            //Reset triggers
                            saveButton.SetActive(false);
                            saveOther = false;
							escapeTrigger = false;
						}
					}
				}
			}

			//if activate button pushed 
			if (activateTrigger) {
				//Reset trigger
				activateTrigger = false;
				//Get reference to other Teleport
				objectID = otherTeleport;
				//Log information about activation
				//Debug.Log (gameObject.name + " activated teleport " + gameObject.transform.position + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID);
                Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Activated" + "," + this.gameObject.name + "," + gameObject.transform.position);
                //Activate other teleport
                CmdActivateOtherTeleport (objectID);
			}
			//if escape button pushed 
			if (escapeTrigger) {
				//Reset trigger
				escapeTrigger = false;
				//Log information about escape
				//Debug.Log (gameObject.name + " escaped alone" + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID);
                Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Escape" + "," + this.gameObject.name);
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                //For each game manager activate escape button
                foreach (GameObject manager in GameObject.FindGameObjectsWithTag ("GameManager")) {
					CmdActivateEscapeButton (manager);
				}
			}
		}
	}

    public void NotifyBots(string action, GameObject actor, GameObject target = null)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<Behaviour>().enabled)
                player.GetComponent<Behaviour>().ListenerActionAvatar(action, actor, target);
        }
    }

    [Command]
    public void CmdHybernate(GameObject obj)
    {
       
            objNetId = obj.GetComponent<NetworkIdentity>();
            objNetId.AssignClientAuthority(connectionToClient);
            RpcHybernate(obj);
            objNetId.RemoveClientAuthority(connectionToClient);
        
    }


    [ClientRpc]
    public void RpcHybernate(GameObject obj)
    {
        obj.GetComponent<HybernationSystem>().hybernated = true;
    }

    [Command]
    public void CmdDisableHybernate(GameObject obj)
    {
        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcDisableHybernate(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    public void RpcDisableHybernate(GameObject obj)
    {
        obj.GetComponent<HybernationSystem>().hybernated = false;
    }


    [Command]
    void CmdFixGenerator(GameObject obj)
    {
        
        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcFixGenerator(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    //Rpc other teleport
    [ClientRpc]
    void RpcFixGenerator(GameObject obj)
    {
        
      //  Debug.Log(GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>().repairPoints);
        obj.GetComponent<Generator>().repairPoints -= 1;
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + ","
            + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "FIX" + "," + this.gameObject.name);
    }

    [Command]
    void CmdActivateGenerator(GameObject obj)
    {

        objNetId = obj.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcActivateGenerator(obj);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    //Rpc other teleport
    [ClientRpc]
    void RpcActivateGenerator(GameObject obj)
    {

        //  Debug.Log(GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>().repairPoints);
        obj.GetComponent<Generator>().active = true;
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + ","
            + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "ActivateGenerator" + "," + this.gameObject.name);
    }


    //Send other teleport  object to a server and then rpc on all the clients
    [Command]
	void CmdActivateOtherTeleport (GameObject obj){
		
		objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority (connectionToClient);
		Rpc_ActivateOtherTeleport (obj);
		objNetId.RemoveClientAuthority (connectionToClient);
	}

	//Rpc other teleport
	[ClientRpc]
	void Rpc_ActivateOtherTeleport(GameObject obj){
		//Get particle system from received object
		ParticleSystem teleport = obj.GetComponent<ParticleSystem> ();
		//if particle system is active then stop and clear
		if (teleport.isPlaying) {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Deactivated" + "," + this.gameObject.name + "," + gameObject.transform.position);

            

            teleport.Stop ();
			teleport.Clear ();
		//if stopped then set active
		} else if (teleport.isStopped) {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Activated" + "," + this.gameObject.name + "," + gameObject.transform.position);
            NotifyBots("Activated", gameObject, otherTeleport);
            /*
			foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (player.GetComponent<Behaviour>().enabled)
					player.GetComponent<Behaviour>().ListenerActionAvatar("Activated", gameObject, otherTeleport);
			}
            */
            teleport.Play ();
		}
	}

	//Send other teleport  object to a server and then rpc on all the clients
	[Command]
	void CmdStopOtherTeleport (GameObject obj){
		objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority (connectionToClient);
		Rpc_StopOtherTeleport (obj);
		objNetId.RemoveClientAuthority (connectionToClient);
	}

	//Deactivate other teleport by stopping and clear particle system
	[ClientRpc]
	void Rpc_StopOtherTeleport(GameObject obj){
		ParticleSystem teleport = obj.GetComponent<ParticleSystem> ();

		if (teleport.isPlaying) {
			teleport.Stop ();
			teleport.Clear ();
		}
	}


    public void RespawnPlayers(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd) {
        if (roundEnd && isLocalPlayer && GetComponent<HybernationSystem>().isHybernated())
        {
            audioSync.PlayLocalSound(7);
        }
        else if(roundEnd && isLocalPlayer && !GetComponent<HybernationSystem>().isHybernated()) {
            audioSync.PlayLocalSound(5);
        }
        Cmd_TransformPlayer(obj, position, rotation, roundEnd);
    }

	//Transform player on a server
	[Command]
	public void Cmd_TransformPlayer(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd){
        //if not round end then add times saved
        if (!roundEnd)
        obj.GetComponent<UseTeleport>().timesSaved += 1;
        //Warp to a new position 
		obj.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);
		//Disable navMeshAgent
		//obj.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
        //Set fixed transform rotation
        //obj.transform.eulerAngles = new Vector3(0, 45, 0);
        obj.transform.rotation = rotation;
		//if not end of a round then activate teleport sound 
		if (!roundEnd) {

			//audioSync.PlaySound (4);
            //teleportEffect.Play();
			//else activate roundEnd sound
		} else {
			//audioSync.PlaySound (5);
			//enable navMeshAgent
			obj.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
			//Disable announcement text
			announcementText.SetActive (false);
		}
		//Call function on clients
		Rpc_TransformPlayer (obj, position, rotation,roundEnd);
	}

	//Transform player on a clients
	[ClientRpc]
	public void Rpc_TransformPlayer(GameObject obj, Vector3 position, Quaternion rotation, bool roundEnd){
		//Warp to a new position 
		obj.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(position);
		//Disable navMeshAgent
		//obj.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
        //Set fixed transform rotation
        //obj.transform.eulerAngles = new Vector3(0, 45, 0);
        obj.transform.rotation = rotation;
        //if not end of a round then activate teleport sound 
        if (!roundEnd) {
			//audioSync.PlaySound (4);
			//else activate roundEnd sound
		} else {
			//audioSync.PlaySound (5);
			//enable navMeshAgent
			obj.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
			//Disable announcement text
			announcementText.SetActive (false);
		}
	}

	//Call escape for a player on a server
	[Command]
	void CmdActivateEscapeButton(GameObject obj){
		objNetId = obj.GetComponent<NetworkIdentity> ();
		objNetId.AssignClientAuthority (connectionToClient);
		RpcActivateEscapeButton (obj);
		objNetId.RemoveClientAuthority (connectionToClient);
	}

	//Set escaped trigger to true
	[ClientRpc]
	void RpcActivateEscapeButton(GameObject manager){

       // GameManager.escaped = true;
	}

	//if player entered collider
	void OnTriggerEnter(Collider other)
	{
		//Only for local player activate button
		if(isLocalPlayer)
		if(other.gameObject.tag == "Platform_1" || other.gameObject.tag == "Platform_2"){
			activateButton.SetActive(true);
		}

      

		//if entered platform 1 then reassign variables
		if (other.gameObject.tag == "Platform_1") {
			thisTeleport = other.gameObject;
			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_2");
			//Log information to a server
		    Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "enterTP" + "," + this.gameObject.name + "," + thisTeleport.gameObject.transform.position);
            //Do same for other teleport
        } else if (other.gameObject.tag == "Platform_2") {

			thisTeleport = other.gameObject;
			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_1");
			//Debug.Log (this.name + " entering " + thisTeleport.name + " sessionID = " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID);
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "enterTP" + "," + this.gameObject.name + "," + thisTeleport.gameObject.transform.position);
        }
	}

	//When player stay on teleport
	void OnTriggerStay(Collider other)
	{
		//if local player trigger buttons based on teleport state
		if(isLocalPlayer)
		if (other.gameObject.tag == "Platform_1"){
			thisTeleport = other.gameObject;
			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_2");
			if (thisTeleport.GetComponent<ParticleSystem> ().isPlaying) {
				takeOffButton.SetActive (true);
			} else if (thisTeleport.GetComponent<ParticleSystem> ().isStopped) {
				takeOffButton.SetActive (false);
			}

		}	else if( other.gameObject.tag == "Platform_2"){

			thisTeleport = other.gameObject;
			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_1");

			if (thisTeleport.GetComponent<ParticleSystem> ().isPlaying) {
				takeOffButton.SetActive (true);
			} else if (thisTeleport.GetComponent<ParticleSystem> ().isStopped) {
				takeOffButton.SetActive (false);
			}
		}
	}

	//if exited teleport
	void OnTriggerExit(Collider other)
	{
        //Only for local player 
        if (isLocalPlayer || this.gameObject.GetComponent<Behaviour>().enabled || this.gameObject.GetComponent<DamagedBotBehavior>().enabled)
            if (other.gameObject.tag == "Platform_1" || other.gameObject.tag == "Platform_2")
            {
                //Deactivate teleport buttons 
                if (isLocalPlayer) { 
                takeOffButton.SetActive(false);
                activateButton.SetActive(false);
                }
                //Log information about exiting
                Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "exitTP" + "," + this.gameObject.name + "," + this.gameObject.transform.position);

                //Send command to a server to stop teleport 
                objectID = otherTeleport;
                CmdStopOtherTeleport(objectID);
                /*
                //BUG
                if (this.gameObject.GetComponent<Behaviour>().enabled)
                {
                    //|| this.gameObject.GetComponent<AI_2>().enabled) {
                    objectID = thisTeleport;
                    CmdStopOtherTeleport(objectID);
                }
                */
                //Reset triggers
                thisTeleport = null;
                otherTeleport = null;
            }

       
    }

	//Triggers button
	public void ActivateTeleportOnClick(){
		activateTrigger = true;
	}

	//Triggers button
	public void ActivateSaveButton(){
        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.saveButton);

        if (!saveOther)
        {
            saveOther = true;
            GameObject.FindGameObjectWithTag("Save").GetComponent<Image>().color = Color.clear;
        }
        else
        {
            saveOther = false;
            GameObject.FindGameObjectWithTag("Save").GetComponent<Image>().color = Color.white;
        }
    }

    public void AI_ActivateSaveButton()
    {
        saveOther = true;
        
    }

        //Triggers button
    public void ActivateEscapeButton(){
		escapeTrigger = true;
	}


	//Triggers button
	public void ActivateTakeOffButton(){
		takeOffTrigger = true;
	}


    public void OnFixClick()
    {
        fixTrigger = true;
        
    }




    public void ActivateGenerator()
    {

        activeGeneratorTrigger = true;

    }



    public void OnHybernateClick()
    {
        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.hybernateButton);


        if (!hybernateTrigger)
        {
            hybernateTrigger = true;
            UseTeleport.hybernateButton.GetComponent<Image>().color = Color.clear;

        }
        else
        {
            hybernateTrigger = false;
            UseTeleport.thankButton.GetComponent<Image>().color = Color.white;
        }
    }

    public void AI_Fix()
    {
        CmdFixGenerator(GameObject.FindGameObjectWithTag("Generator"));
    }

	public void AI_ActivateGenerator()
	{
		CmdActivateGenerator(GameObject.FindGameObjectWithTag("Generator"));
	}




        //Used for AI actions triggering
        public void AI_ActivateTeleport(GameObject self)
    {

        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + self.GetComponent<UseTeleport>().playerID + "," + "Activated" + "," + self.name + "," + self.transform.position);

        objectID = otherTeleport;
        CmdActivateOtherTeleport(objectID);
        // Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "TakeOff" + "," + this.gameObject.name + "," + this.gameObject.transform.position);
        //  CmdActivateOtherTeleport(otherTeleport)
    }

    //Used for AI actions triggering
    public void AI_ActivateTakeOffButton(){
		
		if (thisTeleport.GetComponent<ParticleSystem> ().isPlaying || otherTeleport.GetComponent<ParticleSystem> ().isPlaying) {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "TakeOff" + "," + this.gameObject.name + "," + this.gameObject.transform.position);
            Cmd_TransformPlayer(this.gameObject, pos_1.transform.position, pos_1.transform.rotation, false);
            
        }
	}



	//Used for AI actions triggering
	public void AI_ActivateSaveButton(GameObject playerToSave){

        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Saved" + "," + this.gameObject.name + "," + playerToSave.transform.position + "," + playerToSave.name);
        Cmd_TransformPlayer (playerToSave, pos_2.transform.position, pos_2.transform.rotation, false);

	}

    public void AI_Hybernate(GameObject target)
    {
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + ","
            + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Hybernate" + "," + this.gameObject.name + ","
            + target.transform.position + "," + target.transform.gameObject.name);

        /*
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<Behaviour>().enabled)
                player.GetComponent<Behaviour>().ListenerActionAvatar("Hybernated", this.gameObject, target.transform.gameObject);
        }
        */

        NotifyBots("Hybernated", this.gameObject, target.transform.gameObject);
        //Transform hitted player
        GameObject[] spawnPoints = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().spawnPoints;

        RespawnPlayers(target.transform.gameObject, spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.position,
            spawnPoints[(UnityEngine.Random.Range(0, spawnPoints.Length))].transform.rotation, false);

        //hit.transform.gameObject.GetComponent<HybernationSystem>().
        CmdHybernate(target.transform.gameObject);
    }

}



