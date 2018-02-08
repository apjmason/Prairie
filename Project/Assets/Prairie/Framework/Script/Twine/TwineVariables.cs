using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TwineVariables
{

    private static TwineVariables instance;
    private Dictionary<string, string> diction;

    private TwineVariables()
    {

        this.diction = new Dictionary<string, string>();
        this.diction["color"] = "white";

        //		foreach (string r in vars) {
        //			string ar = r.Replace ("((", string.Empty).Replace ("))", string.Empty);
        //			List<string> pair = ar.Split (new string[] { ":" }, StringSplitOptions.None).ToList();
        //			this.diction [pair[0]] = pair[1];
        //		}
    }

    public Dictionary<string, string> GetVariables()
    {
        return diction;
    }

    public void AssignValue(string var, string val)
    {
        // TODO: check if there is a var, if not, automatically create one
        this.diction[var] = val;
    }

    public string GetValue(string var)
    {
        if (diction.ContainsKey(var))
        {
            return diction[var];
        } else
        {
            return "asdfkajsdfljasf";
        }
    }


    public static TwineVariables GetVariableObject()
    {
        if (instance == null)
        {
            instance = new TwineVariables();
        }
        return instance;
    }

}