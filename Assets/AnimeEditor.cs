using UnityEngine;
using System.Collections;

public class AnimeEditor : MonoBehaviour {
    public bool Hide = true;
    public float FrameSlider = 0f;
    public string sFrame = "";
    public string sFrameC = "";
    public string speed = "";
    VectorString PositionText = new VectorString(0, 0, 0);
    VectorString RotationText = new VectorString(0, 0, 0);
    VectorString ScaleText = new VectorString(0, 0, 0);
    GameObject Piece;
    RoboBuild RB;
    string AnimeID = "";
    int guilock = 0;
    float prevValue;
    // Use this for initialization
    void Start () {
        RB = GetComponent<RoboBuild>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {

        
       if (!Hide)
        {
            if (guilock == 0)
                GUI_AnimeID();
            else
            {
                GUI_FrameSlider();
                GUI_BoneMenu();
            }
        }
        
    }

    void GUI_AnimeID()
    {
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height/2 - 50, 200, 100), "Enter Animation ID");
        AnimeID = GUI.TextField(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 10, 50, 20), AnimeID);
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 100, 20), "Load")) { RB.LoadRoboAnime(AnimeID); guilock = 1; }
    }

    void GUI_FrameSlider()
    {
        //frame slider
        GUI.Box(new Rect(Screen.width / 2 - 210, Screen.height - 100, 420, 70), "Frame Slider");


        GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 75, 50, 20), "Frame:");
        sFrame = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height - 75, 40, 20), sFrame);
        if (sFrame != sFrameC)
            float.TryParse(sFrame, out FrameSlider);
        
        sFrameC = sFrame;
        FrameSlider = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 200, Screen.height - 50, 400, 30), FrameSlider, 0f, 150f);
        sFrame = Mathf.RoundToInt(FrameSlider).ToString();
        if (prevValue != FrameSlider)
            RB.AnimeFrameGo(Mathf.RoundToInt(FrameSlider));

        prevValue = FrameSlider;
        if (GUI.Button(new Rect(Screen.width / 2 + 90, Screen.height - 75, 70, 20), "Play")) { }

        GUI.Label(new Rect(Screen.width / 2 - 35, Screen.height - 75, 50, 20), "Speed:");
        speed = GUI.TextField(new Rect(Screen.width / 2 + 15, Screen.height - 75, 40, 20), speed);
    }

    void GUI_BoneMenu()
    {
        //content box
        GUI.Box(new Rect(10, Screen.height / 2 - 200, 275, 400), "Animation Editor");

        //transform data
        GUI.Label(new Rect(30, Screen.height / 2 - 180, 50, 20), "Position");
        GUI.Label(new Rect(30, Screen.height / 2 - 160, 20, 20), "  X");
        PositionText.x = GUI.TextField(new Rect(50, Screen.height / 2 - 160, 50, 20), PositionText.x);
        GUI.Label(new Rect(100, Screen.height / 2 - 160, 20, 20), "  Y");
        PositionText.y = GUI.TextField(new Rect(120, Screen.height / 2 - 160, 50, 20), PositionText.y);
        GUI.Label(new Rect(170, Screen.height / 2 - 160, 20, 20), "  Z");
        PositionText.z = GUI.TextField(new Rect(190, Screen.height / 2 - 160, 50, 20), PositionText.z);

        GUI.Label(new Rect(30, Screen.height / 2 - 140, 50, 20), "Rotation");
        GUI.Label(new Rect(30, Screen.height / 2 - 120, 20, 20), "  X");
        RotationText.x = GUI.TextField(new Rect(50, Screen.height / 2 - 120, 50, 20), RotationText.x);
        GUI.Label(new Rect(100, Screen.height / 2 - 120, 20, 20), "  Y");
        RotationText.y = GUI.TextField(new Rect(120, Screen.height / 2 - 120, 50, 20), RotationText.y);
        GUI.Label(new Rect(170, Screen.height / 2 - 120, 20, 20), "  Z");
        RotationText.z = GUI.TextField(new Rect(190, Screen.height / 2 - 120, 50, 20), RotationText.z);

        GUI.Label(new Rect(30, Screen.height / 2 - 100, 50, 20), "Scale");
        GUI.Label(new Rect(30, Screen.height / 2 - 80, 20, 20), "  X");
        ScaleText.x = GUI.TextField(new Rect(50, Screen.height / 2 - 80, 50, 20), ScaleText.x);
        GUI.Label(new Rect(100, Screen.height / 2 - 80, 20, 20), "  Y");
        ScaleText.y = GUI.TextField(new Rect(120, Screen.height / 2 - 80, 50, 20), ScaleText.y);
        GUI.Label(new Rect(170, Screen.height / 2 - 80, 20, 20), "  Z");
        ScaleText.z = GUI.TextField(new Rect(190, Screen.height / 2 - 80, 50, 20), ScaleText.z);
    }

    public void UpdateTransformValues()
    {
        try
        {
            PositionText = new VectorString(Piece.transform.position);
            RotationText = new VectorString(Piece.transform.eulerAngles);
            ScaleText = new VectorString(Piece.transform.localScale);
        }
        catch { }
    }

    private void ApplyTransformValues()
    {
        try
        {
            Piece.transform.position = PositionText.getVector3();
            Piece.transform.eulerAngles = RotationText.getVector3();
            Piece.transform.localScale = ScaleText.getVector3();
        }
        catch { }
    }
}
