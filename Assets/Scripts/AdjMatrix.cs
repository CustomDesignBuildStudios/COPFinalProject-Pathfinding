using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AdjMatrix : Graph
{
    private Edge[,] edgeMatrix;
    private List<Node> nodes;
    private int size;

    // Constructor
    public AdjMatrix(int _size)
    {
        size = _size;
        edgeMatrix = new Edge[size, size];
    }

    // Get Graph Size
    public override int GetSize()
    {
        return nodes.Count;
    }
    // Get all nodes in edge list
    public override Dictionary<string, Node> GetNodes()
    {
        return null;
    }

    public override void ResetGraph()
    {
        
    }
    public override void ResetSameGraph()
    {
 
    }


    // Add an edge between two nodes with a weight
    public bool AddEdge(Node source, Node destination, int weight = 1)
    {
        if (source.HasNeighbor(destination))
        {
            return false;
        }
        Edge edge = new Edge(source, destination, weight);
        edgeMatrix[1,1] = edge;

        return true;

    }

    public bool HasEdge(Node source, Node destination)
    {

        if (source.HasNeighbor(destination))
        {
            return false;
        }

        return true;
    }



}