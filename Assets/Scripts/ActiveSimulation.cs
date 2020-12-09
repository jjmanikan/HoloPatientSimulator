using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ActiveSimulation 
{
    public string accessToken;
    public string completeSimulationID;
    public string simulationID;
    public string userID;
    
    public ActiveSimulation()
    {

    }

    public ActiveSimulation(string accesstoken, string completeSimulationID, string simulationID, string userID)
    {
        this.accessToken = accesstoken;
        this.completeSimulationID = completeSimulationID;
        this.simulationID = simulationID;
        this.userID = userID;
    }
}
