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

	// Add the newest entry in the in-range area annotations to the UI.
	public void AddAnnotationEntry(List<Annotation> annotations) {
		Annotation a = annotations[annotations.Count-1];
		GameObject newStoryEntry;

		if (contentPanel.transform.childCount < annotations.Count) {
			// Instantiate a new prefab if not enough entries have been created before.
			newStoryEntry = (GameObject)GameObject.Instantiate (prefab);
			newStoryEntry.transform.SetParent (contentPanel);

			// Scale the prefab according to resolution.
			fitPrefabToParentSize(newStoryEntry);
		} else {
			// Modify and setActive a previously recycled, unused prefab entry.
			newStoryEntry = contentPanel.transform.GetChild (annotations.Count - 1).gameObject;
			newStoryEntry.SetActive (true);
		}

		// Update the button index to match the keyDown operation in Annotation.
		Button b = newStoryEntry.GetComponentInChildren<Button> ();
		b.GetComponentInChildren<Text> ().text = annotations.Count.ToString();

		// Update the content in the prefab.
		newStoryEntry.GetComponentsInChildren<Text> ()[1].text = a.summary;
	}

	// Remove the last entry when out of range.
	public void RemoveAnnotationEntry(List<Annotation> annotations) {
		int indexToRemove = contentPanel.transform.childCount - 1;
		GameObject entryToRemove = contentPanel.transform.GetChild (indexToRemove).gameObject;

		// Recycle the prefab for later use instead of destroying it.
		entryToRemove.SetActive (false);
	}


	private void fitPrefabToParentSize(GameObject storyEntry) {
		double actualWidth = gameObject.transform.Find (AREAANNOTATION).GetComponent<RectTransform> ().rect.width; // This is the actual width we want to prefab to be after scaling.
		float originalWidth = storyEntry.GetComponent<LayoutElement>().preferredWidth; // This is the preset preferred width of the prefab.

		// Constrain the prefab width to its parent panel.
		RectTransform rt = storyEntry.transform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2((float)actualWidth, rt.rect.height);

		// Scale the content within the prefab.
		float scale = (float)actualWidth / (float)originalWidth;
		storyEntry.transform.localScale = new Vector3 (scale, scale, scale);
	}
}
