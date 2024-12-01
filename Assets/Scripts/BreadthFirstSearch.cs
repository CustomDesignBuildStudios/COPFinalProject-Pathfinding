using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// Handles BFS for all data structures
/// </summary>
public class BreadthFirstSearch : MainAlgorithm
{
    //Main function to call
    public static IEnumerator BFS_OnGraph(RunReport report, Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        //Depending on graph type run BFS with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return BFS_AdjList(report,graph as AdjList, source, destination, callback);
            yield break;
        }
        else if (graph is EdgeListGraph edgeList)
        {
            yield return BFS_EdgeList(report, graph as EdgeListGraph, source, destination, callback);
            yield break;
        }
        else if (graph is AdjMatrix adjMatrix)
        {            yield return BFS_AdjMatrix(report, graph as AdjMatrix, source, destination, callback);
            yield break;
        }
        yield return null;
    }

    //BFS on a adjMatrix
    private static IEnumerator BFS_AdjMatrix(RunReport report, AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //BFS on a EdgeList
    private static IEnumerator BFS_EdgeList(RunReport report, EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //BFS on a AdjList
    private static IEnumerator BFS_AdjList(RunReport report, AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
        float startTime = Time.realtimeSinceStartup;
        //Setup
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, (Node toNode, Edge withEdge)> parentMap = new Dictionary<Node, (Node toNode, Edge withEdge)>();

        //Add source node
        queue.Enqueue(source);
        visited.Add(source);
        source.VisitNode();


        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            //chect if front of queue is destination
            if (current == destination)
            {
                float endTime = Time.realtimeSinceStartup;
                report.UpdateReport(endTime - startTime, visited.Count);
                yield return ReconstructPath(report,parentMap, destination, callback);
                ReportsManager.Instance.AddReport(report);
                yield break;
            }
            //Get all neighbors and add to queue
            List<Edge> neighbors = current.GetNeighbors();
            foreach (var edge in neighbors)
            {
                Node neighbor = edge.GetDestination();
                //Make sure neighbors are not already visited
                if (!visited.Contains(neighbor) && neighbor.GetIsWalkable())
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    edge.VisitEdge();
                    if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    neighbor.VisitNode();

                    parentMap[neighbor] = (current, edge); 
                }
            }
        }
    }

}