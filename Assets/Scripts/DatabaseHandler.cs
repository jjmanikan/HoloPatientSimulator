using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using Proyecto26;

public static class DatabaseHandler
{
    private static readonly string databaseURL = $"https://holopatient.firebaseio.com/";

    private static fsSerializer serializer = new fsSerializer();


    public delegate void PostSimulationCallback();
    public delegate void SubmitCompleteSimulationCallback();
    public delegate void GetSimulationCallback(Simulation simulation);
    public delegate void GetSymptomsCallback(Dictionary<string, string> visualSymptoms);
    public delegate void GetAllSymptomsCallback(Dictionary<string, VisualSymptom> visualSymptoms);
    public delegate void GetActiveSimulationCallback(ActiveSimulation activeSimulation);

    

    public static void GetSimulation(string simulationId, GetSimulationCallback callback)
    {
        RestClient.Get<Simulation>($"{databaseURL}simulations/{simulationId}.json").Then(simulation =>
        {
          
            callback(simulation);
        });
    }

    public static void GetSymptoms(string simulationId, string symptom, GetSymptomsCallback callback)
    {
        RestClient.Get($"{databaseURL}simulations/{simulationId}/visualSymptoms/{symptom}.json").Then(visualSymptoms =>
        {
            var symptomParamJson = visualSymptoms.Text;

            var data = fsJsonParser.Parse(symptomParamJson);

            object deserialized = null;

            serializer.TryDeserialize(data, typeof(Dictionary<string, string>), ref deserialized);

            var symptomParam = deserialized as Dictionary<string, string>;
            callback(symptomParam);
        });
    }

    public static void GetActiveSimulation(string simulationCode, GetActiveSimulationCallback callback)
    {
        RestClient.Get<ActiveSimulation>($"{databaseURL}activeSimulations/{simulationCode}.json").Then(activeSimulation =>
        {

            callback(activeSimulation);
        });
    }

    public static void GetAllSymptoms(string simulationId, GetAllSymptomsCallback callback)
    {
        RestClient.Get($"{databaseURL}simulations/{simulationId}/visualSymptoms.json").Then(response =>
        {

            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, VisualSymptom>), ref deserialized);

            var visualSymptoms = deserialized as Dictionary<string, VisualSymptom>;
            callback(visualSymptoms);
        });
    }

    //put complete simulation to database
    public static void SubmitCompleteSimulation(CompletedSimulation completedSimulation, string simulationID, SubmitCompleteSimulationCallback callback )
    {
        RestClient.Put<CompletedSimulation>($"{databaseURL}completedSimulations/{simulationID}.json", completedSimulation);
    }

    public static void PostSimulation(Simulation simulation, string simulationId, PostSimulationCallback callback)
    {
        //use .Then(response =>{Debug.Log("Test")}; for testing
        RestClient.Put<Simulation>($"{databaseURL}simulations/{simulationId}.json", simulation);

    }

}
