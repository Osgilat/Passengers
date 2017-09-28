using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//Used to show clock on left top 
public class DigitalClock : NetworkBehaviour {

	//public SessionID sessionID = new SessionID ();
	[SyncVar]
	public int sessionID;

	void Start (){
		if (isServer)
			sessionID = UnityEngine.Random.Range (0, 100000);
	}

	void OnGUI(){
		DateTime time = DateTime.Now;
		//Format date by padding (added 0)
		String hour = time.Hour.ToString().PadLeft (2, '0');
		String minute = time.Minute.ToString().PadLeft (2, '0');
		String second = time.Second.ToString().PadLeft (2, '0');

        // + "PlayerID: " + .GetComponent<UseTeleport>().playerID
        GUILayout.Label(hour + ":" + minute + ":" + second + " SessionID:  " + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID);
	}


}
