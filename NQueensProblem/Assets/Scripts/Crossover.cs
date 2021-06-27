using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossover : MonoBehaviour
{
    int[] parent1 = { 2, 1, 3, 2, 3, 1, 2, 3, 1 };
    int[] parent2 = { 1, 3, 3, 2, 1, 2, 1, 2, 3 };

    void Start()
    {
        ppx();
        pbx();
    }

    // Precedence Preservation Crossover
    public void ppx()
    {
        // Get parent genomes into lists
        List<int> p1 = new List<int>();
        List<int> p2 = new List<int>();

        for (int i = 0; i < parent1.Length; i++)
        {
            p1.Add(parent1[i]);
            p2.Add(parent2[i]);
        }

        // Generate a random binary string
        int[] binarySeq = new int[parent1.Length];
        for (int i = 0; i < binarySeq.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                binarySeq[i] = 1;
            } else
            {
                binarySeq[i] = 0;
            }
        }

        // Generate child genome
        int[] childDNA = new int[parent1.Length];
        for (int i = 0; i < binarySeq.Length; i++)
        {
            if (binarySeq[i] == 0)
            {
                childDNA[i] = p1[0];
                p2.Remove(p1[0]); // remove selected genome from parent 2 DNA
                p1.RemoveAt(0); // remove selected genome from parent 1 DNA
            }
            else
            {
                childDNA[i] = p2[0];
                p1.Remove(p2[0]); // remove selected genome from parent 1 DNA
                p2.RemoveAt(0); // remove selected genome from parent 2 DNA
            } 
        }
        // child DNA collect here
    }

    // Position Based Crossover
    public void pbx()
    {
        // List of available integers from parent 2 after selection from parent 1
        List<int> availableGenesP2 = new List<int>();
        for (int i = 0; i < parent2.Length; i++)
        {
            availableGenesP2.Add(parent2[i]);
        }

        // Get genes from parent 1
        int[] childDNA = new int[parent1.Length];

        for (int i = 0; i < parent1.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                childDNA[i] = parent1[i]; //  place the selected genome from parent 1 into the same index of child DNA
                availableGenesP2.Remove(parent1[i]); // remove selected genome from parent 2
            }
            else
            {
                childDNA[i] = parent1.Length+1; // indicator of empty values in the array (since 0's are default values - cannot use 0 due to DNA encoding)
            }
        }

        // Get genes from parent 2
        for (int i = 0; i < availableGenesP2.Count; i++)
        {
            for (int j = 0; j < childDNA.Length; j++)
            {
                if (childDNA[j] == parent1.Length + 1) // look for any empty slots in the child DNA
                {
                    childDNA[j] = availableGenesP2[i]; // populate with left-most genome from parent 2's DNA
                    break;
                }
            }
        }
    }

    // Method to print integer arrays
    public void printArray(int[] arr)
    {
        string a = "";
        for (int i = 0; i < arr.Length; i++)
        {
            a += arr[i] + ",";
        }
        Debug.Log(a);
    }

}
