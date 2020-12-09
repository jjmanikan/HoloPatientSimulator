using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisualSymptom 
{
    public string visualSymptomName;
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
    public float scaleX, scaleY, scaleZ;

    public VisualSymptom()
    {

    }

    public VisualSymptom(string visualSymptomName, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float scaleX, float scaleY, float scaleZ)
    {
        this.visualSymptomName = visualSymptomName;
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
        this.rotX = rotX;
        this.rotY = rotY;
        this.rotZ = rotZ;
        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.scaleZ = scaleZ;
    }
}
