using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Slideshow))]
public class SlideshowEditor : Editor {

	Slideshow slideshow;
    Texture2D[] editorSlides = new Texture2D[0];

	public override void OnInspectorGUI()
	{
		slideshow = (Slideshow)target;

        if (editorSlides.Length == 0)
        {
            editorSlides = slideshow.Slides;
        }

		editorSlides = PrairieGUI.drawObjectList ("Slides", editorSlides);

		for (int i = 0; i < slideshow.Slides.Length; i++) 
		{
			if (slideshow.Slides [i] == null) 
			{
				DrawWarnings ();
				break;
			}
		}

        slideshow.Slides = PrairieGUI.RemoveNulls(editorSlides);

		if (GUI.changed) {
			EditorUtility.SetDirty(slideshow);
		}
	}

	public void DrawWarnings()
	{
		PrairieGUI.warningLabel ("One or more of the slides in your slideshow is empty.  Please fill the empty slides or remove them.");
	}
}
