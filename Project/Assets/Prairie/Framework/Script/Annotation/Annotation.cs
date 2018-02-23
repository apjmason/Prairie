using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;


public class AnnotationContent
{
    public List<string> parsedText;
    public List<string> imagePaths;

    public AnnotationContent()
    {
        parsedText = new List<string>();
        imagePaths = new List<string>();
    }

}

public enum AnnotationTypes : int {SUMMARY = 0, AREA = 1 };
public enum ImportTypes : int {NONE = 0, IMPORT = 1, INSPECTOR = 2 };

[AddComponentMenu("Prairie/Annotation/Annotation")]
public class Annotation : Interaction
{
    public AnnotationContent content;

    public string textFilePath = ""; //saved for use in the editor
    public string imagePath = "";

    public int importType = (int)ImportTypes.NONE; //uses int for editor purposes
    public int annotationType = (int)AnnotationTypes.SUMMARY; //0 for summary, 1 for area.  Default to 0 for inspector

    public bool includeImages = false;
    public bool sharedFile = true; //whether images and text are in the same file
    
    public TextAsset textFile;
    public string text = "";
    public List<Texture2D> images;
    public string summary = "";

	// The UI element representing an entry in area annotation toolbox
	public GameObject areaAnnotationUIEntry;

	// Journal option
	public bool addToJournal = true;

	private FirstPersonInteractor player;

    void Start()
    {
        content = new AnnotationContent();

        images = new List<Texture2D>();

        //this changes parsedText and imgPaths
        ParseAnnotation.ParseAnnotationText(text, content);
        
        if (includeImages)
        {
            foreach (string path in content.imagePaths)
            {
                //grabbing byte data from file so we can convert it into a texture
                try {
                    byte[] fileData = File.ReadAllBytes(imagePath + path);
                    Texture2D img = new Texture2D(2, 2);
                    img.LoadImage(fileData);
                    images.Add(img);
                } catch (Exception e)
                {
                    Debug.Log(e);
                    images.Add(null);
                }
            }
        } 
    }

    protected override void PerformAction()
    {
        if (importType != (int)ImportTypes.NONE)
        {
            FirstPersonInteractor player = this.GetPlayer();
            if (player != null)
            {
                player.SetCanMove(false);
                player.SetDrawsGUI(false);

				// Add summary annotation log to journal
				if (addToJournal && annotationType == (int)AnnotationTypes.SUMMARY) {
					player.GetComponentInChildren<Journal> ().AddToJournal (this);
				}

				// Display full annotation
				FullAnnotationGui annotationGui = player.GetComponentInChildren<FullAnnotationGui> ();

				if (!annotationGui.isUIActive ()) {
					annotationGui.ActivateGui (this);
				}
            }
        }
    }

    void Update()
    {
		FirstPersonInteractor player = this.GetPlayer ();
		if (player != null) {
			FullAnnotationGui annotationGui = player.GetComponentInChildren<FullAnnotationGui> ();
			if (annotationGui.isUIActive()) {

				if (Input.GetKey (KeyCode.Q) || Input.GetKey (KeyCode.Escape)) {
					annotationGui.DeactivateGui ();

					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;

					player.SetCanMove (true);
					player.SetDrawsGUI (true);
				} else {
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;

					player.SetCanMove (false);
					player.SetDrawsGUI (false);
				}
			}
		}
    }

	void OnTriggerEnter(Collider other)
	{
        if (this.annotationType != (int)AnnotationTypes.AREA)
        {
            // do not act as area annotation if not specified as one
            return;
        }

		// ensure we're being triggered by a player
		FirstPersonInteractor interactor = other.gameObject.GetComponent<FirstPersonInteractor> ();
		if (interactor == null)
		{
			return;
		}
		else
		{
			interactor.areaAnnotationsInRange.Add(this);

			// Draw area annotation entries in the lower left corner
			AreaAnnotationGui aag = interactor.GetComponentInChildren<AreaAnnotationGui> ();
			if (!aag.isUIActive ()) {
				aag.ActivateGui ();
			}
			aag.AddAnnotationEntry (this);

			// Add area annotation log to journal
			if (addToJournal) {
				interactor.GetComponentInChildren<Journal> ().AddToJournal (this);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
        if (this.annotationType != (int)AnnotationTypes.AREA)
        {
            // do not act as area annotation if not specified as one
            return;
        }

		// ensure we're being triggered by a player
		FirstPersonInteractor interactor = other.gameObject.GetComponent<FirstPersonInteractor> ();
		if (interactor == null)
		{
			return;
		}
		else
		{
			interactor.areaAnnotationsInRange.Remove(this);

			// Remove annotation entry from the toolbox on the lower left corner.
			interactor.GetComponentInChildren<AreaAnnotationGui> ().RemoveAnnotationEntry (this);

			if (interactor.areaAnnotationsInRange.Count == 0) {
				interactor.GetComponentInChildren<AreaAnnotationGui> ().DeactivateGui ();
			}
		}
	}
}
