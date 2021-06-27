using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// DNA CLASS

public class Dna
{
    public int[] sequence; // genetic encoding
    public float fitness; // fitness of the individual

    public Dna(int size)
    {
        sequence = new int[size];

        // generate a random genetic encoding or DNA
        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = Random.Range(0, size);
        }
    }

    // Calculate Fitness of Individual
    public void CalculateFitness()
    {
        int collisions = 0; // counter for the number of queens that attack each other

        // iterate through the DNA sequence or genetic endoing
        for (int i = 0; i < sequence.Length - 1; i++)
        {
            int x = i; // x coordinate of queen
            int y = sequence[i]; // y coordinate of queen

            // iterate through the DNA sequence from the second queen in the sequence
            for (int j = i + 1; j < sequence.Length; j++)
            {
                // checks if queen and next queen positions are on the same horizontal line, vertical line, or diagonal lines
                if (Math.Abs(j - x) == Math.Abs(sequence[j] - y) || y == sequence[j] || x == j)
                {
                    collisions++;
                }        
            }
        }

        //fitness = 100f-(2f*collisions); // Fitness function
        fitness = 100 / (1 + 2 * collisions); // Fitness function 2
    }

    // Crossover Methods

    // 1PX
    public Dna onePoint(Dna partner)
    {
        
        Dna child1 = new Dna(sequence.Length);
        int a = Random.Range(0, sequence.Length);

        for (int i = 0; i < sequence.Length; i++)
        {
            if (i < a)
            {
                child1.sequence[i] = sequence[i];
            } else
            {
                child1.sequence[i] = partner.sequence[i];
            }
        }
        return child1;
    }

    // Precedence Preservation Crossover
    public Dna ppx(Dna parent2)
    {
        // Get parent genomes into lists
        List<int> p1 = new List<int>();
        List<int> p2 = new List<int>();

        for (int i = 0; i < sequence.Length; i++)
        {
            p1.Add(sequence[i]);
            p2.Add(parent2.sequence[i]);
        }

        // Generate a random binary string
        int[] binarySeq = new int[sequence.Length];
        for (int i = 0; i < binarySeq.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                binarySeq[i] = 1;
            }
            else
            {
                binarySeq[i] = 0;
            }
        }

        // Generate child DNA
        Dna child = new Dna(sequence.Length);
        for (int i = 0; i < binarySeq.Length; i++)
        {
            if (binarySeq[i] == 0)
            {
                child.sequence[i] = p1[0];
                p2.Remove(p1[0]); // remove selected genome from parent 2 DNA
                p1.RemoveAt(0); // remove selected genome from parent 1 DNA
            }
            else
            {
                child.sequence[i] = p2[0];
                p1.Remove(p2[0]); // remove selected genome from parent 1 DNA
                p2.RemoveAt(0); // remove selected genome from parent 2 DNA
            }
        }

        return child;
    }

    // Position Based Crossover
    public Dna pbx(Dna parent2)
    {
        // List of available integers from parent 2 after selection from parent 1
        List<int> availableGenesP2 = new List<int>();
        for (int i = 0; i < parent2.sequence.Length; i++)
        {
            availableGenesP2.Add(parent2.sequence[i]);
        }

        // Generate child DNA
        // Get genes from parent 1
        Dna child = new Dna(sequence.Length);
        for (int i = 0; i < sequence.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                child.sequence[i] = sequence[i]; //  place the selected genome from parent 1 into the same index of child DNA
                availableGenesP2.Remove(sequence[i]); // remove selected genome from parent 2
            }
            else
            {
                child.sequence[i] = sequence.Length + 1; // indicator of empty values in the array (since 0's are default values - cannot use 0 due to DNA encoding)
            }
        }

        // Get genes from parent 2
        for (int i = 0; i < availableGenesP2.Count; i++)
        {
            for (int j = 0; j < child.sequence.Length; j++)
            {
                if (child.sequence[j] == sequence.Length + 1) // look for any empty slots in the child DNA
                {
                    child.sequence[j] = availableGenesP2[i]; // populate with left-most genome from parent 2's DNA
                    break;
                }
            }
        }

        return child;
    }

    // Mutation Method
    public void Mutation()
    {
        int a = Random.Range(0, sequence.Length);
        int b = Random.Range(0, sequence.Length);

        // swap gene at two random points in the DNA sequence
        int temp = sequence[a];
        sequence[a] = sequence[b];
        sequence[b] = temp;
    }
}
