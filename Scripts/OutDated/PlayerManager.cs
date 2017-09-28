using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class PlayerManager : MonoBehaviour {

		// This class is to manage various settings on a tank.
		// It works with the GameManager class to control how the tanks behave
		// and whether or not players have control of their tank in the 
		// different phases of the game.

 
		[HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
		[HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
		[HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
		[HideInInspector] public int m_Wins;                    // The number of wins this player has so far.


		private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.


		public void Setup ()
		{
			// Get references to the components.
			m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;

			
		}


		// Used during the phases of the game where the player shouldn't be able to control their tank.
		public void DisableControl ()
		{
			m_CanvasGameObject.SetActive (false);
		}

			// Used at the start of each round to put the tank into it's default state.
//		public void Reset ()
//		{
//			m_Instance.transform.position = m_SpawnPoint.position;
//			m_Instance.transform.rotation = m_SpawnPoint.rotation;
//
//			m_Instance.SetActive (false);
//			m_Instance.SetActive (true);
//		}
}
