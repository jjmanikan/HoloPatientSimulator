
//using Microsoft.MixedReality.Toolkit;
//using System.Linq;
//using System.Runtime.CompilerServices;
using DecalSystem;
using FullSerializer;
using JsonData;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using TMPro;
using TTSJsonData;
using UnityEditor;
using UnityEngine;
//using Microsoft.Speech.Sythesis;
using UnityEngine.Networking;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
//using Proyecto26;

[RequireComponent(typeof(AudioSource))]
public class LoadSimulation : MonoBehaviour
{
    private float DEF_SCALE_X = 0.75f;
    private float DEF_SCALE_Y = 0.75f;
    private float DEF_SCALE_Z = 0.75f;

    public GameObject botResponses;
    public GameObject simulationDescription;

    [Header("Decal Prefab")]
    public GameObject decalPrefab;

    [Header("Layermask")]
    public LayerMask layermask;

    [Header("Materials")]
    public Material bruiseMat;
    public Material cutMat;
    public Material rashMat;
    public Material scrapeMat;

    [Header("Sprites")]
    public Sprite bruiseSprite;
    public Sprite cutSprite;
    public Sprite rashSprite;
    public Sprite scrapeSprite;

    // Body Parts Dictionary
    private Dictionary<string, GameObject> bodyPartsDictionary;

    // Visual Symptom Dictionary
    private Dictionary<string, Material> visualSymptomMatDictonary;
    private Dictionary<string, Sprite> visualSymptomSpriteDictionary;

    private static fsSerializer serializer = new fsSerializer();

    //A boolean that flags whether there's a connected microphone
    private bool micConnected = false;

    //The maximum and minimum available recording frequencies
    private int minFreq;
    private int maxFreq;

    //A handle to the attached AudioSource
    private AudioSource goAudioSource;

    //Public variable for saving recorded sound clip
    public AudioClip recordedClip;
    private float[] samples;
    private byte[] bytes;
    //dialogflow
    private AudioSource audioSource;
    
    //private volatile
    public volatile bool recordingActive;

    //transcript object to be saved to firebase
    public string transcript = "";

    public GameObject buttonObject;

    private TextToSpeech textToSpeech;
    public AudioSource sharedAudioSource;
    public Interactable button1;
    public Interactable button2;
    public Interactable simulationCode;
    public Interactable submitReset;
    public TouchScreenKeyboard keyboard;

    public string accessToken;
    public string simulationID;
    public string userID;
    public string completeSimulationID;
    public string keyBoardText;
    public string agentID;
  
    
    public GameObject canvasObject;
    public InputField codeInput;

    public bool time;
    public float simulationTime;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    // Start is called before the first frame update
    void Start()
    {
        // AccessToken is being generated manually in terminal
        //StartCoroutine(GetAgent("ya29.c.ElpfBkjOUlTRSaNDg-i0tBjGc2WlRT9GePIqe1_j5Xq9flXHMGJWnn5sEjNHyG1VfMFqtt3WapHAVo2-RwvPNKRTHI0BkF9OVUzZJ5OWJEILr64_ge1tgcbS7AA"));
        //https://stackoverflow.com/questions/51272889/unable-to-send-post-request-to-dialogflow-404
        //first param is the dialogflow API call, second param is Json web token
        //StartCoroutine(PostRequest("https://dialogflow.googleapis.com/v2/projects/aragenttest-hugf/agent/sessions/34563:detectIntent",
        // "ya29.c.Kp0B5gco1DystKb2_PJT3P2M68N0KLZgbEzabqIzxvNMJCi7gb5QzhDvLZsBzSh8IO51cUrbE44EsXtvtv2Y5pGcdw06i9_U1ge6r68DeRJVTqcX61y83vx8HwK8jeGfU2CJ1TJQNKcwiKgEp3AyGh6lJs88FM3kX59M4IU7MJvJsMBnir9Lxf5nNkov6Fov1OouIZcL-lvqSgRsDfQTsg"));
        //canvasObject.SetActive(true);
        textToSpeech = GetComponent<TextToSpeech>();

        // Visual Symptoms Materials
        visualSymptomMatDictonary = new Dictionary<string, Material>();

        visualSymptomMatDictonary["Bruise"] = bruiseMat;
        visualSymptomMatDictonary["Cut"] = cutMat;
        visualSymptomMatDictonary["Rash"] = rashMat;
        visualSymptomMatDictonary["Scrape"] = scrapeMat;

        // Visual Symptoms Sprites
        visualSymptomSpriteDictionary = new Dictionary<string, Sprite>();

        visualSymptomSpriteDictionary["Bruise"] = bruiseSprite;
        visualSymptomSpriteDictionary["Cut"] = cutSprite;
        visualSymptomSpriteDictionary["Rash"] = rashSprite;
        visualSymptomSpriteDictionary["Scrape"] = scrapeSprite;


        

        //Check if there is at least one microphone connected
        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't
            Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present
        {
            //Set 'micConnected' to true
            micConnected = true;

            //Get the default microphone recording capabilities
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate
                maxFreq = 44100;
            }

            //Get the attached AudioSource component
            goAudioSource = this.GetComponent<AudioSource>();
        }

