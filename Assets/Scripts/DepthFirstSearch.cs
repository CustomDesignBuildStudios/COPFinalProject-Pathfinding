using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;
/// <summary>
/// Handles DFS for all data structures
/// </summary>
public class DepthFirstSearch : MainAlgorithm
{
    //Main function to call
    public static IEnumerator DFS_OnGraph(RunReport report, Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        //Depending on graph type run DFS with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return DFS_AdjList( report, graph as AdjList, source, destination, callback);
            yield break;
        }
        else if (graph is EdgeListGraph edgeList)
        {
            yield return DFS_EdgeList( report, graph as EdgeListGraph, source, destination, callback);
            yield break;
        }
        else if (graph is AdjMatrix adjMatrix)
        {
            yield return DFS_AdjMatrix( report, graph as AdjMatrix, source, destination, callback);
            yield break;
        }
        yield return null;
    }

    //DFS on a adjMatrix
    private static IEnumerator DFS_AdjMatrix(RunReport report, AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //DFS on a EdgeList
    private static IEnumerator DFS_EdgeList(RunReport report, EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //DFS on a AdjList
    private static IEnumerator DFS_AdjList(RunReport report, AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
        float startTime = Time.realtimeSinceStartup;

        //Setup
        Stack<Node> stack = new Stack<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, (Node toNode, Edge withEdge)> parentMap = new Dictionary<Node, (Node toNode, Edge withEdge)>();
        //Add source node
        stack.Push(source);
        visited.Add(source);
        source.VisitNode();

        while (stack.Count > 0)
        {
            Node current = stack.Pop();

            //chect if top of stack is destination
            if (current == destination)
            {
                float endTime = Time.realtimeSinceStartup;
                report.timeToRun = endTime - startTime;
                ReportsManager.Instance.AddReport(report);
                yield return ReconstructPath(parentMap, destination, callback);
                yield break;
            }

            //Get all neighbors and add to stack
            List<Edge> neighbors = current.GetNeighbors();
            foreach (var edge in neighbors)
            {
                Node neighbor = edge.GetDestination();

                //Make sure neighbors are not already visited
                if (!visited.Contains(neighbor) && neighbor.GetIsWalkable())
                {
                    stack.Push(neighbor);
                    visited.Add(neighbor);
                    if(SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    edge.VisitEdge();
                    if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    neighbor.VisitNode();

                    parentMap[neighbor] = (current, edge);
                }
            }
        }
    }
}