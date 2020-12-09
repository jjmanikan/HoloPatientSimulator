using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationObjectManager : MonoBehaviour
{
    public Simulation simulation;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        simulation = new Simulation();
        //simulation.physicalSymptoms = new Dictionary<string, string>;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
