using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerTeleport : NetworkBehaviour {

	GameObject teleport_1; 
	GameObject teleport_2;
	GameObject activateButton;
	GameObject takeOffButton;

	
	
	// [SyncVar]
	private static bool isActive = false;
	[SyncVar] private GameObject objectID;
	private NetworkIdentity objNetId;
	// public GameObject vfx;

	// Use this for initialization
	// public GameObject hand;
    // void Example() {
    //     hand = GameObject.Find("Hand");
    //     hand = GameObject.Find("/Hand");
    //     hand = GameObject.Find("/Monster/Arm/Hand");
    //     hand = GameObject.Find("Monster/Arm/Hand");
    // }
	void Start () {
		// otherTeleportTrigger = otherTeleport.GetComponent<BoxCollider>();
		// thisTeleportArea = this.GetComponent<BoxCollider>();
		// Button activation = activate.GetComponent<Button>();	
		// activate.GetComponent<Button>().OnClick.AddListener(ActivateOtherTeleport);

		// foreach (Transform r in Object.FindObjectsOfType(typeof(Transform)) as Transform[]) 
		// {
		// 	switch (r.gameObject.name)
		// 	{
		// 		case ("Teleporter_1"): teleport_1 = r.gameObject; break;
		// 		case ("Teleporter_2"): teleport_2 = r.gameObject; break;
		// 		case ("Activate"): activateButton = r.gameObject; break;
		// 		case ("Take Off"): takeOffButton = r.gameObject; break;
		// 	}

			
			
		// }


		teleport_1 = GameObject.Find("Teleporter_1");
		teleport_1.SetActive(false);
	
		teleport_2 = GameObject.Find("Teleporter_2");
		teleport_2.SetActive(false);
		
		activateButton = GameObject.Find("Activate");
		activateButton.SetActive(false);
		
		takeOffButton = GameObject.Find("Take Off");
		takeOffButton.SetActive(false);

		Debug.Log(teleport_1.name);
		Debug.Log(teleport_2.name);
		Debug.Log(activateButton.name);
		Debug.Log(takeOffButton.name);
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(isLocalPlayer){

		}	
	}


	//  [Command]
	public void CmdActivateOtherTeleport(){
		// if (!isLocalPlayer)
		// 	return;
	
		Debug.Log("CmdActivateOtherTeleport");
	    isActive = !isActive;
		Debug.Log("Activated - " + isActive);
		// scriptTp.activated = !scriptTp.activated; 
		// Teleportation.activated = !Teleportation.activated;
		// thisTeleport.SetActive(false);
	}
	
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

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	// void OnCollisionEnter(Collision other)
	// {

	// 	if(other.gameObject.tag == "Platform_1"){
	// 		teleport_2.SetActive(false);
	// 		activateButton.SetActive(true);
	// 	} else if(other.gameObject.tag == "Platform_2"){
	// 		teleport_1.SetActive(false);
	// 		activateButton.SetActive(true);
	// 	}
	// }

    /// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Platform_1"){
			// teleport_2.SetActive(false);
			Debug.Log("Inside OnTriggerEnter for 1");
			activateButton.SetActive(true);
		} else if(other.gameObject.tag == "Platform_2"){
			// teleport_1.SetActive(false);
			Debug.Log("Inside OnTriggerEnter for 2");
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
		if(other.gameObject.tag == "Platform_1"){
			if(teleport_1.activeSelf){
			takeOffButton.SetActive(true);
		} else {
			takeOffButton.SetActive(false);
		}
			if (isActive)
			Debug.Log("isActive inside TriggerStay" + isActive);
			// Cmd_Activate_2_teleport(isActive);
			
		} else if(other.gameObject.tag == "Platform_2"){

		if(teleport_2.activeSelf){
			takeOffButton.SetActive(true);
		} else {
			takeOffButton.SetActive(false);
		}

			// Cmd_Activate_1_teleport(isActive);
		}
	}

	// [Command]
	// void Cmd_Activate_1_teleport(isActive){
	// 	teleport_1.SetActive(isActive);
	// }

	// [Command]
	// void Cmd_Activate_2_teleport(isActive){
	// 	teleport_2.SetActive(isActive);
	// }

	 /// <summary>
	/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Platform_1" || other.gameObject.tag == "Platform_2" ){
			teleport_1.SetActive(false);
			teleport_2.SetActive(false);
			activateButton.SetActive(false);
			takeOffButton.SetActive(false);
			isActive = false;
			}
	}
}
