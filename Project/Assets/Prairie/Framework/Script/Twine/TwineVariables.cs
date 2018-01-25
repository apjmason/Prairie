using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TwineVariables {

	private static TwineVariables instance;
	private Dictionary<string, string> diction;

	private TwineVariables(string[] vars) {

		this.diction = new Dictionary<string, string>();
		this.diction ["color"] = "red";

//		foreach (string r in vars) {
//			string ar = r.Replace ("((", string.Empty).Replace ("))", string.Empty);
//			List<string> pair = ar.Split (new string[] { ":" }, StringSplitOptions.None).ToList();
//			this.diction [pair[0]] = pair[1];
//		}
	}

	public Dictionary<string, string> Variables(){
		return diction;
	}


	public static TwineVariables GetTwineVariables(string[] vars) {
		if (instance == null)
		{
			instance = new TwineVariables(vars);
		}
		return instance;
	}

}