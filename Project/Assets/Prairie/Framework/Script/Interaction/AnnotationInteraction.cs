using UnityEngine;
using System.Collections;

public class AnnotationInteraction : Interaction
{
    private bool active = false;
    private GameObject controller;
    public string text;

    private GUIContent content;
    private GUIStyle style;

    void Start()
    {
        content = new GUIContent(text);
        style = new GUIStyle();
        style.wordWrap = true;
        style.normal.textColor = Color.white;
        style.normal.background = Texture2D.blackTexture;
        style.normal.background.height = Screen.width - 20;
        style.normal.background.width = Screen.width / 2;
        style.richText = true;




    }

    protected override void PerformAction()
    {
        active = true;
        
        //Want to change this in case user changes the name of FPSController
        //find better way of disabling movement
        GameObject.Find("FPSController").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        GameObject.Find("FPSController").GetComponent<FirstPersonInteractor>().enabled = false;
    }

    void OnGUI()
    {
        if (active)
        {

            // Make a background box
            
            GUI.backgroundColor = Color.black;
            GUI.Box(new Rect(Screen.width / 4, 10, Screen.width / 2, Screen.height - 20), content, style);
            
            
        }
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            active = false;
            GameObject.Find("FPSController").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            GameObject.Find("FPSController").GetComponent<FirstPersonInteractor>().enabled = true;
        }

    }
}
