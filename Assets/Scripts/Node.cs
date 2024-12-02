using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Node class
/// Can handle the different data structures requirements for nodes
/// </summary>
public class Node
{
    private int gridX;
    private int gridZ;
    private string key;
    private Vector3 worldPosition;
    private bool isWalkable;
    private List<Edge> neighbors = new List<Edge>();
    private Renderer meshRenderer;
    private GameObject go;
    private AlgoState currentState;

    //Constructor
    public Node(Vector3 _worldPosition, bool _isWalkable)
    {

        worldPosition = _worldPosition;
        gridX = Utilities.GetKeyX(worldPosition);
        gridZ = Utilities.GetKeyZ(worldPosition);
        key = Utilities.CreateKey(worldPosition);
        isWalkable = _isWalkable;
        ResetMaterial();
        if (go != null) go.transform.position = worldPosition;
    }
    //Getters & Setters
    public bool GetIsWalkable()
    {
        return isWalkable;
    }
    public void SetIsWalkable(bool _isWalkable)
    {
        isWalkable = _isWalkable;
        if (isWalkable)
        {
            ResetMaterial();
        }
        else
        {
            NotWalkableNode();
        }
    }
    public Vector3 GetPosition()
    {
        return worldPosition;
    }
    public string GetKey()
    {
        return key;
    }
    public int GetX()
    {
        return gridX;
    }
    public int GetZ()
    {
        return gridZ;
    }
    //Neighbor/Edge functions
    public void AddNeighbor(Edge edge)
    {
        if (neighbors == null) neighbors = new List<Edge>();
        neighbors.Add(edge);
    }
    public List<Edge> GetNeighbors()
    {
        return neighbors;
    }
    public bool HasNeighbor(Node node)
    {
        foreach (Edge edge in neighbors)
        {
            if (edge.GetDestination() == node) return true;
        }
        return false;
    }
    //Reset node for use on the same graph
    //Just resets iswalkable and material
    //Allows the graph to reupdate
    public void ResetSameNode()
    {
        isWalkable = true;
        ResetMaterial();
        foreach (var edge in neighbors)
        {
            edge.ResetSameEdge();
        }
    }
    //Resets node entirely
    //Causes node and edges to go to garbage collection
    public void ResetNode()
    {
        foreach (var edge in neighbors)
        {
            edge.ResetEdge();
        }
        if(go != null)go.SetActive(false);
        go = null;
        meshRenderer = null;
        neighbors = new List<Edge>();
    }

    //Graphical Functions
    public void ResetMaterial()
    {
        //Debug.Log("ResetMaterial");
        UpdateGraphical(AlgoState.unvisited);
    }
    public void VisitNode()
    {
        //Debug.Log("VisitNode");
        UpdateGraphical(AlgoState.visited);
    }
    public void TraverseNode()
    {
        //Debug.Log("TraverseNode");
        UpdateGraphical(AlgoState.traversed);
    }
    public void NotWalkableNode()
    {
        //Debug.Log("NotWalkableNode");
        UpdateGraphical(AlgoState.unwalkable);
    }
    public void UpdateGraphical(AlgoState state)
    {
        currentState = state;
        if (currentState == AlgoState.unvisited && (int)SettingsManager.Instance.graphicalType >= 3) ActivateGrahpicalGO();
        else if ((currentState == AlgoState.visited || currentState == AlgoState.unwalkable) && (int)SettingsManager.Instance.graphicalType >= 2) ActivateGrahpicalGO();
        else if (currentState == AlgoState.traversed && (int)SettingsManager.Instance.graphicalType >= 1) ActivateGrahpicalGO();
        else DeActivateGrahpicalGO();
    }
    public void ActivateGrahpicalGO()
    {
        if (go == null)
        {
            go = PoolManager.Instance.GetNode();
            meshRenderer = go.GetComponent<Renderer>();
        }
        go.transform.position = worldPosition;

        if (GetIsWalkable()) {
            if (currentState == AlgoState.unvisited)
            {
                meshRenderer.material = SettingsManager.Instance.nodeDefaultMaterial;
            }
            else if (currentState == AlgoState.visited)
            {
                meshRenderer.material = SettingsManager.Instance.nodeVisitedMaterial;
            }
            else if (currentState == AlgoState.traversed)
            {
                meshRenderer.material = SettingsManager.Instance.nodeTraversalMaterial;
            }
        }
        else if (currentState == AlgoState.unwalkable)
        {
            meshRenderer.material = SettingsManager.Instance.nodeNotWalkableMaterial;
        }

    }
    public void DeActivateGrahpicalGO()
    {
        if(go != null) go.SetActive(false);
    }






    public override string ToString()
    {
        string print = $"Node: {GetKey()}";
        if(neighbors != null){
            print += " : Neighbors = ";
            for (int i = 0; i < neighbors.Count; i++)
            {
                print += neighbors[i].GetDestination().GetKey();
            }
        
        }
        return print;
    }





}
