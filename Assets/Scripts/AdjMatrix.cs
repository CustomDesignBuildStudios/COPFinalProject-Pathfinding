using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

/// <summary>
/// Adj Matrix
/// TODO NOT FINISHED
/// </summary>
public class AdjMatrix : Graph
{
    private Edge[,] edgeMatrix;
    private Dictionary<string, Node> nodes;
    private int size;

    public AdjMatrix(int _size)
    {
        size = _size;
        edgeMatrix = new Edge[size, size];
    }
    public override int GetSize()
    {
        return nodes.Count;
    }
    public override void ResetGraph()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                edgeMatrix[i, j].ResetEdge();
            }
        }

        edgeMatrix = new Edge[size, size];
        nodes = new Dictionary<string, Node>();
    }
    public override void ResetSameGraph()
    {
        for (int i = 0; i < size; i++) 
        {
            for (int j = 0; j < size; j++) 
            {
                edgeMatrix[i, j].ResetSameEdge();
            }
        }
    }
    public Node AddNode(Vector3 position)
    {
        if (nodes == null) nodes = new Dictionary<string, Node>();


        Node node = new Node(position, true);
        nodes.Add(node.GetKey(), node);

        return node;
    }
    public bool AddEdge(Node source, Node destination, float weight)
    {
        if (source.HasNeighbor(destination))
        {
            return false;
        }
        Edge edge = new Edge(source, destination, weight);
        source.AddNeighbor(edge);

        return true;
    }
    public override Dictionary<string, Node> GetNodes()
    {
        return nodes;
    }

}