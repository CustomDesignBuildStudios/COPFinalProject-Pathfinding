using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;


public enum AlgoState
{
    unvisited, visited, traversed, unwalkable
}
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
    ////////////////////////////////////////GETTERS SETTERS//////////////////////////////////////////////////
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
    public void AddNeighbor(Edge edge)
    {
        if (neighbors == null) neighbors = new List<Edge>();
        neighbors.Add(edge);
    }

    public List<Edge> GetNeighbors()
    {
        return neighbors;
    }
    ////////////////////////////////////////RESET//////////////////////////////////////////////////

    public void ResetSameNode()
    {
        isWalkable = true;
        ResetMaterial();
        foreach (var edge in neighbors)
        {
            edge.ResetSameEdge();
        }
    }
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

    ////////////////////////////////////////GRAPHICAL//////////////////////////////////////////////////

    public void ResetMaterial()
    {
        UpdateGraphical(AlgoState.unvisited);
    }
    public void VisitNode()
    {
        UpdateGraphical(AlgoState.visited);
    }
    public void TraverseNode()
    {
        UpdateGraphical(AlgoState.traversed);
    }
    public void NotWalkableNode()
    {
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
        Debug.Log("A");
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





    public bool HasNeighbor(Node node)
    {
        foreach (Edge edge in neighbors)
        {
            if(edge.GetDestination() == node) return true;
        }
        return false;
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
