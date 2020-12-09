using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CompletedSimulation
{
   
    public string transcript;
    public float simulationTime;
  
    public CompletedSimulation()
    {

    }

    public CompletedSimulation( string transcript, float simulationTime)
    {

        this.transcript = transcript;
        this.simulationTime = simulationTime;
    }
}
