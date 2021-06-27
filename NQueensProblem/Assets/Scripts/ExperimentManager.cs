using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EXPERIMENT MANAGER CLASS

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager Instance
    {
        get;
        private set;
    }

    public int numOfRun;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
