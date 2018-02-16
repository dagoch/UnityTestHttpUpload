#define BESTHTTP_DISABLE_COOKIES
#define BESTHTTP_DISABLE_WEBSOCKET
#define BESTHTTP_DISABLE_SIGNALR
#define BESTHTTP_DISABLE_SOCKETIO
#define BESTHTTP_DISABLE_ALTERNATE_SSL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BestHTTP;
using System;



public class SendData : MonoBehaviour {

	public InputField textField;
	public RawImage image1;
	public RawImage image2;
	public Text sendButtonText;
	public Text sendVideoButtonText;

	// Use this for initialization
	void Start () {
		filenameByRequest = new Dictionary<HTTPRequest, string>();
		
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

	public Dictionary<HTTPRequest, string> filenameByRequest;

	void OnRequestFinished(HTTPRequest request, HTTPResponse response)
	{
//		Debug.Log(		request.Uri);
//		Debug.Log(request.GetEntityBody().ToString());
//		Debug.Log(request.ToString());
		Debug.Log("Request finished for filename: "+filenameByRequest[request]);

		if (response != null) {
			Debug.Log("Request Finished! Text received: " + response.DataAsText);

			sendButtonText.text = "Sent!";
		} else {
			Debug.Log("Request Finished but no response received.");
		}


		/* how to tell what happened to request
 switch (req.State)
 	 {
 		 // The request finished without any problem.
 		 case HTTPRequestStates.Finished:
 			 Debug.Log("Request Finished Successfully!\n" + resp.DataAsText);
 			 break;

 		 // The request finished with an unexpected error.
// The request's Exception property may contain more information about the error.
 		 case HTTPRequestStates.Error:
 			 Debug.LogError("Request Finished with Error! " +
(req.Exception != null ?
(req.Exception.Message + "\n" + req.Exception.StackTrace) :
"No Exception"));
 			 break;

 		 // The request aborted, initiated by the user.
 		 case HTTPRequestStates.Aborted:
 			 Debug.LogWarning("Request Aborted!");
 			 break;

 		 // Ceonnecting to the server timed out.
 		 case HTTPRequestStates.ConnectionTimedOut:
 			 Debug.LogError("Connection Timed Out!");
 			 break;

 		 // The request didn't finished in the given time.
 		 case HTTPRequestStates.TimedOut:
 			 Debug.LogError("Processing the request Timed Out!");
 			 break;
 	 }
 	 */
	}

	string filename1;
	string filename2;


	// called from Upload button
	public void TestPost() {
		string url = "http://localhost:3000/upload";

		for (int i = 1; i < 3; i++) {
			string filename;
			Texture2D tex;
			if (i == 1) {
				tex = (Texture2D) image1.texture;
				filename = filename1;
			} else {
				tex = (Texture2D) image2.texture;
				filename = filename2;
			}

			// use bestHTTP
			var request = new HTTPRequest(new Uri(url),
				HTTPMethods.Post,
				OnRequestFinished);


			request.AddBinaryData("image", tex.EncodeToPNG(), filename );
			request.AddField("send_email",textField.text);
			request.AddField("filename", filename);
			request.OnUploadProgress = OnUploadProgress;
			request.Send();
			sendButtonText.text = "Sending...";

			filenameByRequest[request] = filename;
		}
/*
		WWWForm form = new WWWForm();

		form.AddField("send_email",textField.text);

		string url = "http://localhost:3000/upload";

		Texture2D tex = (Texture2D) image.texture;

		byte[] bytes = tex.EncodeToJPG();
		form.AddBinaryData("file", bytes, "testupload.jpg", "image/jpg");

		StartCoroutine(Post(url,form));
		*/

	}


	void OnUploadProgress(HTTPRequest request, long uploaded, long length) {
		float progressPercent = (uploaded / (float)length) * 100.0f;
		Debug.Log("Uploaded: " + progressPercent.ToString("F2") + "%");
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

			filename1 = "v1_thumb.jpg";
		filename2 = "demo1.jpg";
		LoadMediaFile("/Users/dave/Work/Gigs/superbright/dev/TestHttpUpload/Assets/StreamingAssets/"+filename1, image1);
		LoadMediaFile("/Users/dave/Work/Gigs/superbright/dev/TestHttpUpload/Assets/StreamingAssets/"+filename2, image2);
	}


	public void LoadMediaFile (string filePath, RawImage image)
	{
		//Debug.Log ("Loading media file " + filePath);
		FileInfo f = new FileInfo (filePath);

		if (f.Extension == ".jpg") {
			string fullpath = "file://" + filePath;
			Debug.Log("Loading file: "+fullpath);
			WWW www = new WWW (fullpath);
			StartCoroutine (LoadImage (www, image));
		}


	}
		
	private IEnumerator LoadImage (WWW www, RawImage image)
	{
		yield return www;
		Debug.Log("Got "+www.text);
		if (www.error == null) {
			image.texture = www.texture;
		} else
			Debug.LogError (www.error);
	}


	//  --------  THIS IS FOR SENDING A VIDEO ATTACHMENT --------

	public string server = "http://localhost:3000";
	public string uploadPath = "/upload";

	byte[] videoBytes;

	private IEnumerator LoadVideo (WWW www, string filename, string email)
	{
		yield return www;
		Debug.Log("Got "+www.text);

		if (www.error == null) {
			videoBytes = www.bytes;

			string url = server+uploadPath;
			var request = new HTTPRequest(new Uri(url),
				HTTPMethods.Post,
				OnVideoRequestFinished);


			request.AddBinaryData("image", videoBytes, filename );
			request.AddField("send_email",email);
//			request.AddField("filename", filename);
			request.OnUploadProgress = OnUploadProgress;
			request.Send();
			sendVideoButtonText.text = "Sending...";

			filenameByRequest[request] = filename;

		} else
			Debug.LogError (www.error);
	}

	public void SendVideo() {
		string pathpath = "/Users/dave/Work/Gigs/superbright/dev/TestHttpUpload/Assets/StreamingAssets/";
		 string filename = "v1.mp4";
		string filepath = "file://"+pathpath + filename;


			string email = textField.text;
		



		// does this work?
		WWW www = new WWW (filepath);

		StartCoroutine (LoadVideo (www,filename,email));




	}


	// callback when http request completed
	void OnVideoRequestFinished(HTTPRequest request, HTTPResponse response)
	{

		Debug.Log ("Request finished for filename: " + filenameByRequest [request]);



		switch (request.State) {

		case HTTPRequestStates.Finished:  		// The request finished without any problem.
			Debug.Log ("Request Finished Successfully!\n" + response.DataAsText);
			sendVideoButtonText.text = "Video Sent!";
			break;

		case HTTPRequestStates.Error:   // The request finished with an unexpected error.
			// The request's Exception property may contain more information about the error.
			Debug.LogError ("Request Finished with Error! " +
				(request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
			sendVideoButtonText.text = "Send Video";
			break;

		default:
			Debug.LogError ("Request failed.");
			sendVideoButtonText.text = "Send Video";
			break;

		}

		// remove request entry in dictionary
		filenameByRequest.Remove(request);

	}

}
