#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using Swing.Editor;

public class LS_VRPromoCreator : EditorWindow {
    [MenuItem("Tools/VR Trailer Maker")]
    
    // Use this for initialization
    static void Init()
    {
        GUIContent gcw = new GUIContent("VR Trailer Maker");
        LS_VRPromoCreator window = (LS_VRPromoCreator)EditorWindow.GetWindow(typeof(LS_VRPromoCreator));
        window.titleContent = gcw;
        window.maxSize = new Vector2(420, 320);
        window.minSize = window.maxSize;
        window.Show();
    }

    enum MusicSelection {None, MusicIncludedInZip, FunMusic, FunMusic2, FunMusic3, Heavenly, Heavenly2, Heavenly3, Heavenly4, Heavenly5, Horror, Horror2, Horror3, Horror4, Mystic, Mystic2, Mystic3, Mystic4, Sorrow, Sorrow2, Sorrow3, Sorrow4, TechMusic, TechMusic2, TechMusic3, TechMusic4, TechMusic5, TechMusic6, TechMusic7, TechMusic8, TechMusic9}

    MusicSelection musicTrack = MusicSelection.None;

    enum VRPlatform { VRGeneral, GearVR, Rift, Vive, PlaystationVR, All }

    VRPlatform vrPlatform = VRPlatform.VRGeneral;

    public string emailField;
    public string movieFileLocation = "None";

    Texture logoIcon = (Texture)Resources.Load("LS_VRPromo_Logo");

    //Production API
    string apiUrl = "http://52.88.181.168/API/vtm.php";


    public void OnGUI()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Box("Promo", GUILayout.Width(414), GUILayout.Height(160));
        EditorGUI.DrawPreviewTexture(new Rect(10, 10, 400, 60), logoIcon);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUI.LabelField(new Rect(10, 74, 400, 90),
                "Instructions: Zip your 360 video(s), 360 image(s) and/or 360 image \nsequence(s) into one file. You can add your own music to track to \nthe zip file if you would like it used. Complete & select Upload Now. \n \n   -Upload Limit: 1000 mb (1 GB) \n   -Processing Time: Up to 48 Hours");


        GUILayout.BeginArea(new Rect(10, 170, 400, 20));
        emailField = EditorGUI.TextField(new Rect(0, 0, 400, 20), "Email (Download Link): ", emailField);
        GUILayout.EndArea();

        EditorGUILayout.Separator();

        GUILayout.BeginArea(new Rect(10, 194, 400, 20));
        musicTrack = (MusicSelection)EditorGUILayout.EnumPopup("Music Track: ", musicTrack);
        GUILayout.EndArea();

        EditorGUILayout.Separator();

        GUILayout.BeginArea(new Rect(10, 214, 400, 20));
        vrPlatform = (VRPlatform)EditorGUILayout.EnumPopup("End-Room Platform: ", vrPlatform);
        GUILayout.EndArea();

        EditorGUILayout.Separator();


        GUILayout.BeginArea(new Rect(10, 234, 120, 40));
        if (GUILayout.Button("Select Content Zip"))
        {
            movieFileLocation = EditorUtility.OpenFilePanel("Select your 360 content zip file (Video, Image Sequence, Screenshots)", "", "zip");
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(330, 234, 80, 40));
        if (GUILayout.Button("Clear"))
        {
            movieFileLocation = "";
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 258, 400, 40));
        string s2 = movieFileLocation;

        if (movieFileLocation.Length > 48)
        {
            s2 = "..." + movieFileLocation.Substring(movieFileLocation.Length - 48);
        }
        EditorGUI.LabelField(new Rect(0, 0, 80, 20),
                "File Selected:  ");
        EditorGUI.LabelField(new Rect(86, 0, 314, 20),
                s2);
        GUILayout.EndArea();

        EditorGUILayout.Separator();

        GUILayout.BeginArea(new Rect(155, 286, 120, 20));

        //Upload to API
        if (GUILayout.Button("Upload Now"))
        {
            if(string.Equals(movieFileLocation,"None") == true || string.IsNullOrEmpty(movieFileLocation))
            {
                Debug.LogError("Please Select a Zip File");
                return;
            }
            if (string.IsNullOrEmpty(emailField))
            {
                Debug.LogError("Please enter a email");
                return;
            }
            EditorCoroutine.start(PrepareFile());
        }

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(155, 370, 120, 20));

        ////Goto Website
        //if (GUILayout.Button("More Info"))
        //{
        //    Application.OpenURL("http://www.lucidsight.com/");
        //}

        GUILayout.EndArea();
    }

    IEnumerator PrepareFile()
    {

        Debug.Log("Loading File From Loacation: " + movieFileLocation);

        //Read the zip file
        WWW loadedFile = new WWW("file:///" + movieFileLocation);

        yield return loadedFile;

        Debug.Log("File Size: " + loadedFile.bytes.Length);

        StepTwo(loadedFile);

    }

    void StepTwo(WWW post)
    {
       EditorCoroutine.start(UpoadTheZip(post));
    }


    IEnumerator UpoadTheZip(WWW post)
    {

        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;

        //Debug.Log("File To Be Sent Size: " + post.bytes.Length);

        form.AddBinaryData("contentFile", post.bytes, "contentData.zip", "application/zip");
        form.AddField("Email", emailField);
        form.AddField("MusicTrack", musicTrack.ToString());
        form.AddField("Platform", vrPlatform.ToString());

        form.headers["Content-Type"] = "multipart/form-data";

        //Send POST request
        string url = apiUrl;

        WWW postFile = new WWW(url, form);

        Debug.Log("Uploading Files...");

        yield return postFile;

        while (!postFile.isDone) {
            EditorUtility.DisplayProgressBar("Sending Content for VPM", "Uploading ", Mathf.InverseLerp(0, 1, postFile.uploadProgress));
            yield return "";
        }

        EditorUtility.ClearProgressBar();

        Debug.Log("Upload Complete!");

        Debug.Log("" + postFile.text);
    }


}

#endif
