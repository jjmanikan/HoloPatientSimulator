﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using JsonData;

public class DialogflowAPIScript : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		// AccessToken is being generated manually in terminal
		//StartCoroutine(GetAgent("ya29.c.ElpfBkjOUlTRSaNDg-i0tBjGc2WlRT9GePIqe1_j5Xq9flXHMGJWnn5sEjNHyG1VfMFqtt3WapHAVo2-RwvPNKRTHI0BkF9OVUzZJ5OWJEILr64_ge1tgcbS7AA"));

		//https://stackoverflow.com/questions/51272889/unable-to-send-post-request-to-dialogflow-404
		//first param is the dialogflow API call, second param is Json web token
		StartCoroutine(PostRequest("https://dialogflow.googleapis.com/v2/projects/aragenttest-hugf/agent/sessions/34563:detectIntent",
							  "ya29.c.Kp0B4geCSeEZWxKgxIINaQbNRk2qdS4cpCK2CfzJI3ueFgLii5e12goO4PRJXRKM1lBwBJ_hYcQe-Iu3vQjBZ9_m4QAK7tE7KenzjCnvAewiErd6yqFl8AJS5BN0yO0kDA6qXh9CUSBwB4oVsT3DibO_3sNWdyR6l6puTJe-UYI5KQ4So4Na7BqA3P-sVETv6fT_Zy10lNmM9A7jzE3xXw"));
	}

	// Update is called once per frame
	void Update()
	{

	}

	IEnumerator PostRequest(String url, String AccessToken)
	{
		UnityWebRequest postRequest = new UnityWebRequest(url, "POST");
		RequestBody requestBody = new RequestBody();
		//requestBody.queryInput = new QueryInput();
		//requestBody.queryInput.text = new TextInput();
		//requestBody.queryInput.text.text = "hello";
		//requestBody.queryInput.text.languageCode = "en";

		string jsonRequestBody = JsonUtility.ToJson(requestBody, true);
		Debug.Log(jsonRequestBody);

		byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
		//Debug.Log(bodyRaw);
		postRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
		postRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		postRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		//postRequest.SetRequestHeader("Content-Type", "application/json");

		yield return postRequest.SendWebRequest();

		if (postRequest.isNetworkError || postRequest.isHttpError)
		{
			Debug.Log(postRequest.responseCode);
			Debug.Log(postRequest.error);
		}
		else
		{
			// Show results as text
			Debug.Log("Response: " + postRequest.downloadHandler.text);

			// Or retrieve results as binary data
			byte[] resultbyte = postRequest.downloadHandler.data;
			string result = System.Text.Encoding.UTF8.GetString(resultbyte);
			ResponseBody content = (ResponseBody)JsonUtility.FromJson<ResponseBody>(result);
			Debug.Log(content.queryResult.responseMessages[0].text);
		}
	}

	IEnumerator GetAgent(String AccessToken)
	{
		UnityWebRequest www = UnityWebRequest.Get("https://dialogflow.googleapis.com/v2/projects/aragenttest-hugf/agent");

		www.SetRequestHeader("Authorization", "Bearer " + AccessToken);

		yield return www.SendWebRequest();
		//myHttpWebRequest.PreAuthenticate = true;
		//myHttpWebRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
		//myHttpWebRequest.Accept = "application/json";

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			// Show results as text
			Debug.Log(www.downloadHandler.text);

			// Or retrieve results as binary data
			byte[] results = www.downloadHandler.data;
		}
	}
}