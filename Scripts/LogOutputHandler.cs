﻿/// <summary>
/// Basic controller that takes logs from Unity's debug.log function and sends output to Loggly
/// </summary>
/// USAGE: Simply put this script in your scripts folder and it will operate.
/// Created by Mike Turner of Charmed Matter Games.

using UnityEngine;
using System.Collections;

public class LogOutputHandler : MonoBehaviour {

	//Register the HandleLog function on scene start to fire on debug.log events 
	void Awake(){
		Application.RegisterLogCallback(HandleLog);
	} 
	
	//Create a string to store log level in
	string level = "";
	
	//Capture debug.log output, send logs to Loggly
	public void HandleLog(string logString, string stackTrace, LogType type) {
	
		//Initialize WWWForm and store log level as a string
		level = type.ToString ();
		var loggingForm = new WWWForm();
		
		//Add log message to WWWForm
		loggingForm.AddField("LEVEL", level);
		loggingForm.AddField("Message", logString);
		loggingForm.AddField("Stack_Trace", stackTrace);
		
        	//Add any User, Game, or Device MetaData that would be useful to finding issues later 
		loggingForm.AddField("Device_Model", SystemInfo.deviceModel); 
		
		//Send WWW Form to Loggly, replace TOKEN with your unique ID from Loggly
		Debug.Log("Loggly working");
		var sendLog = new WWW("http://logs-01.loggly.com/inputs/e6c1b146-4a82-4807-8f80-76c68b986641/tag/http/", loggingForm);
	}
}
