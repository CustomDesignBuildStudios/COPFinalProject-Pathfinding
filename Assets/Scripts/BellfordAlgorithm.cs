using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.RectTransform;
/// <summary>
/// Handles Bellford for all data structures
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


        foreach (var node in graph.GetNodes())
        {
            distancesToNodes[node.Key] = float.MaxValue;
        }
        distancesToNodes[source.GetKey()] = 0;


        //Relax edges repeatedly
        for (int i = 0; i < graph.GetNodes().Count - 1; i++)
        {
            foreach (var node in graph.GetNodes())
            {
                if (node.Value.GetIsWalkable() == false) continue;

                node.Value.VisitNode();
                if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());



                foreach (var edge in graph.GetNode(node.Key).GetNeighbors())
                {
                    if (edge.GetDestination().GetIsWalkable() == false) continue;

                    edge.VisitEdge();
                    if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

                    if (distancesToNodes[node.Key] != float.MaxValue &&
                        distancesToNodes[node.Key] + edge.GetWeight() < distancesToNodes[edge.GetDestination().GetKey()])
                    {
                        distancesToNodes[edge.GetDestination().GetKey()] = distancesToNodes[node.Key] + edge.GetWeight();
                        //Tracks the path
                        parentMap[edge.GetDestination()] = (node.Value, edge); 

                    }
                }
            }
        }

        //foreach (var node in graph.GetNodes())
        //{
        //    foreach (var edge in graph.GetNode(node.Key).GetNeighbors())
        //    {
        //        if (distancesToNodes[node.Key] != float.MaxValue &&
        //            distancesToNodes[node.Key] + edge.GetWeight() < distancesToNodes[edge.GetDestination().GetKey()])
        //        {
        //            callback(null);
        //            yield break;
        //        }
        //    }
        //}

        float endTime = Time.realtimeSinceStartup;
        report.UpdateReport(endTime - startTime, graph.GetNodes().Count);


        yield return ReconstructPath(report, parentMap, destination, callback);
        ReportsManager.Instance.AddReport(report);
    }

}