using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShootAbility : NetworkBehaviour {

    [SyncVar]
    public bool stunned = false;

    [SyncVar]
    public bool alive = true;  //SyncVars for player state
    [SyncVar]
	public bool hasAmmo = true;  //SyncVars for ammo and lighting
	private bool lightIsON = false;
    private Animator animator;		//Reference to animator
   

    public GameObject bulletPrefab = null;
    public Transform bulletSpawn = null;
    public float rotationSpeed = 120.0f;
    
    public bool locked = false; //bool to check if locked on another player

    public ParticleSystem indicator;        //Particle System above player to indicate who shot
    public GameObject shootLightPrefab; 	//Light effect for shooting other players
	public Transform lightSpawn;			//Spawn point for a light effect
    public Transform diePoint;              

    private Quaternion q;   //used for rotation
	private GameObject lockedPlayer;    //player on which this locked
	private UnityEngine.AI.NavMeshAgent navmeshAgent;   //reference to this navmeshAgent
	private GameObject shootLightLocal;   //Used to take value on Initiate function  
	private Vector3 targetDir;          //Vector to a target player location

	private static bool shootBool = false; //used to shoot bullet in onclick function

    public float timeForHeal = 7.0f; //time to heal player after he stunned

    [SyncVar]
	public int shootPoints = 0; //to check points 

    //Used to initialize values
    private void Start()
    {
        navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public static bool pointed = false;

    public void OnClickPoint()
    {
        gameObject.GetComponent<PlayerActor>().ResetUI(UseTeleport.targetButton);
        if (!pointed)
        {
            pointed = true;
            GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.clear;
            /*
            if (GameObject.FindGameObjectWithTag("Greet") != null)
            {
                PlayerActor.sayHelpMe = false;
                GameObject.FindGameObjectWithTag("Greet").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("ThankButton") != null)
            {
                PlayerActor.sayThank = false;
                GameObject.FindGameObjectWithTag("ThankButton").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("Kick") != null)
            {
                PushAbility.pushed = false;
                GameObject.FindGameObjectWithTag("Kick").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("HealButton") != null)
            {
                HealAbility.healTrigger = false;
                GameObject.FindGameObjectWithTag("HealButton").GetComponent<Image>().color = Color.white;
            }

            if (GameObject.FindGameObjectWithTag("WakeUpButton") != null)
            {
                WakeUpAbility.wakeUp = false;
                GameObject.FindGameObjectWithTag("WakeUpButton").GetComponent<Image>().color = Color.white;
            }
            */
        }
        else
        {
            pointed = false;
            GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.white;
        }
    }

	//Called OnClick on SHOOT button
	public void ClickToShoot()
    {
        shootBool = true;
    }


    
 
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        

        if (!hasAmmo)
        {
            UseTeleport.shootButton.SetActive(false);
            UseTeleport.targetButton.SetActive(false);
        }
        else
        {
            UseTeleport.targetButton.SetActive(true);
        }

        if (stunned)
        {
            shootBool = false;
        }

		if (shootBool) {
			Cmd_Shoot ();
		}

        
        //take a mouse position at a moment
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if locked on a player then rotate to the target player location 
        if (locked)
        {
            q = Quaternion.LookRotation(lockedPlayer.transform.position - transform.position);
            transform.position = transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 200 * Time.deltaTime);
        }

        //if clicked then take raycast
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit))
        {
            //if clicked on ground then unlock bool and player and call StopLight
            if (hit.transform.CompareTag("Ground"))
            	{
                UseTeleport.shootButton.SetActive(false);
                locked = false;
                pointed = false;
                
                GameObject.FindGameObjectWithTag("TargetButton").GetComponent<Image>().color = Color.white;
                
                lockedPlayer = null;
				CmdTurnLightOFF ();
                //else if clicked on player and not locked on anybody then reset path, lock on this player by bool and gameobject
           		 }
            //else if (hit.transform.CompareTag("Player") && !locked && hit.transform.gameObject != gameObject)
            else if (hit.transform.CompareTag("Player") && hit.transform.gameObject != gameObject && pointed)
            {
                pointed = false;
				navmeshAgent.ResetPath ();
                lockedPlayer = hit.transform.gameObject;
                locked = true;
            	}
			}
        //if locked on somebody then take a vector to him and toggle light based on angle 
		if (lockedPlayer != null && hasAmmo)
        {
            // gameObject.GetComponent<UseTeleport>().shootButton.SetActive(true);
            UseTeleport.shootButton.SetActive(true);
            targetDir = lockedPlayer.transform.position - transform.position;
			float angle = Vector3.Angle (targetDir, transform.forward);

            if (angle < 5.0f && !lightIsON 
                && !gameObject.GetComponent<PlayerActor>().lightSpawn.GetComponent<Light>().enabled)
            {
                CmdTurnLightON();
            }
            else if (angle > 5.0f)
            {
                CmdTurnLightOFF();
            }

		}
    }

    [Command]
    void CmdTurnLightON()
    {
        lightSpawn.GetComponent<Light>().enabled = true;
        RpcTurnLightON();
    }

    
    [ClientRpc]
	void RpcTurnLightON()
    {
        lightSpawn.GetComponent<Light>().enabled = true;
    }
    
    [Command]
    void CmdTurnLightOFF()
    {
        lightSpawn.GetComponent<Light>().enabled = false;
        RpcTurnLightOFF();
    }

    [ClientRpc]
	void RpcTurnLightOFF()
    {
        lightSpawn.GetComponent<Light>().enabled = false;
    }

    [Command]
    public void CmdRechargeAmmo()
    {
        hasAmmo = true;
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));
        RpcRechargeAmmo();
    }

    [ClientRpc]
    void RpcRechargeAmmo()
    {
        hasAmmo = true;
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, .5f));
    }

    [Command]
    void Cmd_Shoot()
    {

        if (!hasAmmo)
        {
            return;
        }
		/*
        if (lightIsON)
        {
            shootPoints += 1;
        }
*/
        if (lockedPlayer != null)
        {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + ","
                + "Shoot" + "," + this.name + "," + this.gameObject.transform.position + "," + lockedPlayer.name);
        }
        else
        {
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DigitalClock>().sessionID + "," + this.gameObject.GetComponent<UseTeleport>().playerID + ","
               + "Shoot" + "," + this.name + "," + this.gameObject.transform.position + "," + "missed");
        }


        this.gameObject.GetComponent<AudioSync>().PlaySound(6);
        GameObject obj = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        // setup bullet component
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.velocity = transform.forward;
        // spawn on the clients
        NetworkServer.Spawn(obj);
        // destroy after 2 secs
        Destroy(obj, 2.0f);


        //Set bools 
        shootBool = false;
        locked = false;
        lockedPlayer = null;
        hasAmmo = false;
        //Turn light OFF
        CmdTurnLightOFF();
        //Set indicator to blue
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 1, .5f));
        RpcShoot();

    }

    [ClientRpc]
	void RpcShoot()
    {

        //Set bools 
        shootBool = false;
        locked = false;
        lockedPlayer = null;
        hasAmmo = false;
        //Turn light OFF
        CmdTurnLightOFF();
        //Set indicator to blue
        ParticleSystem.MainModule settings = indicator.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 1, .5f));

    }


    public IEnumerator PlayOneShot(string paramName)
    {
        animator.SetBool(paramName, true);
        yield return null;
        animator.SetBool(paramName, false);
    }

    public void AI_Shoot()
    {
        Cmd_Shoot();
    }

    public void AI_Target(GameObject playerToShoot)
    {
        locked = true;
        pointed = true;
        lockedPlayer = playerToShoot;
    }





}
