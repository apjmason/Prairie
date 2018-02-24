using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TwineVariables
{

    private static TwineVariables instance;
    private Dictionary<string, string> varDictionary;

    private TwineVariables()
    {
        this.varDictionary = new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetVariables()
    {
        return varDictionary;
    }

    /// <summary>
    /// Assign a value to a twine variable.  The value must be provided as a 
    /// string, and it can be parsed as an int later if needed.
    /// </summary>
    /// <param name="var">Variable name</param>
    /// <param name="val">New value</param>
    public void AssignValue(string var, string val)
    {
        this.varDictionary[var] = val;
    }
    
    /// <summary>
    /// Adds a value to an existing variable value.  If both values are 
    /// numeric, they are converted to ints, summed, and then stored as a 
    /// string; otherwise, the two strings are concatenated
    /// </summary>
    /// <param name="var"></param>
    /// <param name="val"></param>
    public void IncrementValue(string var, string val)
    {
        if (varDictionary.ContainsKey(var))
        {
            // Try parsing the two values as ints
            int val1;
            int val2;
            if (Int32.TryParse(varDictionary[var], out val1) &&
                Int32.TryParse(val, out val2))
            {
                int newVal = val1 + val2;
                varDictionary[var] = newVal.ToString();
            }
            // If that fails, concatenate them
            else
            {
                varDictionary[var] = varDictionary[var] + val;
            }
            
        }
        else
        {
            // If there is no existing variable for this value, create one and
            // set it equal to the provided value.
            varDictionary[var] = val;
        }
    }

    public string GetValue(string var)
    {
        if (varDictionary.ContainsKey(var))
        {
            return varDictionary[var];
        }
        else
        {
            // TODO: Discuss the best way to handle this situation
            return "";
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