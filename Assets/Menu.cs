using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    PartEditor PE;
    AnimeEditor AE;
    RoboBuild RB;
    bool Hide = false;
    Rect windowRect = new Rect(40, 80, 380, 260);
    bool toggleHide;
    bool toggleHideCheck;
    // Use this for initialization
    void Start () {
        PE = GetComponent<PartEditor>();
        AE = GetComponent<AnimeEditor>();
        RB = GetComponent<RoboBuild>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        if (!Hide)
            windowRect = GUI.Window(0, windowRect, WindowFunction, "Load Menu");

        if (PE.Hide == false || AE.Hide == false)
            GUI_ShowType();
        
    }
    void WindowFunction(int windowID)
    {
        GUI_LoadData(10,30);
        GUI.DragWindow();

    }
    private void GUI_LoadData(float x, float y)
    {
        GUI.BeginGroup(new Rect(x, y, 370, 200));
        GUI.Box(new Rect(0, 0, 360, 200), "Load Data");

        GUI.Label(new Rect(20, 30, 110, 20), "Xed output Path");
        RB.BPpath = GUI.TextField(new Rect(20, 50, 320, 20), RB.BPpath);

        GUI.Label(new Rect(20, 80, 110, 20), "Model Folder Path");
        RB.Modelpath = GUI.TextField(new Rect(20, 100, 320, 20), RB.Modelpath);

        if (GUI.Button(new Rect(10, 145, 165, 20), "Load to Skeleton Editor")) { RB.SavePaths(); RB.LoadRobo(); PE.Hide = false; PE.UpdateTransformValues(); Hide = true; }
        if (GUI.Button(new Rect(185, 145, 165, 20), "Load to Animation Editor")) { RB.SavePaths(); RB.LoadRobo(); AE.Hide = false; Hide = true; }
        GUI.EndGroup();
    }

    private void GUI_ShowType()
    {
        //hide models that are set to hide in boneproperty 
        GUI.Box(new Rect(Screen.width - 130, 10, 120, 40), "Use Windom Hide");
        toggleHide = GUI.Toggle(new Rect(Screen.width - 100, 29, 110, 40), toggleHide, "On/Off");
        if (toggleHide != toggleHideCheck)
            ToggleWindom_Hide(toggleHide);
        toggleHideCheck = toggleHide;

    }

    private void ToggleWindom_Hide(bool toggle)
    {
        foreach (GameObject part in RB.parts)
        {
            if (part.GetComponent<MeshRenderer>() != null)
            {
                if ((part.GetComponent<BoneData>().Windom_Hide == 1) && toggle)
                    part.GetComponent<MeshRenderer>().enabled = false;
                else
                    part.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
