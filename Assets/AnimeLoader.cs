using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public static class AnimeLoader
{

    public static void Load(ref ScriptKeys sKeys, ref List<BoneCurves> rBC, string file)
    {
        Debug.Log("loading");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);
        XmlNode AnimeName = doc.SelectSingleNode("AnimeName");
        sKeys.AnimeName = AnimeName.Attributes["Name"].Value;
        sKeys.ID = int.Parse(AnimeName.Attributes["ID"].Value);
        foreach (XmlNode node in AnimeName.ChildNodes)
        {
            switch (node.Name)
            {
                case "Windom_TopScript":
                    sKeys.TopScript = node.InnerText;
                    break;
                case "Time":
                    scriptKey key;
                    key.Frame = int.Parse(node.Attributes["Value"].Value);
                    key.script = node.FirstChild.InnerText;
                    sKeys.SK.Add(key);
                    break;
                case "BoneData":
                    string curvetype = "";
                    XmlNode BoneData = node;

                    foreach (XmlNode Bone in BoneData.ChildNodes)
                    {
                        BoneCurves BC = new BoneCurves { name = Bone.Attributes["Text"].Value };
                        //Debug.Log(Bone.Name);

                        XmlNodeList childs = Bone.ChildNodes;
                        for (int i = 0; i < childs.Count; i++)
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
                        rBC.Add(BC);
                    }
                    break;
            }
        }
    }

    public static void Save(List<BoneCurves> Bones,ScriptKeys sKeys, string file, List<GameObject> GOs)
    {
        XmlWriterSettings xws = new XmlWriterSettings() { Indent = true };
        using (FileStream FS = new FileStream(file, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite))
        {
            using (XmlWriter xw = XmlWriter.Create(FS, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("AnimeName");
                xw.WriteAttributeString("Name", sKeys.AnimeName);
                xw.WriteAttributeString("ID", sKeys.ID.ToString());

                xw.WriteStartElement("Windom_TopScript");
                xw.WriteString(sKeys.TopScript);
                xw.WriteEndElement();

                xw.WriteStartElement("ScriptKey");
                xw.WriteAttributeString("Count", sKeys.SK.Count.ToString());
                xw.WriteEndElement();

                foreach (scriptKey key in sKeys.SK)
                {
                    xw.WriteStartElement("Time");
                    xw.WriteAttributeString("Value", key.Frame.ToString());
                    xw.WriteStartElement("ScriptText");
                    xw.WriteString(key.script);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                xw.WriteStartElement("BoneData");
                foreach (GameObject GO in GOs)
                {
                    BoneCurves BC = new BoneCurves();
                    bool hasCurves = false;
                    foreach (BoneCurves loopBC in Bones)
                    {
                        if (GO == loopBC.GO)
                        { BC = loopBC; hasCurves = true; break; }
                    }

                    if (hasCurves)
                    {
                        xw.WriteStartElement("BoneName");
                        xw.WriteAttributeString("Text", BC.name);

                        xw.WriteStartElement("RotateKey");
                        xw.WriteAttributeString("Count", BC.RotCurve.Count.ToString());
                        xw.WriteEndElement();

                        foreach (RotPoint RP in BC.RotCurve)
                        {
                            xw.WriteStartElement("Time");
                            xw.WriteAttributeString("Value", RP.Frame.ToString());

                            xw.WriteStartElement("Rota");
                            xw.WriteAttributeString("x", RP.Value.x.ToString());
                            xw.WriteAttributeString("y", RP.Value.y.ToString());
                            xw.WriteAttributeString("z", RP.Value.z.ToString());
                            xw.WriteAttributeString("w", RP.Value.w.ToString());
                            xw.WriteEndElement();

                            xw.WriteStartElement("CalcType");
                            xw.WriteAttributeString("Value", RP.calctype.ToString());
                            xw.WriteEndElement();

                            if (RP.calctype == 1)
                            {
                                xw.WriteStartElement("PowVal");
                                xw.WriteAttributeString("Value", RP.PowVal.ToString());
                                xw.WriteEndElement();
                            }

                            xw.WriteEndElement();
                        }

                        xw.WriteStartElement("ScaleKey");
                        xw.WriteAttributeString("Count", BC.ScaleCurve.Count.ToString());
                        xw.WriteEndElement();

                        foreach (ScalePoint SP in BC.ScaleCurve)
                        {
                            xw.WriteStartElement("Time");
                            xw.WriteAttributeString("Value", SP.Frame.ToString());

                            xw.WriteStartElement("Scale");
                            xw.WriteAttributeString("x", SP.Value.x.ToString());
                            xw.WriteAttributeString("y", SP.Value.y.ToString());
                            xw.WriteAttributeString("z", SP.Value.z.ToString());
                            xw.WriteEndElement();

                            xw.WriteStartElement("CalcType");
                            xw.WriteAttributeString("Value", SP.calctype.ToString());
                            xw.WriteEndElement();

                            if (SP.calctype == 1)
                            {
                                xw.WriteStartElement("PowVal");
                                xw.WriteAttributeString("Value", SP.PowVal.ToString());
                                xw.WriteEndElement();
                            }

                            xw.WriteEndElement();
                        }

                        xw.WriteStartElement("PosKey");
                        xw.WriteAttributeString("Count", BC.PosCurve.Count.ToString());
                        xw.WriteEndElement();

                        foreach (PosPoint PP in BC.PosCurve)
                        {
                            xw.WriteStartElement("Time");
                            xw.WriteAttributeString("Value", PP.Frame.ToString());

                            xw.WriteStartElement("Pos");
                            xw.WriteAttributeString("x", PP.Value.x.ToString());
                            xw.WriteAttributeString("y", PP.Value.y.ToString());
                            xw.WriteAttributeString("z", PP.Value.z.ToString());
                            xw.WriteEndElement();

                            xw.WriteStartElement("CalcType");
                            xw.WriteAttributeString("Value", PP.calctype.ToString());
                            xw.WriteEndElement();

                            if (PP.calctype == 1)
                            {
                                xw.WriteStartElement("PowVal");
                                xw.WriteAttributeString("Value", PP.PowVal.ToString());
                                xw.WriteEndElement();
                            }

                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();

                    }

                }
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
        }
    }
}
