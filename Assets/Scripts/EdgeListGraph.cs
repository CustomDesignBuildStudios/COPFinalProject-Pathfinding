using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// Edge List - Data Structure
/// Holds a 
/// </summary>
public class EdgeListGraph: Graph
{
    private List<Edge> edges;
    private int nodeCount;
    //private Dictionary<string, Node> nodes;

    // Get Graph Size
    public override int GetSize()
    {
        return nodeCount;
    }
    // Get all nodes in edge list
    public override Dictionary<string, Node> GetNodes()
    {
        Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        foreach (var edge in edges)
        {
            nodes.Add(edge.GetSource().GetKey(), edge.GetSource());
            nodes.Add(edge.GetDestination().GetKey(), edge.GetDestination());
        }
        return nodes;
    }

    // Get all edges in edge list
    public List<Edge> GetEdges()
    {
        return edges;
    }

    public override void ResetGraph()
    {
        foreach (var edge in edges)
        {
            edge.ResetEdgeAndNode();
        }
        edges = new List<Edge>();
    }
    public override void ResetSameGraph()
    {
        foreach (var edge in edges)
        {
            edge.ResetSameEdgeAndNode();
        }
    }
    public bool HasEdge(string source, string destination)
    {
        foreach(var edge in edges)
        {
            if(edge.GetSource().GetKey() == source &&  edge.GetDestination().GetKey() == destination)
            {
                return true;
            }
        }
        return false;
    }

    public void AddEdge(Node source, Node destination, float weight)
    {
        if(edges == null) edges = new List<Edge>();


        Edge edge = new Edge(source, destination, weight);
        edges.Add(edge);
    }
}