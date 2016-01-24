using UnityEngine;
using System.Collections;

public class VectorString
{
    public string x;
    public string y;
    public string z;

    public VectorString(string Px, string Py, string Pz) { x = Px; y = Py; z = Pz; }
    public VectorString(float Px, float Py, float Pz) { x = Px.ToString(); y = Py.ToString(); z = Pz.ToString(); }
    public VectorString(Vector3 Pass) { x = Pass.x.ToString(); y = Pass.y.ToString(); z = Pass.z.ToString(); }
    public Vector3 getVector3()
    {
        Vector3 Pass;
        Pass.x = float.Parse(x);
        Pass.y = float.Parse(y);
        Pass.z = float.Parse(z);
        //Debug.Log (Pass);
        return Pass;
    }
}
