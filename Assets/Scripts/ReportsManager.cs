using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// RunReport
/// Holds the data for each report
/// Settings and Results
/// If using agents, each time agent requests path the report will update
/// </summary>
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


    public int runCount;


    public float maxTimeToRun;
    public float minTimeToRun;
    public float avgTimeToRun;
    public float lastTimeToRun;
    public float totalTimeToRun;



    public float avgNodesVisited;
    public int maxNodesVisited;
    public int minNodesVisited;
    public int totalNodesVisited;


    public float avgPathLength;
    public int maxPathLength;
    public int minPathLength;
    public int totalPathLength;


    public ReportTextUI textUI;


    //Updates the report with the run time and nodes visited
    public void UpdateReport(float timeToRun, int nodesVisited)
    {
        runCount++;

        lastTimeToRun = timeToRun;
        totalNodesVisited += nodesVisited;
        totalTimeToRun += timeToRun;

        if (timeToRun > maxTimeToRun) maxTimeToRun = timeToRun;
        if (nodesVisited > maxNodesVisited) maxNodesVisited = nodesVisited;

        if (timeToRun < minTimeToRun) minTimeToRun = timeToRun;
        if (nodesVisited < minNodesVisited) minNodesVisited = nodesVisited;

        avgNodesVisited = totalNodesVisited / runCount;
        avgTimeToRun = totalTimeToRun / runCount;

    }
    //finishes report by adding path length
    public void FinishRunReport(int pathLength)
    {
        totalPathLength += pathLength;
        if (pathLength > maxPathLength) maxPathLength = pathLength;
        if (pathLength < minPathLength) minPathLength = pathLength;

        avgPathLength = totalPathLength / runCount;

    }

    //Constructor
    public RunReport(DataTypes dataTypes, AlgoTypes algoType, GraphTypes graphTypes,int density, int minWeight, int maxWeight, int size, int gridSize, float maxLineSize)
    {
        this.runCount = 0;
        this.lastTimeToRun = 0;

        this.totalNodesVisited = 0;
        this.avgNodesVisited = 0;
        this.minNodesVisited = int.MaxValue;
        this.maxNodesVisited = 0;

        this.totalPathLength = 0;
        this.avgPathLength = 0;
        this.minPathLength = int.MaxValue;
        this.maxPathLength = 0;

        this.totalTimeToRun = 0;
        this.maxTimeToRun = 0;
        this.minTimeToRun = int.MaxValue;
        this.avgTimeToRun = 0;

        this.dataTypes = dataTypes;
        this.algoType = algoType;
        this.graphTypes = graphTypes;
        this.density = density;
        this.minWeight = minWeight;
        this.maxWeight = maxWeight;
        this.size = size;
        this.gridSize = gridSize;
        this.maxLineSize = maxLineSize;




        this.textUI = null;
    }
}






/// <summary>
/// Manages the reports and ui
/// </summary>
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

    // Toggles reports panel ui
    public void ToggleReports()
    {
        if(reportPanelGO.activeSelf) reportPanelGO.SetActive (false);
        else reportPanelGO.SetActive (true);
    }
    //Adds report to ui
    public void AddReport(RunReport report)
    {

        lastRunUIText.text = $"Last Run: {Math.Round(report.lastTimeToRun,5)}";

        // if report does not have ui create it
        if(report.textUI == null)
        {
            GameObject textGO = Instantiate(textPrefabGO, reportsUIHolder);
            ReportTextUI reportText = textGO.GetComponent<ReportTextUI>();
            report.textUI = reportText;
        }

        //update report ui text
        report.textUI.resultsText.text = $"Run Times: {report.runCount} | Avg Time: {Math.Round(report.avgTimeToRun, 5)} | Avg Path: {Math.Round(report.avgPathLength, 1)} | Avg Visited: {Math.Round(report.avgNodesVisited, 1)}";

        report.textUI.settingsText.text = $"Settings: " +
            $"{Enum.GetName(typeof(AlgoTypes), report.algoType)} | " +
            $"{Enum.GetName(typeof(GraphTypes), report.graphTypes)} | " +
            $"{Enum.GetName(typeof(DataTypes), report.dataTypes)}";

        runs.Add(report);
    }
     
}
