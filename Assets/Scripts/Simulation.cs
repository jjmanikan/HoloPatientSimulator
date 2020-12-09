using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Simulation 
{
    //put file/simulation attributes
    public string agentID;
    public string correctDiagnosis;
    public string instructorID;

    public string patientAge;
    public string patientBloodPressure;
    public string patientBodyTemp;
    public string patientHeartRate;
    public string patientRespRate;
    public string patientName;

    public string simulationDescription;
    public string simulationTitle;
    public string virtualClassroomID;
    public List<VisualSymptom> visualSymptoms;

    public Simulation()
    {

    }
    public Simulation(string agentID, string correctDiagnosis, string instructorID, string simulationDescription, string simulationTitle, string virtualClassroomID, List<VisualSymptom> visualSymptoms)
    {
        this.agentID = agentID;
        this.correctDiagnosis = correctDiagnosis;
        this.instructorID = instructorID;
        this.simulationDescription = simulationDescription;
        this.simulationTitle = simulationTitle;
        this.virtualClassroomID = virtualClassroomID;
        this.visualSymptoms = visualSymptoms;
    }
}
