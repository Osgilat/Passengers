using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PushAbility : NetworkBehaviour {

	//Set move and rotation speed after kicked
	public float moveSpeed = 1000000;
	public float rotationSpeed;

	public static bool pushed = false; //Used to control button's behavior

	private Vector3 targetDir;	//
	private Button push;		//Reference to a button
	private AudioSync audioSync;	//Reference to a audioSync script
	private static GameObject pushButton;	//Reference to a button
	private Animator animator;		//Reference to animator

	//Used to initialize values
	void Start(){
		animator = GetComponentInChildren<Animator> ();
		audioSync = GetComponent<AudioSync> ();
		GameObject.FindGameObjectWithTag ("Kick").GetComponentInChildren<Text> ().text = "KICK";


	}


    private ColorBlock theColor;

	//Control button's behavior
	public void ClickToPush(){

        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.kickButton);

        if (!pushed) {
            pushed = true;
            GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.clear;
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

            if (GameObject.FindGameObjectWithTag("HealButton") != null)
            {
                HealAbility.healTrigger = false;
                GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.white;
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
        else {
			pushed = false;
            GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;
        }

       

    }

	//Every tick
	void Update(){
		//Only for local player
		if (!isLocalPlayer) {
			return;
		}

        

        //if left click and button is active
        if (Input.GetMouseButton (0) && pushed) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				//if hitted player
				if (hit.transform.CompareTag ("Player") || hit.transform.CompareTag("DamagedBot")) {
					//Kick across network
					Cmd_CollidersIdentify (this.transform.position, 2.5f, hit.transform.gameObject);

				}
			}
		}

	}

	//Function to AI control
	public void AI_AttackPlayer(GameObject ai_target){

		Cmd_CollidersIdentify(this.transform.position, 3f, ai_target);

	}


	[Command]
	void Cmd_CollidersIdentify(Vector3 center, float radius, GameObject target) {
		//Take all coliders in a radius
         Collider[] hitColliders = Physics.OverlapSphere(center, radius);
         int i = 0;
         while (i < hitColliders.Length) {
			//If player in radius
             if((hitColliders[i].tag =="Player" || hitColliders[i].tag == "DamagedBot") && hitColliders[i].gameObject != gameObject)
                 {
					//then move player across network
					GameObject player = hitColliders[i].gameObject;	
					Rpc_Move (player,target);
					return;
                 }
             i++;
             }
         }

	//Move player across network
	[ClientRpc]
	void Rpc_Move(GameObject player, GameObject target){
		//Calculate push vector
		targetDir = target.transform.position - this.transform.position;
		float step = rotationSpeed * Time.deltaTime;

		//Rotate towards a goal
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);

        //Set animation, sound and deactivate button
       // kickEffect.Play();
        
		animator.SetTrigger ("TriggerAnger");
		audioSync.PlaySound (3);
		pushed = false;
        GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;
        
        /*
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
			if (p.GetComponent<Behaviour>().enabled)
				p.GetComponent<Behaviour>().ListenerActionAvatar("Kick", player, target);
        }
        */

        gameObject.GetComponent<UseTeleport>().NotifyBots("Kick", player, target);


        //Log about kick
        Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + "," + "Kick" + "," + this.name + "," + this.gameObject.transform.position + "," + player.name);

        //Set velocity for target's navmeshAgent
        player.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
		player.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity = transform.forward * 15;
	}
}
