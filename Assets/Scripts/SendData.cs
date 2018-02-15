using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SendData : MonoBehaviour {

	public InputField textField;
	public RawImage image;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TestGet() {
		StartCoroutine(Get(url));
	}

	public string url = "http://localhost:3000/ping";

	// just test that www is working
	IEnumerator Get(string url) {

		using (WWW www = new WWW(url))
		{
			yield return www;

			Debug.Log("Got "+url);
			Debug.Log(www.text);
		}


	}

	// called from Upload button
	public void TestPost() {
		WWWForm form = new WWWForm();

		form.AddField("send_email",textField.text);

		string url = "http://localhost:3000/upload";

		Texture2D tex = (Texture2D) image.texture;

		byte[] bytes = tex.EncodeToJPG();
		form.AddBinaryData("file", bytes, "testupload.jpg", "image/jpg");

		StartCoroutine(Post(url,form));

	}

	IEnumerator Post(string url, WWWForm form) {
		
		using (WWW www = new WWW(url,form))
		{
			yield return www;

			Debug.Log("Posted "+url);
			Debug.Log(www.text);
		}
	}

	// called by Load Image button
	public void PopulateImage() {

		LoadMediaFile("/Users/dave/Work/Gigs/superbright/dev/TestHttpUpload/Assets/StreamingAssets/demo1.jpg");
	}


	public void LoadMediaFile (string filePath)
	{
		//Debug.Log ("Loading media file " + filePath);
		FileInfo f = new FileInfo (filePath);

		if (f.Extension == ".jpg") {
			string fullpath = "file://" + filePath;
			Debug.Log("Loading file: "+fullpath);
			WWW www = new WWW (fullpath);
			StartCoroutine (LoadImage (www));
		}


	}
		
	private IEnumerator LoadImage (WWW www)
	{
		yield return www;
		Debug.Log("Got "+www.text);
		if (www.error == null) {
			image.texture = www.texture;
		} else
			Debug.LogError (www.error);
	}



}
