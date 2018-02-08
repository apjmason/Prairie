using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AreaAnnotationGui : MonoBehaviour
{
	private bool active = false;

	public Transform contentPanel;
	public GameObject prefab;
	public FirstPersonInteractor FPI;

	// Name of the areaAnnotationGUI object in game scene.
	private const string AREAANNOTATION = "AnnotationPanel";
	private const string SUMMARY = "Summary";

	void Start() {
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	public void ActivateGui() {
		active = true;
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	public void DeactivateGui() {
		active = false;
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	public bool isUIActive() {
		return active;
	}

	public void AddAnnotationEntry(List<Annotation> annotations)
	{
		Annotation a = annotations[annotations.Count-1];
		GameObject newStoryEntry;
		if (contentPanel.transform.childCount < annotations.Count) {
			newStoryEntry = (GameObject)GameObject.Instantiate (prefab);
			newStoryEntry.transform.SetParent (contentPanel);
		} else {
			newStoryEntry = contentPanel.transform.GetChild (annotations.Count - 1).gameObject;
			newStoryEntry.SetActive (true);
		}

		Button b = newStoryEntry.GetComponentInChildren<Button> ();
		b.GetComponentInChildren<Text> ().text = annotations.Count.ToString();

		newStoryEntry.GetComponentsInChildren<Text> ()[1].text = a.summary;
	}

	public void RemoveAnnotationEntry(List<Annotation> annotations)
	{
		int indexToRemove = contentPanel.transform.childCount - 1;
		GameObject entryToRemove = contentPanel.transform.GetChild (indexToRemove).gameObject;
		entryToRemove.SetActive (false);
	}
}
