using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Unity.VisualScripting.Member;

/// <summary>
/// Edge class
/// Handles edge functions for all data structures
/// </summary>
public class Edge {

    private Node source;
    private Node destination;
    private float weight;
    private LineRenderer lineRenderer;
    public GameObject go;
    public AlgoState currentState;

    //Constructor
    public Edge(Node _source, Node _destination, float _weight)
    {
        source = _source;
        destination = _destination;
        weight = _weight;
        ResetMaterial();
    }
    //Reset edge for use on the same graph
    public void ResetSameEdge()
    {
        ResetMaterial();
    }
    //Reset edge entirely
    //Cuases it to go to garbage collection
    public void ResetEdge()
    {
        source = null;
        destination = null;
        if(go != null)go.SetActive(false);
        go = null;
        lineRenderer = null;
    }

    //Getter / Setters
    public Node GetSource()
    {
        return source;
    }
    public Node GetDestination()
    {
        return destination;
    }
    public float GetWeight()
    {
        return weight;
    }


    //Graphical functions
    public void ResetMaterial()
    {
        UpdateGraphical(AlgoState.unvisited);
    }
    public void VisitEdge()
    {
        UpdateGraphical(AlgoState.visited);
    }
    public void TraverseEdge()
    {
        UpdateGraphical(AlgoState.traversed);
    }
    public void UpdateGraphical(AlgoState state)
    {
        currentState = state;
        if (state == AlgoState.unvisited && (int)SettingsManager.Instance.graphicalType >= 3) ActivateGrahpicalGO();
        else if (state == AlgoState.visited && (int)SettingsManager.Instance.graphicalType >= 2) ActivateGrahpicalGO();
        else if (state == AlgoState.traversed && (int)SettingsManager.Instance.graphicalType >= 1) ActivateGrahpicalGO();
        else DeActivateGrahpicalGO();
    }
    public void ActivateGrahpicalGO()
    {
        if (go == null)
        {
            go = PoolManager.Instance.GetEdge();
            lineRenderer = go.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
        }
        lineRenderer.SetPosition(0, source.GetPosition());
        lineRenderer.SetPosition(1, destination.GetPosition());
        float size = Utilities.GetPercentage(weight, SettingsManager.Instance.minWeight, SettingsManager.Instance.maxWeight);
        lineRenderer.startWidth = size * SettingsManager.Instance.maxLineSize;
        lineRenderer.endWidth = size * SettingsManager.Instance.maxLineSize;

        if (source.GetIsWalkable())
        {
            if (currentState == AlgoState.unvisited)
            {
                lineRenderer.startColor = SettingsManager.Instance.lineDefaultColor;
                lineRenderer.endColor = SettingsManager.Instance.lineDefaultColor;
            }
            else if (currentState == AlgoState.visited)
            {
                lineRenderer.startColor = SettingsManager.Instance.lineVisitedColor;
                lineRenderer.endColor = SettingsManager.Instance.lineVisitedColor;
            }
            else if (currentState == AlgoState.traversed)
            {
                lineRenderer.startColor = SettingsManager.Instance.lineTraversalColor;
                lineRenderer.endColor = SettingsManager.Instance.lineTraversalColor;
            }
        }
        else
        {
            lineRenderer.startColor = SettingsManager.Instance.lineNotWalkableColor;
            lineRenderer.endColor = SettingsManager.Instance.lineNotWalkableColor;
        }

    }
    public void DeActivateGrahpicalGO()
    {
        if(go != null)go.SetActive(false);
        go = null;
        lineRenderer = null;
    }
}