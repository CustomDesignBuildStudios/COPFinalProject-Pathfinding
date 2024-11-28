using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


public class AdjList : Graph
{
    private Dictionary<string, Node> nodes;
    public override int GetSize()
    {
        return nodes.Count;
    }
    public override void ResetGraph()
    {
        foreach(var node in nodes)
        {
            node.Value.ResetNode();
        }
        nodes = new Dictionary<string, Node>();
    }
    public override void ResetSameGraph()
    {
        foreach (var node in nodes)
        {
            node.Value.ResetSameNode();
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