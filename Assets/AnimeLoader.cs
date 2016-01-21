using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
public static class AnimeLoader{
    
    public static List<BoneCurves> Load(string file)
    {
        Debug.Log("loading");
        List<BoneCurves> BClist = new List<BoneCurves>();
        XmlDocument doc = new XmlDocument();
        doc.Load(file);
        string curvetype = "";
        XmlNode BoneData = doc.SelectSingleNode("AnimeName/BoneData");

        foreach (XmlNode Bone in BoneData.ChildNodes)
        {
            int keyvalue = 0;
            BoneCurves BC = new BoneCurves();
            BC.name = Bone.Attributes["Text"].Value;
            //Debug.Log(Bone.Name);
     
            XmlNodeList childs = Bone.ChildNodes;
            for(int i = 0; i < childs.Count; i++)
            {
                switch (childs[i].Name)
                {
                    case "RotateKey":
                        curvetype = "RotateKey";
                        break;
                    case "ScaleKey":
                        curvetype = "Scalekey";
                        break;
                    case "PosKey":
                        curvetype = "PosKey";
                        break;
                    case "Time":
                        switch (curvetype)
                        {
                            case "RotateKey":
                                    RotPoint RP = new RotPoint();
                                    RP.Frame = int.Parse(childs[i].Attributes["Value"].Value);
                                    XmlNode Rota = childs[i].ChildNodes[0];
                                    RP.Value = new Quaternion(float.Parse(Rota.Attributes["x"].Value),
                                        float.Parse(Rota.Attributes["y"].Value),
                                        float.Parse(Rota.Attributes["z"].Value),
                                        float.Parse(Rota.Attributes["w"].Value));
                                    RP.calctype = int.Parse(childs[i].ChildNodes[1].Attributes["Value"].Value);
                                    if (RP.calctype == 1)
                                        RP.PowVal = int.Parse(childs[i].ChildNodes[2].Attributes["Value"].Value);
                                    BC.RotCurve.Add(RP);
                                
                                break;
                            case "ScaleKey":
                                    ScalePoint SP = new ScalePoint();
                                    SP.Frame = int.Parse(childs[i].Attributes["Value"].Value);
                                    XmlNode Scale = childs[i].ChildNodes[0];
                                    SP.Value = new Vector3(float.Parse(Scale.Attributes["x"].Value),
                                        float.Parse(Scale.Attributes["y"].Value),
                                        float.Parse(Scale.Attributes["z"].Value));
                                    SP.calctype = int.Parse(childs[i].ChildNodes[1].Attributes["Value"].Value);
                                    if (SP.calctype == 1)
                                        SP.PowVal = int.Parse(childs[i].ChildNodes[2].Attributes["Value"].Value);

                                    BC.ScaleCurve.Add(SP);
                                
                                break;
                            case "PosKey":
                                    PosPoint PP = new PosPoint();
                                    PP.Frame = int.Parse(childs[i].Attributes["Value"].Value);
                                    XmlNode Pos = childs[i].ChildNodes[0];
                                    PP.Value = new Vector3(float.Parse(Pos.Attributes["x"].Value),
                                        float.Parse(Pos.Attributes["y"].Value),
                                        float.Parse(Pos.Attributes["z"].Value));
                                    PP.calctype = int.Parse(childs[i].ChildNodes[1].Attributes["Value"].Value);
                                    if (PP.calctype == 1)
                                        PP.PowVal = int.Parse(childs[i].ChildNodes[2].Attributes["Value"].Value);

                                    BC.PosCurve.Add(PP);
                                
                                break;
                        }

                        break;
                }
            }
            BClist.Add(BC);
        }
        return BClist;
    }

	public static void Save(List<BoneCurves> Bones, string file)
    {
        
        







    }
}
