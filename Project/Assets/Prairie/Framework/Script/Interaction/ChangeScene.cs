using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : PromptInteraction {
	

	public string SceneName;

	protected override void PerformAction() {
		SceneManager.LoadScene(SceneName);
	}
}
