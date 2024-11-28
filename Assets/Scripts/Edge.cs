using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Edge {

    private Node source;
    private Node destination;
    private float weight;
    private LineRenderer lineRenderer;
    public GameObject go;
    public AlgoState currentState;

    public Edge(Node _source, Node _destination, float _weight)
    {
        source = _source;
        destination = _destination;
        weight = _weight;
        ResetMaterial();
    }
    public void ResetSameEdge()
    {
        ResetMaterial();
    }
    public void ResetEdge()
    {
        source = null;
        destination = null;
        if(go != null)go.SetActive(false);
        go = null;
        lineRenderer = null;
    }


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
        Debug.Log(currentState);
        Debug.Log("ActivateGrahpicalGO");
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