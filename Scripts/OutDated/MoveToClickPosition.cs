using UnityEngine;
using System.Collections;

public class MoveToClickPosition : MonoBehaviour {
	UnityEngine.AI.NavMeshAgent agent;

	void Start(){
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
				agent.destination = hit.point;
			}
		}
	}
}