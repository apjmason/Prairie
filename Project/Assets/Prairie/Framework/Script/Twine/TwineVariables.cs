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
        this.diction[var] = val;
    }

    //public void AssignValueArithmetic(string var, int val)
    //{
    //    if (this.diction[var] == null)
    //    {
    //        this.diction[var] = val;
    //    }
    //    this.diction[var] += val;
    //}

    public string GetValue(string var)
    {
        if (diction.ContainsKey(var))
        {
            return diction[var];
        }
        else
        {
            // TODO: Discuss the best way to handle this situation
            return null;
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
