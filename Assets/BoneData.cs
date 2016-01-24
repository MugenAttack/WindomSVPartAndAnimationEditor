using UnityEngine;
using System.Collections;

public class BoneData : MonoBehaviour
{

    public int EulerMode;
    public int BoneLayers;
    public int[] BoneFlag;
    public float[] LimitAng;
    public string Windom_FileName;
    public int Windom_Hide;
    public Matrix4x4 SMatrix;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setDefault()
    {
        EulerMode = 0;
        BoneLayers = 1;
        BoneFlag = new int[] { 10, 1 };
        LimitAng = new float[] { -180, 180, -180, 180, -180, 180 };
        Windom_Hide = 0;
    }
}
