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
    public static IEnumerator BFS_OnGraph(Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        //Depending on graph type run BFS with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return BFS_AdjList(graph as AdjList, source, destination, callback);
            yield break;
        }
        else if (graph is EdgeListGraph edgeList)
        {
            yield return BFS_EdgeList(graph as EdgeListGraph, source, destination, callback);
            yield break;
        }
        else if (graph is AdjMatrix adjMatrix)
        {
            yield return BFS_AdjMatrix(graph as AdjMatrix, source, destination, callback);
            yield break;
        }
        yield return null;
    }

    //BFS on a adjMatrix
    private static IEnumerator BFS_AdjMatrix(AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //BFS on a EdgeList
    private static IEnumerator BFS_EdgeList(EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //BFS on a AdjList
    private static IEnumerator BFS_AdjList(AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
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
                yield return ReconstructPath(parentMap, destination, callback);
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
    ////Reconstruct path to the start point
    ////And visualize it
    //private static IEnumerator ReconstructPath(Dictionary<Node, (Node toNode, Edge withEdge)> parentMap, Node destination, Action<List<Node>> callback)
    //{
    //    List<Node> path = new List<Node>();
    //    Node currentNode = destination;
    //    Edge currentEdge = null;


    //    while (currentNode != null)
    //    {
    //        path.Add(currentNode);
    //        if (currentEdge != null) currentEdge.TraverseEdge();
    //        if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

    //        currentNode.TraverseNode();
    //        if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());


    //        if (parentMap.TryGetValue(currentNode, out (Node parentNode, Edge parentEdge) parentInfo))
    //        {
    //            currentNode = parentInfo.parentNode;
    //            currentEdge = parentInfo.parentEdge;
    //        }
    //        else
    //        {
    //            break; 
    //        }
    //    }


    //    path.Reverse();
    //    callback(path);
    //}
}