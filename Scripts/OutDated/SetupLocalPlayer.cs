using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class SetupLocalPlayer : NetworkBehaviour {

  /*  [SyncVar]
    public string pname = "player";

    void OnGUI()
    {
        if (isLocalPlayer)
        {
          //  pname = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), pname);
            if(GUI.Button(new Rect(130,Screen.height - 40, 80, 30), "Change"))
            {
            //    CmdChangeName(pname);
            }
        }
        
    }

    [Command]
    private void CmdChangeName(string newName)
    {
        pname = newName;
    }
*/

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
			Debug.Log ("isLocalPlayer from LOCAL " + isLocalPlayer);
			GetComponent<PlayerActor>().enabled = true;
			// GetComponent<ShootAbility>().enabled = true;
			GetComponent<PushAbility>().enabled = true;
			GetComponent<UseTeleport>().enabled = true;
		//	SmoothCameraFollow.target = this.transform;
        }
	}

//
//	[SyncVar]
//	bool activated; 
//	GameObject thisTeleport;
//	GameObject otherTeleport;
//
//	[SyncVar]
//	private GameObject objectID;
//
//	//	public bool stay;
//	bool local;
//
//	private NetworkIdentity objNetId;
//
//
//	public void ActivateTeleport(){
//		Debug.Log ("INSIDE ACTIVATE TELEPORT");
//		//		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, .99f);
//		//		Debug.Log (hitColliders.Length);
//		//		Debug.Log (hitColliders[0]);
//		//		Debug.Log (hitColliders[1]);
//		//		int i = 0;
//		//		while (i < hitColliders.Length) {
//		//			Debug.Log("HIT COLLIDERS  " + hitColliders[i]);
//		//			if ((hitColliders [i].tag == "Platform_1" || hitColliders [i].name == "Platform_2")
//		//			   && hitColliders [i].gameObject != gameObject) {
//		//				Debug.Log ("SELECTED COLLIDER" + hitColliders[i]);
//		//				thisTeleport = hitColliders [i].gameObject;
//		//				Debug.Log ("THIS TELEPORT " + thisTeleport);
//		//				otherTeleport = thisTeleport.gameObject
//		//					.GetComponent<Teleportation> ()
//		//					.otherTeleport;
//		//				Debug.Log ("OTHER TELEPORT " + otherTeleport);
//		//				break;
//		//			}
//		//			i++;
//		//		}
//
//
//		Debug.Log ("isServer " + this.GetComponent<NetworkIdentity>().isServer);
//		Debug.Log ("isLocalPlayer " + isLocalPlayer);
//
//		if (this.GetComponent<NetworkIdentity>().isLocalPlayer) {
//			Debug.Log ("INSIDEisLocalPlayer");
//			activated = !activated;
//			Debug.Log (activated);
//			CheckIfOtherIsActive ();
//		}
//	}
//
//	//	void Update(){
//	////		if(isLocalPlayer)
//	////		yield WaitForSecondsRealtime(1.0f);
//	//
//	//	}
//
//	void CheckIfOtherIsActive (){
//		//		if (stay) {
//		objectID = otherTeleport;
//
//		CmdActivateOther (objectID,activated);
//		//		}
//
//	}
//
//	[Command]
//	void CmdActivateOther(GameObject obj, bool active){
//		Debug.Log ("Inside CMD " + active);
//		objNetId = obj.gameObject.GetComponent<NetworkIdentity> ();
//		Debug.Log ("Inside CMD objNetId = " + objNetId);
//		objNetId.AssignClientAuthority (objNetId.connectionToClient);
//		Rpc_ActivateOtherOnClients (obj, active);
//		objNetId.RemoveClientAuthority (objNetId.connectionToClient);
//	}
//
//	[ClientRpc]
//	void Rpc_ActivateOtherOnClients(GameObject obj, bool active){
//		Debug.Log ("Inside RPC " + active);
//		if (active) {
//			obj.GetComponent<ParticleSystem> ().Play ();
//		} else {
//			obj.GetComponent<ParticleSystem> ().Stop ();
//		}
//	}
//
//	// Update is called once per frame
//	void Update () {
//
//	}
//
//	void OnTriggerEnter(Collider other)
//	{
//
//		if (other.gameObject.tag == "Platform_1"){
//			thisTeleport = other.gameObject;
//			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_2");
//		}	else if( other.gameObject.tag == "Platform_2"){
//
//			thisTeleport = other.gameObject;
//			otherTeleport = GameObject.FindGameObjectWithTag ("Platform_2");
//			//			Debug.Log ("OnTriggerEnter");
//			//			//			otherTeleport.SetActive(false);
//			//			//			thisTeleport.SetActive(false);
//			//			activateButton.SetActive(true);
//		}
//	}

  	
}
