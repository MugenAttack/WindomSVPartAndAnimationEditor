using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Assets;

public class PartEditor : MonoBehaviour
{
    RoboBuild RB;
    AnimeEditor AE;
    public int ID;
    string Name = "";
    string NewPiece = "";
    string FName = "";
    int LockGUI = 4;
    int selectedPiece;
    int PrevselectedPiece;
    int selectedAttach = 0;
    VectorString PositionText = new VectorString(0, 0, 0);
    VectorString RotationText = new VectorString(0, 0, 0);
    VectorString ScaleText = new VectorString(0, 0, 0);
    Vector2[] ScrollPositions = { Vector2.zero, Vector2.zero, Vector2.zero };
    Rect windowRect = new Rect(40, 80, 380, 260);
    int ToolSelect;
    string[] ToolsName = { "Main", "Transform", "Pieces", "Attach" };
    public GameObject Piece;
    string TextureUrl = "";
    public string folder;
    string apName = "";
    string apFile = "";
    GameObject[] Childlist;
    bool isChildList = false;
    bool Hide = false;
    //rewrite GUI simplify things into 2 menus or less
    void Start()
    {
        RB = GetComponent<RoboBuild>();
        AE = GetComponent<AnimeEditor>();
        RB.LoadPaths();

    }
    void Update()
    {

    }

    void OnGUI()
    {
        if (!Hide)
        {
            windowRect = GUI.Window(ID, windowRect, WindowFunction, "Part Editor");
            CheckPieceChange();
        }
    }

    void WindowFunction(int windowID)
    {

        //part name and file save/load
        if (LockGUI == 1) { GUI_Transform(10, 30); }
        else if (LockGUI == 2) { GUI_AddPart(10, 50); }
        else if (LockGUI == 3) { GUI_Attach(10, 30); }
        else if (LockGUI == 4) { GUI_LoadData(10, 30); }
        else {
            //GUI.Label(new Rect (50,25,100,20),"Part Name");
            //Name = GUI.TextField(new Rect(120,25,100,20),Name);

            GUI_PieceSelect(10, 20);
        }

        GUI.DragWindow();

    }

    #region GUI Controls	
    private void GUI_PieceSelect(float x, float y)
    {

        //pieces handling aka models
        GUI.BeginGroup(new Rect(x, y, 360, 230));
        GUI.Box(new Rect(0, 0, 360, 230), "Piece Selection and Control");
        GUI.Box(new Rect(10, 22, 250, 51), "");
        GUI.Label(new Rect(20, 25, 90, 20), "Name:");
        GUI.Label(new Rect(90, 25, 170, 20), RB.parts[selectedPiece].name);
        GUI.Label(new Rect(20, 50, 90, 20), "File Name:");
        GUI.Label(new Rect(90, 50, 170, 20), RB.parts[selectedPiece].GetComponent<BoneData>().Windom_FileName);
        if (GUI.Button(new Rect(262, 25, 88, 20), "Add Part")) { LockGUI = 2; }
        if (GUI.Button(new Rect(262, 50, 88, 20), "Transform")) { LockGUI = 1; UpdateTransformValues(); }
        //if (GUI.Button(new Rect(262,69,88,20), "Texture")){LockGUI = 2;}
        if (GUI.Button(new Rect(262, 75, 88, 20), "Parent")) { LockGUI = 3; }
        if (GUI.Button(new Rect(262, 100, 88, 20), "Remove")) { RemoveModel(); }
        if (GUI.Button(new Rect(262, 125, 88, 20), "Save")) { RB.SaveRobo(); }

        GUI.Box(new Rect(10, 77, 250, 140), "");
        string[] Piecelist = listModels();
        ScrollPositions[1] = GUI.BeginScrollView(new Rect(10, 77, 250, 140), ScrollPositions[1], new Rect(0, 0, 234, Piecelist.Length * 25));
        try
        {

            Debug.Log(Piecelist.Length);
            selectedPiece = GUI.SelectionGrid(new Rect(0, 0, 234, (float)25 * (float)Piecelist.Length), selectedPiece, Piecelist, 1);
        }
        catch { }
        GUI.EndScrollView();
        GUI.EndGroup();



    }

