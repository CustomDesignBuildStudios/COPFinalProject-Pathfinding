using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;


public class RunReport
{
    public DataTypes dataTypes;
    public AlgoTypes algoType;
    public GraphTypes graphTypes;
    public int density;
    public int minWeight;
    public int maxWeight;
    private int size;
    public int gridSize;
    public float maxLineSize;
    public float timeToRun;
    public int nodesVisited;
    public int pathLength;
    //public float avgerageFPS;
    public ReportTextUI textUI;
    public RunReport(DataTypes dataTypes, AlgoTypes algoType, GraphTypes graphTypes,int density, int minWeight, int maxWeight, int size, int gridSize, float maxLineSize, float timeToRun)
    {
        this.nodesVisited = 0;
        this.pathLength = 0;
        this.dataTypes = dataTypes;
        this.algoType = algoType;
        this.graphTypes = graphTypes;
        this.density = density;
        this.minWeight = minWeight;
        this.maxWeight = maxWeight;
        this.size = size;
        this.gridSize = gridSize;
        this.maxLineSize = maxLineSize;
        this.timeToRun = timeToRun;
        this.textUI = null;
    }
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

        runs = new List<RunReport> ();
    }


    public List<RunReport> runs;
    public Transform reportsUIHolder;
    public TMP_Text lastRunUIText;
    public GameObject textPrefabGO;
    public GameObject reportPanelGO;

    public void ToggleReports()
    {
        if(reportPanelGO.activeSelf) reportPanelGO.SetActive (false);
        else reportPanelGO.SetActive (true);
    }
    public void AddReport(RunReport report)
    {
        lastRunUIText.text = $"Last Run: {report.timeToRun}";


        GameObject textGO = Instantiate(textPrefabGO, reportsUIHolder);
        ReportTextUI reportText = textGO.GetComponent<ReportTextUI>();
        report.textUI = reportText;
        report.textUI.resultsText.text = report.timeToRun.ToString();
        report.textUI.settingsText.text = $"Settings: Visited {report.nodesVisited} | Path {report.pathLength} | " +
            $"{Enum.GetName(typeof(AlgoTypes), report.algoType)} | " +
            $"{Enum.GetName(typeof(GraphTypes), report.graphTypes)} | " +
            $" ";

        runs.Add(report);
    }
     
}
