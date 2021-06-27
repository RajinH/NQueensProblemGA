using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// GENETIC ALGORITHM MANAGER CLASS

public class GeneticAlgorithmManager : MonoBehaviour
{
    [Header("UI")]
    public Texture2D updateTexture;
    public MeshRenderer meshRenderer;

    public static int[] grid; // grid array (1 dimensional)
    public static int size = 8; // grid length and width 

    [Header("ALGORITHM PARAMETERS"), Space(10)]
    public int populationMax; // population size
    public float mutationRate; // mutation rate

    [Header("EXPERIMENTATION SETTINGS"), Space(10)]
    bool isPbx = false; // toggle between PBX and PPX crossover
    public int numOfTrials = 5; // number of trials to do

    bool hasRun = false; // check if coroutine has run once
    Sprite texture; // sprite to update UI texture

    Population p; // population instance
    GraphHelp h; // grapher helper instance

    float t; // time

    public bool isExperimentOver = false; // keeps track of experiment status

    string csvPathPPX = "C:/Users/rajin/Desktop/DataPPX2.csv"; // csv path (PPX)
    string csvPathPBX = "C:/Users/rajin/Desktop/DataPBX2.csv"; // csv path (PBX)

    [HideInInspector]
    public List<float> generationNum_; // list of generation numbers
    [HideInInspector]
    public List<float> fitnesses_; // list of fitnesses corresponding to generation numbers

    void Awake()
    {
        if (ExperimentManager.Instance.numOfRun > numOfTrials)
        {
            isExperimentOver = true;
            Debug.Log("Experiment Over!!!");
        }
    }

    void Start()
    {
        if (!isExperimentOver)
        {
            updateTexture = new Texture2D(size, size); // instantiate new texture
            updateTexture.filterMode = FilterMode.Point;

            texture = gameObject.GetComponent<SpriteRenderer>().sprite;
            h = GameObject.FindObjectOfType<GraphHelp>();

            h.AddGraph("graphing", Color.red); // set up grapher
            Debug.Log("Trial No: "+ ExperimentManager.Instance.numOfRun);
            p = new Population(populationMax, mutationRate, isPbx); // create population object
        }
        
    }

    void Update()
    {
        if (!isExperimentOver)
        {
            if (!p.isFinished)
            {
                t += Time.deltaTime; // increment time
                p.naturalSelection(); // natural selection
                p.generateNewPop(); // generation of new population
                p.calcFitness(); // calculate fitnes for all memebers of the population

                generationNum_.Add(p.generationNo); // store generation number in a list
                fitnesses_.Add(p.averageFitness); // store average fitness of each generation in a list

                h.Plot(t, p.averageFitness, 0); // plot the average fitness vs time on the grapher

                if (p.bestChild != null)
                {
                    updateTexture = new Texture2D(size, size);
                    updateTexture.filterMode = FilterMode.Point;

                    for (int x = 0; x < size; x++)
                    {
                        updateTexture.SetPixel(x, p.bestChild.sequence[x], Color.black);
                    }

                    updateTexture.Apply();
                    meshRenderer.material.mainTexture = updateTexture;
                }
            }
            else if (!hasRun && p.isFinished)
            {
                StartCoroutine("updateAttack");
                hasRun = true;
            }
        } 
    }

    // Method to save data to CSV file
    public void saveData()
    {
        // Write data to CSV file
        string data = "";

        // set column headers
        data += "Trial " + ExperimentManager.Instance.numOfRun.ToString() + "\n" + "Gen No, Fitness\n";

        // iterate through all the records and append to string
        for (int i = 0; i < generationNum_.Count; i++)
        {
            data += ((int)generationNum_[i]).ToString() + "," + fitnesses_[i].ToString() + "\n";
        }

        // save file
        if (isPbx)
        {
            File.AppendAllText(csvPathPBX, data);
        } else
        {
            File.AppendAllText(csvPathPPX, data);
        }
        
        Debug.Log("File Saved");
        return;
    }

    IEnumerator updateAttack()
    {
        // if a valid solution is reached
        if (p.isFinished)
        {
            updateTexture = new Texture2D(size, size);
            updateTexture.filterMode = FilterMode.Point;

            // iterate through DNA sequence of the best child (the solution) in order to draw the queen attack paths
            for (int i = 0; i < p.bestChild.sequence.Length; i++)
            {
                // draw horizontals
                int x = i;
                int y = p.bestChild.sequence[i];
                for (int j = 0; j < size - i + 1; j++)
                {
                    x++;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                x = i;
                for (int j = 0; j < i + 1; j++)
                {
                    x--;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                // draw verticals
                x = i;
                for (int j = 0; j < size - p.bestChild.sequence[i] + 1; j++)
                {
                    y++;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                y = p.bestChild.sequence[i];
                for (int j = 0; j < p.bestChild.sequence[i] + 1; j++)
                {
                    y--;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                // draw diagonals
                x = i;
                y = p.bestChild.sequence[i];
                while (x < size - 1 && y < size - 1)
                {
                    x++;
                    y++;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                x = i;
                y = p.bestChild.sequence[i];
                while (x > 0 && y > 0)
                {
                    x--;
                    y--;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                x = i;
                y = p.bestChild.sequence[i];
                while (x < size - 1 && y > 0)
                {
                    x++;
                    y--;
                    updateTexture.SetPixel(x, y, Color.red);
                }

                x = i;
                y = p.bestChild.sequence[i];
                while (x > 0 && y < size - 1)
                {
                    x--;
                    y++;
                    updateTexture.SetPixel(x, y, Color.red);
                }
                updateTexture.SetPixel(i, p.bestChild.sequence[i], Color.black);

                updateTexture.Apply(); // apply updated texture
                meshRenderer.material.mainTexture = updateTexture; // update material on plane
                yield return new WaitForSeconds(1f); // pause for a second
            }

            saveData(); // save data into CSV file
            ExperimentManager.Instance.numOfRun++;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloading Scene
    }
}

