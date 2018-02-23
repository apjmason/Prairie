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
	private const string AREAANNOTATION = "AreaAnnotationPanel";

	void Start() {
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	// Called in OnTriggerEnter in Annotation.cs
	public void ActivateGui() {
		active = true;
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	// Called in OnTriggerExit in Annotation.cs
	public void DeactivateGui() {
		active = false;
		gameObject.transform.Find(AREAANNOTATION).gameObject.SetActive (active);
	}

	public bool isUIActive() {
		return active;
	}

	// Add the newest entry in the in-range area annotations to the UI.
	public void AddAnnotationEntry(Annotation a) {
		List<Annotation> annotations = FPI.areaAnnotationsInRange;
		GameObject newStoryEntry;

		// Instantiate a new prefab if not enough entries have been created before.
		newStoryEntry = (GameObject)GameObject.Instantiate (prefab);
		newStoryEntry.transform.SetParent (contentPanel);

		// Scale the prefab according to resolution.
		fitPrefabToParentSize(newStoryEntry);

		// Update the button in the prefab indicating they keyDown for opening full annotations.
		Button b = newStoryEntry.GetComponentInChildren<Button> ();
		b.GetComponentInChildren<Text> ().text = (annotations.IndexOf(a) + 1).ToString ();

		// Update the content in the prefab. (<Text>[0]: button text, <Text>[1]: content text)
		newStoryEntry.GetComponentsInChildren<Text> ()[1].text = a.summary;

		a.areaAnnotationUIEntry = newStoryEntry;
	}

	// Remove the last entry when out of range.
	public void RemoveAnnotationEntry(Annotation removed) {
		// Destory the UI prefab associated with the removed annotation.
		Destroy(removed.areaAnnotationUIEntry);

		// Update all the keyDowns on removal.
		updateButtonIndicesOnRemove();
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

	private void updateButtonIndicesOnRemove() {
		for (int i = 0; i < FPI.areaAnnotationsInRange.Count; i++) {
			Button b = FPI.areaAnnotationsInRange[i].areaAnnotationUIEntry.GetComponentInChildren<Button> ();
			b.GetComponentInChildren<Text> ().text = (i+1).ToString ();
		}
	}
}