    private void GUI_LoadData(float x, float y)
    {
        GUI.BeginGroup(new Rect(x, y, 370, 200));
        GUI.Box(new Rect(0, 0, 360, 200), "Load Data");

        GUI.Label(new Rect(20, 30, 110, 20), "Xed output Path");
        RB.BPpath = GUI.TextField(new Rect(20, 50, 320, 20), RB.BPpath);

        GUI.Label(new Rect(20, 80, 110, 20), "Model Folder Path");
        RB.Modelpath = GUI.TextField(new Rect(20, 100, 320, 20), RB.Modelpath);

        if (GUI.Button(new Rect(10, 145, 165, 20), "Load to Skeleton Editor")) { RB.SavePaths(); RB.LoadRobo(); LockGUI = 0; UpdateTransformValues(); }
        if (GUI.Button(new Rect(185, 145, 165, 20), "Load to Animation Editor")) { RB.SavePaths(); RB.LoadRobo(); Hide = true; AE.Hide = false; }
        GUI.EndGroup();
    }

    private void GUI_AddPart(float x, float y)
    {
        GUI.BeginGroup(new Rect(x, y, 370, 200));
        GUI.Box(new Rect(0, 0, 360, 200), "Load Data");

        GUI.Label(new Rect(20, 30, 110, 20), "Name of Part");
        apName = GUI.TextField(new Rect(20, 50, 320, 20), apName);

        GUI.Label(new Rect(20, 80, 110, 20), "Model Name");
        apFile = GUI.TextField(new Rect(20, 100, 320, 20), apFile);

        if (GUI.Button(new Rect(10, 170, 165, 20), "Load Part"))
        {
            GameObject newPart = new GameObject(apName);
            RB.parts.Add(newPart);
            try
            {
                if (File.Exists(Path.Combine(RB.Modelpath, apFile)))
                {
                    var scen = RB.Importer.ImportFile(Path.Combine(RB.Modelpath, apFile), Helper.PostProcessStepflags);
                    Mesh mesh = new Mesh();
                    mesh.CombineMeshes(scen.Meshes.Select(c => new CombineInstance() { mesh = c.ToUnityMesh(), transform = scen.RootNode.Transform.ToUnityMatrix() }).ToArray(), false);

                    Material[] materials = new Material[scen.Meshes.Length];

                    for (int index = 0; index < materials.Length; index++)
                    {
                        var mat = new Material(Shader.Find("Standard"));

                        if (scen.Materials[scen.Meshes[index].MaterialIndex] != null)
                        {
                            mat.name = scen.Materials[scen.Meshes[index].MaterialIndex].Name;
                            var textures = scen.Materials[scen.Meshes[index].MaterialIndex].GetAllTextures();

                            if (textures.Length > 0)
                            {
                                mat.mainTexture = Helper.LoadTexture(Path.Combine(RB.Modelpath, textures[0].FilePath));
                            }
                        }

                        materials[index] = mat;
                    }

                    newPart.AddComponent<MeshFilter>().mesh = mesh;
                    newPart.AddComponent<MeshRenderer>().materials = materials;
                }
            }
            catch { }

            newPart.AddComponent<BoneData>().setDefault();
            newPart.GetComponent<BoneData>().Windom_FileName = apFile;
            newPart.transform.SetParent(RB.parts[0].transform, true);

            LockGUI = 0;
        }
        if (GUI.Button(new Rect(185, 170, 165, 20), "Cancel")) { LockGUI = 0; }

        GUI.EndGroup();
    }
    private void GUI_Transform(float x, float y)
    {

        //transform data
        GUI.BeginGroup(new Rect(x, y, 370, 200));
        GUI.Box(new Rect(0, 0, 360, 200), "Transform");
        //position/offset
        GUI.Label(new Rect(10, 20, 110, 20), "Position/Offset");
        GUI.Label(new Rect(15, 40, 10, 20), "X");
        PositionText.x = GUI.TextField(new Rect(27, 40, 96, 20), PositionText.x);
        GUI.Label(new Rect(125, 40, 10, 20), "Y");
        PositionText.y = GUI.TextField(new Rect(137, 40, 96, 20), PositionText.y);
        GUI.Label(new Rect(235, 40, 10, 20), "Z");
        PositionText.z = GUI.TextField(new Rect(247, 40, 96, 20), PositionText.z);
        //Rotation
        GUI.Label(new Rect(10, 60, 110, 20), "Rotation");
        GUI.Label(new Rect(15, 80, 10, 20), "X");
        RotationText.x = GUI.TextField(new Rect(27, 80, 96, 20), RotationText.x);
        GUI.Label(new Rect(125, 80, 10, 20), "Y");
        RotationText.y = GUI.TextField(new Rect(137, 80, 96, 20), RotationText.y);
        GUI.Label(new Rect(235, 80, 10, 20), "Z");
        RotationText.z = GUI.TextField(new Rect(247, 80, 96, 20), RotationText.z);
        //Scale
        GUI.Label(new Rect(10, 100, 110, 20), "Scale");
        GUI.Label(new Rect(15, 120, 10, 20), "X");
        ScaleText.x = GUI.TextField(new Rect(27, 120, 96, 20), ScaleText.x);
        GUI.Label(new Rect(125, 120, 10, 20), "Y");
        ScaleText.y = GUI.TextField(new Rect(137, 120, 96, 20), ScaleText.y);
        GUI.Label(new Rect(235, 120, 10, 20), "Z");
        ScaleText.z = GUI.TextField(new Rect(247, 120, 96, 20), ScaleText.z);

        if (GUI.Button(new Rect(135, 160, 88, 20), "Exit")) { LockGUI = 0; }
        GUI.EndGroup();

        ApplyTransformValues();

    }

