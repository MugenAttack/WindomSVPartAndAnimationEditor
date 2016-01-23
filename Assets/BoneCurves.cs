using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct PosPoint
{
    public int Frame;
    public Vector3 Value;
    public int calctype;
    public int PowVal;
}

public struct RotPoint
{
    public int Frame;
    public Quaternion Value;
    public int calctype;
    public int PowVal;
}

public struct ScalePoint
{
    public int Frame;
    public Vector3 Value;
    public int calctype;
    public int PowVal;
}

public class BoneCurves {
    public string name;
    public List<PosPoint> PosCurve;
    public List<RotPoint> RotCurve;
    public List<ScalePoint> ScaleCurve;
    public GameObject GO;
    public BoneCurves()
    {
        PosCurve = new List<PosPoint>();
        RotCurve = new List<RotPoint>();
        ScaleCurve = new List<ScalePoint>();
    }
    
    public void CalculateFrame(int Frame)
    {
        int LowerFrame = -1;
        int HigherFrame = -1;
        //determine position
        if (PosCurve.Count > 1)
        {
            for (int i = 0; i < PosCurve.Count; i++)
            {
                if (PosCurve[i].Frame == Frame)
                {
                    GO.transform.localPosition = PosCurve[i].Value;
                    LowerFrame = -1;
                    HigherFrame = -1;
                    break;
                }
                else
                {
                    if (PosCurve[i].Frame < Frame && (LowerFrame == -1 || PosCurve[i].Frame > PosCurve[LowerFrame].Frame))
                        LowerFrame = i;

                    if (PosCurve[i].Frame > Frame && (HigherFrame == -1 || PosCurve[i].Frame < PosCurve[HigherFrame].Frame))
                        HigherFrame = i;
                }
            }

            if (LowerFrame != -1 && HigherFrame != -1)
            {
                float l = (Frame - PosCurve[LowerFrame].Frame) / (PosCurve[HigherFrame].Frame - PosCurve[LowerFrame].Frame);
                GO.transform.localPosition = Vector3.Lerp(PosCurve[LowerFrame].Value, PosCurve[HigherFrame].Value, l); 
            }

        }
        else if (PosCurve.Count != 0)
            GO.transform.localPosition = PosCurve[0].Value;

        LowerFrame = -1;
        HigherFrame = -1;
        //determine rotation
        if (RotCurve.Count > 1)
        {
            for (int i = 0; i < RotCurve.Count; i++)
            {
                if (RotCurve[i].Frame == Frame)
                {
                    GO.transform.localRotation = RotCurve[i].Value;
                    LowerFrame = -1;
                    HigherFrame = -1;
                    break;
                }
                else
                {
                    if (RotCurve[i].Frame < Frame && (LowerFrame == -1 || RotCurve[i].Frame > RotCurve[LowerFrame].Frame))
                        LowerFrame = i;

                    if (RotCurve[i].Frame > Frame && (HigherFrame == -1 || RotCurve[i].Frame < RotCurve[HigherFrame].Frame))
                        HigherFrame = i;
                }
            }

            if (LowerFrame != -1 && HigherFrame != -1)
            {
                float l = (Frame - RotCurve[LowerFrame].Frame) / (RotCurve[HigherFrame].Frame - RotCurve[LowerFrame].Frame);
                GO.transform.localRotation = Quaternion.Lerp(RotCurve[LowerFrame].Value, RotCurve[HigherFrame].Value, l);
            }

        }
        else if (RotCurve.Count != 0)
            GO.transform.localRotation = RotCurve[0].Value;

        LowerFrame = -1;
        HigherFrame = -1;
        //determine scale
        if (ScaleCurve.Count > 1)
        {
            for (int i = 0; i < ScaleCurve.Count; i++)
            {
                if (ScaleCurve[i].Frame == Frame)
                {
                    GO.transform.localScale = ScaleCurve[i].Value;
                    LowerFrame = -1;
                    HigherFrame = -1;
                    break;
                }
                else
                {
                    if (ScaleCurve[i].Frame < Frame && (LowerFrame == -1 || ScaleCurve[i].Frame > ScaleCurve[LowerFrame].Frame))
                        LowerFrame = i;

                    if (ScaleCurve[i].Frame > Frame && (HigherFrame == -1 || ScaleCurve[i].Frame < ScaleCurve[HigherFrame].Frame))
                        HigherFrame = i;
                }
            }

            if (LowerFrame != -1 && HigherFrame != -1)
            {
                float l = (Frame - ScaleCurve[LowerFrame].Frame) / (ScaleCurve[HigherFrame].Frame - ScaleCurve[LowerFrame].Frame);
                GO.transform.localScale = Vector3.Lerp(ScaleCurve[LowerFrame].Value, ScaleCurve[HigherFrame].Value, l);
            }

        }
        else if (ScaleCurve.Count != 0)
            GO.transform.localScale = ScaleCurve[0].Value;

    }
}
