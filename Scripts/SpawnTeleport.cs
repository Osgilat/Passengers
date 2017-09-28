using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnTeleport : NetworkBehaviour {

	public GameObject Platform_1;
	public GameObject Platform_2;
    public GameObject Generator;
    public GameObject UI;

	GameObject p_1;
	GameObject p_2;
	GameObject ui;
    GameObject gen;


	public override void OnStartServer(){
		p_1 = (GameObject) Instantiate(Platform_1, new Vector3(5.24f,0.12f,0.3527f ), Quaternion.AngleAxis(270,Vector3.right));
		p_2 = (GameObject) Instantiate(Platform_2, new Vector3(-4.2f,0.12f,0.3527f ), Quaternion.AngleAxis(270,Vector3.right));
		ui = (GameObject) Instantiate(UI);
        gen = (GameObject) Instantiate(Generator, Generator.transform.position, Generator.transform.rotation);



        NetworkServer.Spawn(ui);

		NetworkServer.Spawn(p_1);

		NetworkServer.Spawn(p_2);

        NetworkServer.Spawn(gen);

	}

}
