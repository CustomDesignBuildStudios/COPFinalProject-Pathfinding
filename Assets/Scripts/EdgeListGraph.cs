using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
/// <summary>
/// Edge List - Data Structure
/// Holds a 
/// </summary>
public class EdgeListGraph: Graph
{
    private List<Edge> edges;
    private int nodeCount;
    private Dictionary<string, Node> nodes;

    // Get Graph Size
    public override int GetSize()
    {
        return nodes.Count;
    }
    // Get all nodes in edge list
    public override Dictionary<string, Node> GetNodes()
    {
        return nodes;
    }
    // Get all edges in edge list
    public List<Edge> GetEdges()
    {
        return edges;
    }





    public override void ResetGraph()
    {
        foreach (var node in nodes)
        {
            node.Value.ResetNode();
        }
        foreach (var edge in edges)
        {
            edge.ResetEdge();
        }
        nodes = new Dictionary<string, Node>();
        edges = new List<Edge>();
    }
    public override void ResetSameGraph()
    {
        foreach (var node in nodes)
        {
            node.Value.ResetSameNode();
        }
        foreach (var edge in edges)
        {
            edge.ResetSameEdge();
        }
    }


    public Node AddNode(Vector3 position)
    {
        if(nodes == null) nodes = new Dictionary<string, Node>();

        string key = position.ToString();

        if (!nodes.ContainsKey(key))
        {
            Node node = new Node(position, true);
            nodes.Add(key, node);

            return node;
        }
        return null;
    }
    public void AddEdge(Node source, Node destination, float weight)
    {
        if(edges == null) edges = new List<Edge>();

        if (!nodes.ContainsKey(source.GetKey()) || !nodes.ContainsKey(destination.GetKey()))
        {
            throw new ArgumentException("Both source and destination nodes must exist in the graph.");
        }
        Edge edge = new Edge(source, destination, weight);
        edges.Add(edge);
    }
}