using System;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AdjMatrix
{
    private Edge[,] edgeMatrix;
    private Node[] nodes;
    private int size;

    // Constructor
    public AdjMatrix(int _size)
    {
        size = _size;
        edgeMatrix = new Edge[size, size];
    }

    // Add an edge between two nodes with a weight
    public bool AddEdge(Node source, Node destination, int weight = 1)
    {
        if (source.HasNeighbor(destination))
        {
            return false;
        }


        //Edge edge = PoolManager.Instance.GetEdge();
        //edge.Setup(source, destination, weight);
        //source.AddNeighbor(edge);

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