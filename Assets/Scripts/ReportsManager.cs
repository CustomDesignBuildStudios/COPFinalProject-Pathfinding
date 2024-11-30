using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataPoint
{
    public DataTypes dataTypes;
    public AlgoTypes algoTypes;
    public GraphTypes graphTypes;
    public int density;
    public int minWeight;
    public int maxWeight;
    private int size;
    public int gridSize;
    public float maxLineSize;
    public float timeToRun;

}


public class ReportsManager : MonoBehaviour
{
    // Singleton 
    public static ReportsManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



     
}
