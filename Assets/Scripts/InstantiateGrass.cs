using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGrass : MonoBehaviour
{
    public Grass grassObject;
    public int startingNumber;

    public void Awake()
    {

    }

    public void SpawnFirstSeed()
    {
        for (int i = 0; i < startingNumber; i++)
        {
            grassObject.Default();
            grassObject.NewSeed();
        }
    }
}
