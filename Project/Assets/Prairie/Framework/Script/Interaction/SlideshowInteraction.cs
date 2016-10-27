using UnityEngine;
using System.Collections;

public class SlideshowInteraction : Interaction
{
	private bool Active = false;

	public Texture2D[] Slides;
    public int CurrentSlide;

    private Rect Shading;

    void Start()
    {
		// Screen-sized panel used for shading when slideshow is displayed.
        Shading = new Rect(0, 0, Screen.width, Screen.height);
    }

    protected override void PerformAction()
    {
		Active = true;

		// Start at beginning of slideshow upon interaction
		CurrentSlide = 0;

        // Disable player movement/interactor upon interaction
		this.SetPlayerIsFrozen (true);
    }

	/// <summary>
	/// Displays GUI in which an interactive slideshow is displayed against a darkened play screen.
	/// </summary>
    void OnGUI()
    {
        if (Active)
        {
			// Darken the background (hacky method)
            GUI.Box(Shading, Texture2D.blackTexture);
            GUI.Box(Shading, Texture2D.blackTexture);
            GUI.Box(Shading, Texture2D.blackTexture);

			// Padding to ensure that slide is centered.
			GUILayout.BeginArea (Shading);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			// Slide (image) display.  If image is too large (i.e. width or height islarger than the play screen),
			// halve the image dimensions before displaying.  Else display the image as is.
			if (Slides.Length > 0) 
			{
				Texture slide = Slides [CurrentSlide];
				if (slide.width >= Screen.width || slide.height >= Screen.height) {
					float newWidth = (float)slide.width / 2;
					float newHeight = (float)slide.height / 2;
					GUILayout.Label (new GUIContent (slide), GUILayout.Width (newWidth), GUILayout.Height (newHeight));		
				} else 
				{
					GUILayout.Label (new GUIContent (slide));
				}
			}

			// Padding to ensure that slide is centered.
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea ();
        }
    }
		
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            // Advance w/ right arrow or D key; loop back to first image when on last image
            if (CurrentSlide < Slides.Length - 1)
            {
                CurrentSlide++;
            } else
            {
                CurrentSlide = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            // Retreat w/ left arrow or A key; loop back to last image when on first image
            if (CurrentSlide > 0)
            {
                CurrentSlide--;
            } else
            {
                CurrentSlide = Slides.Length - 1;
            }
        }
        else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Escape))
        {
            // Upon ESC, exit from GUI and reenable player control
            Active = false;
			this.SetPlayerIsFrozen (false);
        }
    }
}
