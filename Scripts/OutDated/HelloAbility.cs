using UnityEngine;
using System.Collections;

public class HelloAbility : MonoBehaviour {

	public GameObject cube;

	//public Texture2D ab1;
	//public Texture2D ab1CD;
	float helloTimer = 0;
	public float helloCDTime;

	void OnGUI(){

		helloTimer -= Time.deltaTime;
		bool helloKey = Input.GetKeyDown (KeyCode.Q);

		if (helloTimer <= 0) {
			GUI.Button(new Rect(935, 10, 340,125), "Hello");

			if (helloKey) {
				Hello ();
			}
				
		} else {
			GUI.Button(new Rect(935, 10, 340,125), "HelloCD");
		}
	}

	void Hello(){
		Instantiate (cube, Vector3.zero, Quaternion.identity);
		helloTimer = helloCDTime;
	}
}