        button1.OnClick.AddListener(StartListening);
        button2.OnClick.AddListener(StopListening);
        simulationCode.OnClick.AddListener(InputSimulationCode);
        submitReset.OnClick.AddListener(SubmitSimulationAttempt);

    }

    // Update is called once per frame
    void Update()
    {
        if(time == true)
        {
            simulationTime += Time.deltaTime * 1;
        }
        else
        {

        }
    }

    public void ResetSimulation()
    {
        //looks for each gameobject with tag DecalInstance
        GameObject[] decalInstances = GameObject.FindGameObjectsWithTag("DecalInstance");

        //delete each one
        foreach (GameObject decalInstance in decalInstances)
        {
            GameObject.Destroy(decalInstance);
        }
    }

    public void SubmitSimulationAttempt()
    {
       
        float rounded = (float)(Math.Round((double)simulationTime, 0)); 
        time = false;

        //
        if(completeSimulationID != null && transcript != null) {
            //remove all decals
            ResetSimulation();

            //instantiate complete simulation
            var completeSimulation = new CompletedSimulation(transcript, rounded);
            DatabaseHandler.SubmitCompleteSimulation(completeSimulation, completeSimulationID, () =>
            {

            });

            transcript = "";
            agentID = "";
            simulationDescription.GetComponentInChildren<TextMeshProUGUI>().text = "";
            botResponses.GetComponentInChildren<TextMeshPro>().text = "";

            canvasObject.SetActive(true);

        }

        simulationTime = 0;
        

    }
    public void InputSimulationCode()
    {
        //if (canvasObject.activeSelf == false)
        //{
        //    simulationCode.OnClick.AddListener(GetActiveSimulationCode);
        //}
        //if (canvasObject.activeSelf == true)
        //{

            time = true;
           canvasObject.SetActive(false);
           GetSimulationFromSimulationCode(codeInput.text);
        //}
    }

    public void GetSimulationFromSimulationCode(string code)
    {
        //obtain simulation code
        DatabaseHandler.GetActiveSimulation(code, activeSimulation =>
        {
            accessToken = activeSimulation.accessToken;
            userID = activeSimulation.userID;
            completeSimulationID = activeSimulation.completeSimulationID;
            Debug.Log($"Access token:{accessToken}");

            //takes in specific simulation id
            DatabaseHandler.GetSimulation(activeSimulation.simulationID, simulation =>
            {
                simulationID = activeSimulation.simulationID;
                agentID = simulation.agentID;
                string test = "";
                test = test + $"Simulation:" +
                $" {simulation.simulationTitle} <br><br>Simulation Description: {simulation.simulationDescription}<br>";

                simulationDescription.GetComponentInChildren<TextMeshProUGUI>().text = test;

                //draw every symptom on model
                foreach (var i in simulation.visualSymptoms)
                {
                    string visualSymptomName = i.visualSymptomName;
                    float posX = i.posX;
                    float posY = (float)(i.posY - 17);
                    //compensate for mrtk camera / float 12.6/0.5
                    float posZ = (float)(i.posZ + 15);
                    float rotX = i.rotX;
                    float rotY = i.rotY;
                    float rotZ = i.rotZ;
                    float scaleX = i.scaleX;
                    float scaleY = i.scaleY;
                    float scaleZ = i.scaleZ;

                    //instatiate decal object
                    GameObject visualSymptomObj = Instantiate(decalPrefab, new Vector3(posX, posY, posZ), Quaternion.Euler(rotX, rotY, rotZ));
                    visualSymptomObj.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                    visualSymptomObj.tag = "DecalInstance";

                    Decal decalScript = visualSymptomObj.GetComponent<Decal>();
                    decalScript.Material = visualSymptomMatDictonary[visualSymptomName];
                    decalScript.Sprite = visualSymptomSpriteDictionary[visualSymptomName];
                    decalScript.BuildAndSetDirty();

                }

            });
        });
    }

  
    public void GetActiveSimulationCode()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        canvasObject.SetActive(true);
    }

    //old way of applying visual symptoms
    /*public void ApplySymptoms(string bodypart, string symptom)
    {
        //body part
        Decal scriptBodyPartDecal = bodyPartsDictionary[bodypart].GetComponent<Decal>();

        //apply material
        scriptBodyPartDecal.Material = visualSymptomMatDictonary[symptom];
        scriptBodyPartDecal.Sprite = visualSymptomSpriteDictionary[symptom];
        scriptBodyPartDecal.BuildAndSetDirty();

        //add visual symptom
        //simulationObjectManager.simulation.physicalSymptoms[bodypart] = symptom;

    }*/

    //testing microphone in unity editor
    void OnGUI()
    {
        //If there is a microphone
       /* if (micConnected)
        {
            //If the audio from any microphone isn't being captured
            if (!Microphone.IsRecording(null))
            {
                //Case the 'Record' button gets pressed
                if (GUI.Button(new Rect(625, 250, 150, 50), "Start Simulation"))
                {
                    //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource
                    //goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
                    //recordedClip = goAudioSource.clip;
                    //samples = new float[goAudioSource.clip.samples];
                    //handle dialogflow
                    StartListening();
                }
            }
            else //Recording is in progress
            {
       
                //Case the 'Stop and Play' button gets pressed
                if (GUI.Button(new Rect(625, 250, 150, 50), "Stop"))
                {
                    //Microphone.End(null); //Stop the audio recording
                    //goAudioSource.Play(); //Playback the recorded audio
                    //Debug.Log(recordedClip.length);
                    //send out request
                    StopListening();
                }

                GUI.Label(new Rect(625, 300, 150, 50), "Simulation in progress");
            }
        }
        else // No microphone
        {
            //Print a red "Microphone not connected!" message at the center of the screen
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(Screen.width / 4 - 100, Screen.height / 4 - 25, 200, 50), "Microphone not connected!");
        }*/

    }

    //start simulation function
    public void SimulationStart()
    {
        //checks if mic is connected
        if (micConnected)
        {
            if (!Microphone.IsRecording(null))
            {
                if (buttonObject.GetComponent<TextMesh>().text == "Click to talk")
                {
                    //goAudioSource
                    StartListening();
                }

                //buttonObject.GetComponent<TextMesh>().text = "Stop response";
            }
            else
            {
                if (buttonObject.GetComponent<TextMesh>().text == "Stop response")
                {
                    StopListening();
                    buttonObject.GetComponent<TextMesh>().text = "Click to talk";
                }
            }
        }
        else
        {
            Debug.Log("Microphone not connected");
        }
    }
    public void StartListening()
    {
        //lock (thisLock)
        //{
            if (!recordingActive)
            {
                //this.goAudioSource = goAudioSource;
                StartRecording();
            }
            else
            {
                Debug.LogWarning("Can't start new recording session while another recording session active");
            }
        //}
    }

    private void StartRecording()
    {
        goAudioSource.clip = Microphone.Start(null, true, 20, 16000);
        recordingActive = true;

        //FireOnListeningStarted();
    }

    public void StopListening()
    {
        //if (recordingActive)
        //{

        //float[] samples = null;

            //lock (thisLock)
            //{
                if (recordingActive)
                    {
                        StopRecording();
                        //samples = new float[audioSource.clip.samples];

                        //audioSource.clip.GetData(samples, 0);
                        bytes = WavUtility.FromAudioClip(goAudioSource.clip);
                        //audioSource.Play();
                        Debug.Log("This is the audiosource clip length: " + bytes.Length);
                        audioSource = null;
                    }
        //}

        //new Thread(StartVoiceRequest).Start(samples); https://dialogflow.googleapis.com/v2/projects/aragenttest-hugf/agent/sessions/34563:detectIntent
        //https://dialogflow.googleapis.com/v3beta1/projects/holopatient/locations/us/agents/fb5a179d-8302-41a0-a870-3277460dc856/sessions/34563:detectIntent
        StartCoroutine(StartVoiceRequest($"https://dialogflow.googleapis.com/v3beta1/{agentID}/sessions/34563:detectIntent",
                accessToken,
                bytes));

            
        //}
    }

    private void StopRecording()
    {
        Microphone.End(null);
        recordingActive = false;
    }

    //voice request method
    IEnumerator StartVoiceRequest(String url, String AccessToken, object parameter)
    {
        byte[] samples = (byte[])parameter;
        //TODO: convert float[] samples into bytes[]
        //byte[] sampleByte = new byte[samples.Length * 4];
        //Buffer.BlockCopy(samples, 0, sampleByte, 0, sampleByte.Length);

        string sampleString = System.Convert.ToBase64String(samples);
        if (samples != null)
        {
            //json object creation to send to dialogflow
            UnityWebRequest postRequest = new UnityWebRequest(url, "POST");
            RequestBody requestBody = new RequestBody();
            requestBody.queryInput = new queryInput();
            requestBody.queryInput.languageCode = "en";
            requestBody.queryInput.audio = new Audio();
            requestBody.queryInput.audio.config = new InputAudioConfig();
            requestBody.queryInput.audio.config.audioEncoding = JsonData.AudioEncoding.AUDIO_ENCODING_UNSPECIFIED;
            //TODO: check if that the sample rate hertz
            requestBody.queryInput.audio.config.sampleRateHertz = 16000;
            requestBody.queryInput.audio.audio = sampleString;
            

            string jsonRequestBody = JsonUtility.ToJson(requestBody, true);
            Debug.Log(jsonRequestBody);

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
            postRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
            postRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            postRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //postRequest.SetRequestHeader("Content-Type", "application/json");
            //
            yield return postRequest.SendWebRequest();

            if (postRequest.isNetworkError || postRequest.isHttpError)
            {
                Debug.Log(postRequest.responseCode);
                Debug.Log(postRequest.error);
                Debug.Log(postRequest.downloadHandler.text);
            }
            else
            {

                Debug.Log("Response: " + postRequest.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] resultbyte = postRequest.downloadHandler.data;
                string result = System.Text.Encoding.UTF8.GetString(resultbyte);
                ResponseBody content = (ResponseBody)JsonUtility.FromJson<ResponseBody>(result);
                Debug.Log(content.queryResult.responseMessages[0].text.text[0]);
                // for testing purposes
                transcript = transcript + "Trainee: " + content.queryResult.transcript + "<br>Patient: " + content.queryResult.responseMessages[0].text.text[0] + "<br>";
                botResponses.GetComponentInChildren<TextMeshPro>().text = transcript;

                textToSpeech.StartSpeaking(content.queryResult.responseMessages[0].text.text[0]);

                //StartCoroutine(StartTextToSpeech("https://texttospeech.googleapis.com/v1/text:synthesize",
                //"ya29.c.Kp0B5gco1DystKb2_PJT3P2M68N0KLZgbEzabqIzxvNMJCi7gb5QzhDvLZsBzSh8IO51cUrbE44EsXtvtv2Y5pGcdw06i9_U1ge6r68DeRJVTqcX61y83vx8HwK8jeGfU2CJ1TJQNKcwiKgEp3AyGh6lJs88FM3kX59M4IU7MJvJsMBnir9Lxf5nNkov6Fov1OouIZcL-lvqSgRsDfQTsg",
                //content.queryResult.fulfillmentText));
            }
        }
        else
        {
            Debug.LogError("The audio file is null");
        }
    }

    private void StartVoiceRequest(object parameter)
    {
        float[] samples = (float[])parameter;
        if (samples != null)
        {
            /*try
             {
                 var aiResponse = apiAi.VoiceRequest(samples);
                 ProcessResult(aiResponse);
             }
             catch (Exception ex)
             {
                 FireOnError(ex);
             }*/
        }
    }

    IEnumerator PostRequest(String url, String AccessToken)
    {
        UnityWebRequest postRequest = new UnityWebRequest(url, "POST");
        RequestBody requestBody = new RequestBody();
        requestBody.queryInput = new queryInput();
    
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

   
}
