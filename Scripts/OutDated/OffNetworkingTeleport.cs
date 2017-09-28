using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;


public class OffNetworkingTeleport : NetworkBehaviour {

	public GameObject thisTeleport; 
	public GameObject otherTeleport;
	public GameObject activateButton;
	public GameObject takeOffButton;
	
	[SyncVar]
	bool activated;

	// private NetworkIdentity objNetId;
	// NetworkConnection clientConnection;

	// Use this for initialization
	void Start () {
		// NetworkClient networkClient = new NetworkClient();
		// clientConnection = networkClient.connection;
		// otherTeleportTrigger = otherTeleport.GetComponent<BoxCollider>();
		// thisTeleportArea = this.GetComponent<BoxCollider>();
		// Button activation = activate.GetComponent<Button>();	
		// activate.GetComponent<Button>().OnClick.AddListener(ActivateOtherTeleport);
	}

	// public void ActivateTeleport(){
	// 	activated = !activated;
	// }

	
	// public void TakeOffAction(){
		
	// 	vfx.SetActive(true);
	// 	Instantiate(vfx, thisTeleport.transform.position, thisTeleport.transform.rotation);
	// 	vfx.SetActive(false);
	// }

	// void Update(){
	// 	if(activated)
	// 	otherTeleport.SetActive(true);
	// 	else
	// 	{
	// 		otherTeleport.SetActive(false);
	// 	}
	// }

	public void ActivateOtherTeleport(){
		activated = !activated;
		// if(isServer)
		// Rpc_ActivateOtherOnClients(activated);
		// if(isLocalPlayer)

		// Cmd_ActivateOtherOnServer(otherTeleport, activated);
	}

	// [ClientRpc]
	// void Rpc_ActivateOtherOnClients(bool activate){
	// 	activated = activate;
	// }

	// [Command]
	// void Cmd_ActivateOtherOnServer(GameObject obj, bool activate){
	// 	objNetId = obj.GetComponent<NetworkIdentity>();
	// 	objNetId.AssignClientAuthority(clientConnection);
	// 	Rpc_ActivateOtherOnClients(activated);
	// 	objNetId.RemoveClientAuthority (clientConnection); 
	// }

    /// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player"){
			otherTeleport.SetActive(false);
			thisTeleport.SetActive(false);
			activateButton.SetActive(true);
		}
	}

	/// <summary>
	/// OnTriggerStay is called once per frame for every Collider other
	/// that is touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player"){

			if(thisTeleport.activeSelf){
			takeOffButton.SetActive(true);
		} else {
			takeOffButton.SetActive(false);
		}

			otherTeleport.SetActive(activated);
	
		}
	}

	 /// <summary>
	/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player" ){
			thisTeleport.SetActive(false);
			otherTeleport.SetActive(false);
			activateButton.SetActive(false);
			takeOffButton.SetActive(false);
			activated = false;
			}
	}
	
}