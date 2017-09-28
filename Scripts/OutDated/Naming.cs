using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Naming : NetworkBehaviour {

	/*
	List<string> chars = new List<string>(); //Used to store chars for players

	void Start()
	{
		chars.Add ("A");
		chars.Add ("B");
		chars.Add ("C");
	}

	// Update is called once per frame
	void Update () {

		//Store players in a scene in array


		NamePlayers ();
	}

	[ServerCallback]
	void NamePlayers(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		if (chars.Count != 0 && players.Length != 3) {
			foreach (GameObject player in players) {
				if (player.GetComponentInChildren<TextMesh> ().text == ""){
					int randCharIndex = Random.Range (0, chars.Count);
					CmdNaming (player, chars[randCharIndex]);
					chars.RemoveAt (randCharIndex);
				}
			}
		} 
	}

	[Command]
	void CmdNaming(GameObject player, string naming){
		RpcNaming (player, naming);
	}

	[ClientRpc]
	void RpcNaming(GameObject player, string naming){
		player.GetComponentInChildren<TextMesh> ().text = naming;
		player.name = "Player_" + naming;
	}
	*/
}
