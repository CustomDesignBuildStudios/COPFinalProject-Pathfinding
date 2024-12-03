using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Bellford for all data structures
/// TODO fix the speed the algo runs at
/// Optimize
/// </summary>
public class BellfordAlgorithm : MainAlgorithm
{

    //Main function to call
    public static IEnumerator Bellford_OnGraph(RunReport report, Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {

        //Depending on graph type run Bellford with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return Bellford_AdjList(report, graph as AdjList, source, destination, callback);
            yield break;
        }
        else if (graph is EdgeListGraph edgeList)
        {
            yield return Bellford_EdgeList(report, graph as EdgeListGraph, source, destination, callback);
            yield break;
        }
        else if (graph is AdjMatrix adjMatrix)
        {
            yield return Bellford_AdjMatrix(report, graph as AdjMatrix, source, destination, callback);
            yield break;
        }
        yield return null;
    }

    //Bellford on a adjMatrix
    private static IEnumerator Bellford_AdjMatrix(RunReport report, AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //Bellford on a EdgeList
    private static IEnumerator Bellford_EdgeList(RunReport report, EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //Bellford on a AdjList
    private static IEnumerator Bellford_AdjList(RunReport report, AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
        float startTime = Time.realtimeSinceStartup;


        Dictionary<string, float> distancesToNodes = new Dictionary<string, float>();
        Dictionary<Node, (Node toNode, Edge withEdge)> parentMap = new Dictionary<Node, (Node toNode, Edge withEdge)>();

        List<Edge> allEdges = new List<Edge>();
        foreach (var node in graph.GetNodes())
        {
            if (node.Value.GetIsWalkable() == false) continue;
            distancesToNodes[node.Key] = float.MaxValue;
            allEdges.AddRange(node.Value.GetNeighbors());
        }
        distancesToNodes[source.GetKey()] = 0;
        int visited = 0;

        //Relax edges repeatedly

        foreach (var node in graph.GetNodes())
        {
            if (node.Value.GetIsWalkable() == false) continue;
            bool anyUpdate = false;
            foreach (var edge in allEdges)
            {

                if (!edge.GetSource().GetIsWalkable() || !edge.GetDestination().GetIsWalkable())continue;
                visited++;
                //Relax edge
                string sourceKey = edge.GetSource().GetKey();
                string destKey = edge.GetDestination().GetKey();

                if (distancesToNodes[sourceKey] != float.MaxValue &&
                    distancesToNodes[sourceKey] + edge.GetWeight() < distancesToNodes[destKey])
                {
                    distancesToNodes[destKey] = distancesToNodes[sourceKey] + edge.GetWeight();

                    //Track the path
                    parentMap[edge.GetDestination()] = (edge.GetSource(), edge);
                    anyUpdate = true;
                }

                //Visualization steps
                edge.GetSource().VisitNode();
                if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

                edge.VisitEdge();
                if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

                edge.GetDestination().VisitNode();
                if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
            }
            
            if (!anyUpdate) break;
        }


        float endTime = Time.realtimeSinceStartup;
        report.UpdateReport(endTime - startTime, visited);


        yield return ReconstructPath(report, parentMap, destination, callback);

        Debug.Log(report.totalPathLength);
        Debug.Log(report.avgPathLength);
        ReportsManager.Instance.AddReport(report);
    }

}