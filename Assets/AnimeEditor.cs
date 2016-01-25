using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AnimeEditor : MonoBehaviour
{
    public bool Hide = true;
    public float FrameSlider;
    public string sFrame = "";
    public string sFrameC = "";
    public string speed = "";
    VectorString PositionText = new VectorString(0, 0, 0);
    int selectedPiece;
    GameObject Piece;
    RoboBuild RB;
    string AnimeID = "";
    int guilock;
    float prevValue;
    int selection;
    int prevmode = 0;
    int editmode = 0;
    int editModeChange = 0;
    int selectedFrame;
    int prevSelectedFrame;
    int frameNum;
    int indexBC = 0;
    
    string[] slist = { "Parts", "Frames" };
    string[] elist = { "Position", "Rotation", "Scale" };
    Vector2[] ScrollPositions = { Vector2.zero, Vector2.zero, Vector2.zero };
    // Use this for initialization
    void Start()
    {
        RB = GetComponent<RoboBuild>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {

        UpdateTransformValues();
        if (!Hide)
        {
            if (guilock == 0)
                GUI_AnimeID();
            else
            {
                GUI_FrameSlider();
                GUI_BoneMenu();

                GUI.Box(new Rect(Screen.width - 130, 50, 120, 40), "");
                if (GUI.Button(new Rect(Screen.width - 115, 60, 90, 20),"Save")){ };
            }
        }

    }

    void GUI_AnimeID()
    {
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "Enter Animation ID");
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
        GUI.Label(new Rect(30, Screen.height / 2 - 180, 50, 20), elist[editmode]);
        GUI.Label(new Rect(30, Screen.height / 2 - 160, 20, 20), "  X");
        PositionText.x = GUI.TextField(new Rect(50, Screen.height / 2 - 160, 50, 20), PositionText.x);
        GUI.Label(new Rect(100, Screen.height / 2 - 160, 20, 20), "  Y");
        PositionText.y = GUI.TextField(new Rect(120, Screen.height / 2 - 160, 50, 20), PositionText.y);
        GUI.Label(new Rect(170, Screen.height / 2 - 160, 20, 20), "  Z");
        PositionText.z = GUI.TextField(new Rect(190, Screen.height / 2 - 160, 50, 20), PositionText.z);
        editmode = GUI.SelectionGrid(new Rect(30, Screen.height / 2 - 130, 235, 20), editmode, elist, 3);

        if (editmode != editModeChange)
            UpdateTransformValues();
        editModeChange = editmode;

        ApplyTransformValues();
        try
        {
            if (selection == 1)
            {
                GUI.Box(new Rect(30, Screen.height / 2 - 100, 235, 235), "Frames");
                GUI_Framelist();
                
                if (GUI.Button(new Rect(30, Screen.height / 2 + 140, 116, 20), "Add")) { }
                if (GUI.Button(new Rect(150, Screen.height / 2 + 140, 115, 20), "Remove"))
                {
                    switch (elist[editmode])
                    {
                        case "Position":
                            RB.BC[indexBC].PosCurve.RemoveAt(selectedFrame);
                            break;
                        case "Rotation":
                            RB.BC[indexBC].RotCurve.RemoveAt(selectedFrame);
                            break;
                        case "Scale":
                            RB.BC[indexBC].ScaleCurve.RemoveAt(selectedFrame);
                            break;
                    }
                }
                
                if (selectedFrame != prevSelectedFrame)
                    RB.AnimeFrameGo(frameNum);
                prevSelectedFrame = selectedFrame;
                UpdateTransformValues();
                if (checkIfEmpty())
                    indexBC = 0;
            }
            else
            {
                GUI.Box(new Rect(30, Screen.height / 2 - 100, 235, 260), "Parts");
                string[] Piecelist = listModels();
                ScrollPositions[1] = GUI.BeginScrollView(new Rect(30, Screen.height / 2 - 75, 235, 235), ScrollPositions[1], new Rect(0, 0, 219, Piecelist.Length * 25));
                try
                {
                    selectedPiece = GUI.SelectionGrid(new Rect(0, 0, 219, (float)25 * (float)Piecelist.Length), selectedPiece, Piecelist, 1);
                }
                catch { }
                GUI.EndScrollView();
            }
        }
        catch { }
        selection = GUI.SelectionGrid(new Rect(30, Screen.height / 2 + 165, 235, 20), selection, slist, 2);

    }


    public void UpdateTransformValues()
    {
        try
        {
            switch (elist[editmode])
            {
                case "Position":
                    PositionText = new VectorString(RB.BC[indexBC].PosCurve[selectedFrame].Value);
                    break;
                case "Rotation":
                    PositionText = new VectorString(RB.BC[indexBC].RotCurve[selectedFrame].Value.eulerAngles);
                    break;
                case "Scale":
                    PositionText = new VectorString(RB.BC[indexBC].ScaleCurve[selectedFrame].Value);
                    break;
            }
        }
        catch { }
    }

    private void ApplyTransformValues()
    {
        try
        {
            switch (elist[editmode])
            {
                case "Position":
                    PosPoint ptemp = RB.BC[indexBC].PosCurve[selectedFrame];
                    ptemp.Value = PositionText.getVector3();
                    RB.BC[indexBC].PosCurve[selectedFrame] = ptemp;
                    break;
                case "Rotation":
                    RotPoint rtemp = RB.BC[indexBC].RotCurve[selectedFrame];
                    rtemp.Value = Quaternion.Euler(PositionText.getVector3());
                    RB.BC[indexBC].RotCurve[selectedFrame] = rtemp;
                    break;
                case "Scale":
                    ScalePoint stemp = RB.BC[indexBC].ScaleCurve[selectedFrame];
                    stemp.Value = PositionText.getVector3();
                    RB.BC[indexBC].ScaleCurve[selectedFrame] = stemp;
                    break;
            }
        }
        catch { }
    }

    private string[] listModels()
    {
        List<string> modelslist = new List<string>();

        foreach (GameObject index in RB.parts)
        {
            try
            {
                modelslist.Add(index.name);
            }
            catch { modelslist.Add("Unknown"); }
        }
        return modelslist.ToArray();
    }

    private void GUI_Framelist()
    {
        bool hasFrames = false;
        List<string> framelist = new List<string>();
        List<int> frameNumlist = new List<int>();

        for (int i = 0; i < RB.BC.Count; i++)
        {
            if (RB.BC[i].GO == RB.parts[selectedPiece])
            { indexBC = i; hasFrames = true; break; }

        }
        if (hasFrames)
        {
            //construct list
            switch (elist[editmode])
            {
                case "Position":
                    foreach (PosPoint point in RB.BC[indexBC].PosCurve)
                    {
                        framelist.Add("Frame " + point.Frame.ToString());
                        frameNumlist.Add(point.Frame);
                    }
                    break;
                case "Rotation":
                    foreach (RotPoint point in RB.BC[indexBC].RotCurve)
                    {
                        framelist.Add("Frame " + point.Frame.ToString());
                        frameNumlist.Add(point.Frame);
                    }
                    break;
                case "Scale":
                    foreach (ScalePoint point in RB.BC[indexBC].ScaleCurve)
                    {
                        framelist.Add("Frame " + point.Frame.ToString());
                        frameNumlist.Add(point.Frame);
                    }
                    break;
            }

            if (framelist.Count != 0)
            {
                if (framelist.Count < selectedFrame)
                    selectedFrame = 0;

                ScrollPositions[0] = GUI.BeginScrollView(new Rect(30, Screen.height / 2 - 75, 235, 210), ScrollPositions[0], new Rect(0, 0, 219, framelist.Count * 25));
                selectedFrame = GUI.SelectionGrid(new Rect(0, 0, 219, (float)25 * (float)framelist.Count), selectedFrame, framelist.ToArray(), 1);
                frameNum = frameNumlist[selectedFrame];
                GUI.EndScrollView();
            }
            else
                GUI.Label(new Rect(30, Screen.height / 2 - 75, 235, 210), "No frame data is currently in the curve, press add frame to add keyframes to the curve");

        }
        else
            GUI.Label(new Rect(30, Screen.height / 2 - 75, 235, 210), "this part doesn't have any animation data, press add frame to create animation data and add one frame to selected curve");
    }

    private bool checkIfEmpty()
    {
        if (RB.BC[indexBC].PosCurve.Count == 0 && RB.BC[indexBC].RotCurve.Count == 0 && RB.BC[indexBC].ScaleCurve.Count == 0)
        { RB.BC.RemoveAt(indexBC); return true; }
        return false;
    }
}
