using UnityEngine;
using System.Collections;

public class Progress : MonoBehaviour
{
    public float barDisplay; //current progress
    public float barDisplay_A; //current progress
    public float barDisplay_B; //current progress
    public float barDisplay_C; //current progress
    public Vector2 pos = new Vector2(20, 40);
    public Vector2 size = new Vector2(60, 20);
    private Texture2D emptyTex;
    private Texture2D fullTex;
    private GUIStyle currentStyle = null;
    private GameObject playerA;
    private GameObject playerB;
    private GameObject playerC;
    private GameObject gameManager;
    private bool initBool = false;

    void Start() {
        Invoke("Initialization", 1f);

    }

    void Initialization() {

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
            else
            {
                playerC = player;
            }
        }
        initBool = true;
    }

    Rect screenRelativeRect(float left, float top, float width, float height)
    {
        Rect r = new Rect(Screen.width * left, Screen.height * top, Screen.width * width, Screen.height * height);
        return r;
    }


    void OnGUI()
    {
        InitStyles();

        //draw the background:
        //GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.BeginGroup(new Rect((Screen.width / 2) - pos.x, (Screen.height / 2) - pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), "End of session", currentStyle);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();

        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y - 30f, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), "Player C", currentStyle);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay_C, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();

        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y - 60f, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), "Player B", currentStyle);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay_B, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();

        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y - 90f, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), "Player A", currentStyle);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay_A, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();




    }

    

   
       // GUI.Box(new Rect(0, 0, 100, 100), "Hello", currentStyle);
   

    private void InitStyles()
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.5f));
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


    void Update()
    {
        if (!initBool) {
            return;
        }
        barDisplay_A = playerA.GetComponent<UseTeleport>().timesSaved * 0.01f;
        barDisplay_B = playerB.GetComponent<UseTeleport>().timesSaved * 0.01f;
        barDisplay_C = playerC.GetComponent<UseTeleport>().timesSaved * 0.01f;


        barDisplay = gameManager.GetComponent<GameManager>().timeLeftInGame / 1200;
        //for this example, the bar display is linked to the current time,
        //however you would set this value based on your desired display
        //eg, the loading progress, the player's health, or whatever.
        //barDisplay = Time.time * 0.05f;
        //        barDisplay = MyControlScript.staticHealth;
    }
}