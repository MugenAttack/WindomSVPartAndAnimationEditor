using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class RoboBuild : MonoBehaviour {
    public List<GameObject> parts;
    public string BPpath = "";
    public string Modelpath = "";
    public bool parent = true;
    XmlWriter xw;
    //public List<GameObject> OrderedParts;
    int CurrentID = -1;
    
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	   
	}
 
    public void setBPpath(string path)
    {
        BPpath = path;
    }

    public void setModelpath(string path)
    {
        Modelpath = path;
    }

    public void LoadRobo()
    {
        BpBoneData[] data = BoneProperty.Read(BPpath + "\\BoneProperty.xml");
        Matrix4x4[] pMatrix = new Matrix4x4[data.Length];
        GameObject part;
        parts = new List<GameObject>();
        ObjImporter obj = new ObjImporter();
        //create objects
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].ParentBoneIdx != -1)
            {
                pMatrix[i] = pMatrix[data[i].ParentBoneIdx] * data[i].TransMat.transpose;
            }
            else
            {
                pMatrix[i] = data[i].TransMat.transpose;
            }

            part = new GameObject(data[i].Name);
            Debug.Log(data[i].Name);
            parts.Add(part);
           try
            {
                if (File.Exists(Modelpath + "\\" + data[i].Windom_FileName.Substring(0, data[i].Windom_FileName.Length - 2) + ".obj"))
                {
                   // Debug.Log("Exists");
                    
                    Material m = new Material(Shader.Find("Standard"));
                    part.AddComponent<MeshRenderer>().material = m;
                    part.AddComponent<MeshFilter>().mesh = obj.ImportFile(Modelpath + "\\" + data[i].Windom_FileName.Substring(0, data[i].Windom_FileName.Length - 2) + ".obj");
                }
            }
           catch { };

            part.transform.position = Utils.GetPosition(pMatrix[i]);
            part.transform.rotation = Utils.GetRotation(pMatrix[i]);
            part.transform.localScale = Utils.GetScale(pMatrix[i]);

            //add bone data component
            BoneData BD = part.AddComponent<BoneData>();
            BD.EulerMode = data[i].EulerMode;
            BD.BoneLayers = data[i].BoneLayers;
            BD.BoneFlag = data[i].BoneFlag;
            BD.LimitAng = data[i].LimitAng;
            BD.Windom_FileName = data[i].Windom_FileName;
            BD.Windom_Hide = data[i].Windom_Hide;


            if (data[i].ParentBoneIdx != -1)
                part.transform.SetParent(parts[data[i].ParentBoneIdx].transform, true);

        }
    }


    public void SaveRobo()
    {
        CurrentID = -1;
        GameObject Base = parts[0];
        XmlWriterSettings xws = new XmlWriterSettings();
        xws.Indent = true;
        xw = XmlWriter.Create(BPpath + "\\BoneProperty.xml", xws);
        xw.WriteStartDocument();
        xw.WriteStartElement("BoneProperty");
        xw.WriteAttributeString("Count",parts.Count.ToString());

        //write bone data
        BoneData BD = Base.GetComponent<BoneData>();
        xw.WriteStartElement(Base.name);
        xw.WriteStartElement("Level");
        xw.WriteAttributeString("Value", 0.ToString());
        xw.WriteEndElement();
        xw.WriteStartElement("ParentBoneIdx");
        xw.WriteAttributeString("Value", (-1).ToString());
        xw.WriteEndElement();
        xw.WriteStartElement("TransMat");
        BD.SMatrix = Matrix4x4.TRS(Base.transform.position,Base.transform.rotation,Base.transform.localScale);
        xw.WriteString(MatrixToString(BD.SMatrix.transpose));
        xw.WriteEndElement();
        xw.WriteStartElement("OffsetMat");
        xw.WriteString(MatrixToString(BD.SMatrix.transpose.inverse));
        xw.WriteEndElement();
        xw.WriteStartElement("EulerMode");
        xw.WriteAttributeString("Value", BD.EulerMode.ToString());
        xw.WriteEndElement();
        xw.WriteStartElement("BoneLayers");
        xw.WriteAttributeString("Value", BD.BoneLayers.ToString());
        xw.WriteEndElement();
        xw.WriteStartElement("BoneFlag");
        xw.WriteAttributeString("Value", BD.BoneFlag[0].ToString());
        xw.WriteAttributeString("Value2", BD.BoneFlag[1].ToString());
        xw.WriteEndElement();
        xw.WriteStartElement("LimitAng"); 
        xw.WriteString(LimitAngToString(BD.LimitAng));
        xw.WriteEndElement();
        xw.WriteStartElement("Windom_FileName");
        xw.WriteAttributeString("Text", BD.Windom_FileName);
        xw.WriteEndElement();
        xw.WriteStartElement("Windom_Hide");
        xw.WriteAttributeString("Value", BD.Windom_Hide.ToString());
        xw.WriteEndElement();
        xw.WriteEndElement();
        CurrentID++;
        setChildren(Base.transform, 1, CurrentID);
        xw.WriteEndDocument();
        xw.Close();
    }

    public void setChildren(Transform Tparent,int level,int parent)
    {
        GameObject Base;
        BoneData BD;
        for(int i = 0; i < Tparent.childCount; i++)
        {
            
            Base = Tparent.GetChild(i).gameObject;
            BD = Base.GetComponent<BoneData>();
            xw.WriteStartElement(Base.name);
            xw.WriteStartElement("Level");
            xw.WriteAttributeString("Value", level.ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("ParentBoneIdx");
            xw.WriteAttributeString("Value", parent.ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("TransMat");
            BD.SMatrix = Matrix4x4.TRS(Base.transform.position, Base.transform.rotation, Base.transform.localScale);
            xw.WriteString(MatrixToString((Tparent.gameObject.GetComponent<BoneData>().SMatrix.inverse * BD.SMatrix).transpose));
            xw.WriteEndElement();
            xw.WriteStartElement("OffsetMat");
            xw.WriteString(MatrixToString(BD.SMatrix.transpose.inverse));
            xw.WriteEndElement();
            xw.WriteStartElement("EulerMode");
            xw.WriteAttributeString("Value", BD.EulerMode.ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("BoneLayers");
            xw.WriteAttributeString("Value", BD.BoneLayers.ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("BoneFlag");
            xw.WriteAttributeString("Value", BD.BoneFlag[0].ToString());
            xw.WriteAttributeString("Value2", BD.BoneFlag[1].ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("LimitAng");
            xw.WriteString(LimitAngToString(BD.LimitAng));
            xw.WriteEndElement();
            xw.WriteStartElement("Windom_FileName");
            xw.WriteAttributeString("Text", BD.Windom_FileName);
            xw.WriteEndElement();
            xw.WriteStartElement("Windom_Hide");
            xw.WriteAttributeString("Value", BD.Windom_Hide.ToString());
            xw.WriteEndElement();
            xw.WriteEndElement();
            CurrentID++;
            if (Base.transform.childCount != 0)
                setChildren(Base.transform, level + 1, CurrentID);
        }
    }

    public string MatrixToString(Matrix4x4 pMatrix)
    {
        string Merge = "";

        Merge += pMatrix.m00.ToString() + " ";
        Merge += pMatrix.m01.ToString() + " ";
        Merge += pMatrix.m02.ToString() + " ";
        Merge += pMatrix.m03.ToString() + " ";

        Merge += pMatrix.m10.ToString() + " ";
        Merge += pMatrix.m11.ToString() + " ";
        Merge += pMatrix.m12.ToString() + " ";
        Merge += pMatrix.m13.ToString() + " ";

        Merge += pMatrix.m20.ToString() + " ";
        Merge += pMatrix.m21.ToString() + " ";
        Merge += pMatrix.m22.ToString() + " ";
        Merge += pMatrix.m23.ToString() + " ";

        Merge += pMatrix.m30.ToString() + " ";
        Merge += pMatrix.m31.ToString() + " ";
        Merge += pMatrix.m32.ToString() + " ";
        Merge += pMatrix.m33.ToString() + " ";

        return Merge;
    }

    public string LimitAngToString(float[] LA)
    {
        string Merge = "";
        for (int i = 0; i < 6; i++)
        {
            Merge += LA[i].ToString() + " ";
        }
        return Merge;
    }
}