    private void GUI_ApplyTexture(float x, float y)
    {
        GUI.BeginGroup(new Rect(x, y, 360, 100));
        GUI.Box(new Rect(0, 0, 360, 100), "Apply Texture");
        GUI.Label(new Rect(50, 30, 100, 20), "File Name");
        TextureUrl = GUI.TextField(new Rect(140, 30, 180, 20), TextureUrl);
        if (GUI.Button(new Rect(120, 60, 50, 20), "Apply")) { LockGUI = 0; }
        if (GUI.Button(new Rect(190, 60, 50, 20), "Cancel")) { LockGUI = 0; }
        GUI.EndGroup();
    }

    private void GUI_Attach(float x, float y)
    {

        //Attach
        GUI.BeginGroup(new Rect(x, y, 360, 200));
        GUI.Box(new Rect(0, 0, 360, 200), "Attach to Object");
        GUI.Box(new Rect(10, 20, 340, 140), "");
        string[] Piecelist = listModels();
        try
        {
            ScrollPositions[2] = GUI.BeginScrollView(new Rect(15, 28, 330, 125), ScrollPositions[2], new Rect(0, 0, 314, Piecelist.Length * 25));
            selectedAttach = GUI.SelectionGrid(new Rect(0, 0, 319, (float)25 * (float)Piecelist.Length), selectedAttach, Piecelist, 1);
            GUI.EndScrollView();
        }
        catch { Debug.Log("Fail to Load List"); }
        if (GUI.Button(new Rect(10, 170, 165, 20), "Set Parent")) { setParent(); LockGUI = 0; }
        if (GUI.Button(new Rect(185, 170, 165, 20), "Cancel")) { LockGUI = 0; }
        GUI.EndGroup();

    }
    #endregion



    private void CheckPieceChange()
    {
        //remove possibly
        try
        {
            if (PrevselectedPiece != selectedPiece)
            {
                Piece = RB.parts[selectedPiece];
                UpdateTransformValues();
            }
            else { ApplyTransformValues(); }
            PrevselectedPiece = selectedPiece;
        }
        catch { }
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

    private void RemoveModel()
    {
        if (RB.parts[selectedPiece].transform.childCount == 0)
        {
            GameObject.Destroy(RB.parts[selectedPiece]);
            RB.parts.RemoveAt(selectedPiece);
            selectedPiece = 0;
        }


    }

    private void CreateObject()
    {


    }

    private void setParent()
    {
        RB.parts[selectedPiece].transform.SetParent(RB.parts[selectedAttach].transform, true);
    }


}
