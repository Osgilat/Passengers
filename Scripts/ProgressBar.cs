using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    Image foregroundImage;
    public Image playerImageA;
    public Image playerImageB;
    public Image playerImageC;
    public Image playerImageD;
    private GameObject playerA;
    private GameObject playerB;
    private GameObject playerC;
    private GameObject playerD;
    GameObject gameManager;
    private bool initBool = false;

    void Start()
    {
        Invoke("Initialization", 5f);

        foregroundImage = gameObject.GetComponent<Image>();
    }

    void Initialization()
    {

        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.name == "PlayerMod_A(Clone)")
            {
                playerA = player;
            }
            else if (player.name == "PlayerMod_B(Clone)")
            {
                playerB = player;

            }
            else if (player.name == "PlayerMod_C(Clone)")
            {
                playerC = player;
            } else
            {
                playerD = player;
            }
        }

        initBool = true;

    }
       

    private void Update()
    {
        if (!initBool || GameObject.FindGameObjectsWithTag("Player").Length < 4)
        {
            return;
        }

        foregroundImage.fillAmount = gameManager.GetComponent<GameManager>().timeLeftInGame / 600;

		playerImageA.fillAmount = playerA.GetComponent<ShootAbility>().shootPoints * 0.02f;
		playerImageB.fillAmount = playerB.GetComponent<ShootAbility>().shootPoints * 0.02f;
		playerImageC.fillAmount = playerC.GetComponent<ShootAbility>().shootPoints * 0.02f;
        playerImageD.fillAmount = playerC.GetComponent<ShootAbility>().shootPoints * 0.02f;
    }
}


