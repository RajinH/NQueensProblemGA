using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// POPULATION CLASS

public class Population
{
    public float mutationRate; // probability of a member being picked for mutation
    public Dna[] population; // population of DNA

    List<Dna> matingPool = new List<Dna>(); // mating pool for the creation of the new generation

    public float targetFitness = 100f; // max fitness
    public float averageFitness; // average fitness

    public bool isPbx; // toggle between PBX and PPX

    public int generationNo; // generation number
    public bool isFinished = false; // boolean to check when it is finished
    public Dna bestChild; // best child of the generation

    public Population(int size, float mutationRate_, bool isPbx_)
    {
        mutationRate = mutationRate_;
        population = new Dna[size];
        isPbx = isPbx_;

        // create a number of DNA objects
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = new Dna(GeneticAlgorithmManager.size);
        }

        calcFitness(); // calculate fitness
        isFinished = false;
        generationNo = 0;
        
    }

    // Calculate Fitness of Population
    public void calcFitness()
    {
        // call calculate fitness method on all DNA objects in population
        for (int i = 0; i < population.Length; i++)
        {
            population[i].CalculateFitness();
            //Debug.Log(population[i].fitness);
        }
    }

    // Maps a float value between two set values
    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    // Natural Selection
    public void naturalSelection()
    {
        matingPool.Clear();

        // Caculate max fitness and average fitness
        float maxFitness = 0f;
        float totalFitness = 0f;

        // iterate through all members of population
        for (int i = 0; i < population.Length; i++)
        {
            // check for better fitness
            if (population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
            }

            totalFitness = totalFitness + population[i].fitness; // add to total fitness
        }

        averageFitness = totalFitness / population.Length; // set average fitness
        Debug.Log(totalFitness + " - " + population.Length);

        // Add to mating pool based on fitness
        for (int i = 0; i < Mathf.RoundToInt(population.Length * 0.2f); i++)
        {
            int mutiplier = (int)(population[i].fitness / maxFitness * 100);
            for (int j = 0; j < mutiplier; j++)
            {
                matingPool.Add(population[i]);
            }
        }

        // Sort population - descending order
        for (int i = 0; i < population.Length - 1; i++)
        {
            for (int j = i + 1; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    Dna temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }

        // Assign best child Dna
        bestChild = population[0];

        // If solution is found
        if (bestChild != null && bestChild.fitness == targetFitness)
        {
            //Debug.Log("FOUND on Generation:" + generationNo);

            string a = "";
            for (int i = 0; i < bestChild.sequence.Length; i++)
            {
                a += bestChild.sequence[i];
            }
            
            //Debug.Log("Solution:" + a);
            for (int i = 0; i < population.Length; i++)
            {
                Debug.Log(population[i].fitness);
            }
 
            isFinished = true;
        }
    }

    // Generate New Population
    public void generateNewPop()
    {
        // get the two children from each crossover
        for (int i = 0; i < population.Length / 2; i += 2)
        {
            //gets 2 random parents
            int a = Random.Range(0, matingPool.Count);
            int b = Random.Range(0, matingPool.Count);

            // assigns a different DNA object to the first parent
            while (b == a)
            {
                b = Random.Range(0, matingPool.Count);
            }

            Dna parentA = matingPool[a];
            Dna parentB = matingPool[b];

            Dna child = new Dna(parentA.sequence.Length);

            // crossover of parent DNA sequences to generate child DNA sequence
            if (isPbx)
            {
                child = parentA.pbx(parentB); // crossing over parent DNA with PBX
            } else
            {
                child = parentA.ppx(parentB); // crossing over parent DNA with PPX
            }
            
            population[i] = child; // place child into new population 
        }

        // mutate child based on the mutation rate
        for (int i = 0; i < population.Length; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                population[i].Mutation();
            }
        }
        generationNo++; // increment geneneration number
    }
}