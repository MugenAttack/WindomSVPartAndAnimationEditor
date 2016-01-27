using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct scriptKey
{
    public int Frame;
    public string script;
}

public class ScriptKeys {
    public List<scriptKey> SK;
    public string AnimeName;
    public int ID;
    public string TopScript;
	// Use this for initialization
	public  ScriptKeys()
    {
        SK = new List<scriptKey>();
    }

}
