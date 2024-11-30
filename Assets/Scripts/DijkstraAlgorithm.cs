using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Search;
/// <summary>
/// Handles Dijkstra for all data structures
/// </summary>
public class DijkstraAlgorithm : MainAlgorithm
{

    //Main function to call
    public static IEnumerator Dijkstra_OnGraph(Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        //Depending on graph type run Dijkstra with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return Dijkstra_AdjList(graph as AdjList, source, destination, callback);
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

    //Dijkstra on a adjMatrix
    private static IEnumerator BFS_AdjMatrix(AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //Dijkstra on a EdgeList
    private static IEnumerator BFS_EdgeList(EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //Dijkstra on a AdjList
    private static IEnumerator Dijkstra_AdjList(AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
        //Setup
        var priorityQueue = new SortedSet<(float distance, Node node)>(
            Comparer<(float, Node)>.Create((a, b) =>
            {
                int distanceComparison = a.Item1.CompareTo(b.Item1); // Compare distances
                if (distanceComparison != 0) return distanceComparison;

                // Compare nodes by their names to break ties
                return string.Compare(a.Item2.GetKey(), b.Item2.GetKey(), StringComparison.Ordinal);
            })
        );

        var distances = new Dictionary<Node, float>();
        var parentMap = new Dictionary<Node, (Node toNode, Edge withEdge)>();
        var visited = new HashSet<Node>();

        //Setup all distances to inf
        foreach (var node in graph.GetNodes())
        {
            distances[node.Value] = float.MaxValue;
        }
        //set distance to source to 0
        distances[source] = 0;
        //add source to queue
        priorityQueue.Add((0, source));

        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentNode) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            //If the destination node is reached, reconstruct the path
            if (currentNode == destination)
            {
                yield return ReconstructPath(parentMap, destination, callback);
                yield break;
            }

            //Check if node was visited and is walkable
            if (visited.Contains(currentNode) || currentNode.GetIsWalkable() == false) continue;

            //Add node to visited
            visited.Add(currentNode);
            currentNode.VisitNode();

            //Add neighbors to queue
            foreach (var edge in currentNode.GetNeighbors())
            {
                Node neighbor = edge.GetDestination();
                float newDistance = currentDistance + edge.GetWeight();

                // Update the shortest distance if a shorter path is found
                if (newDistance < distances[neighbor])
                {
                    // Remove the neighbor if it already exists in the queue
                    priorityQueue.Remove((distances[neighbor], neighbor));

                    distances[neighbor] = newDistance;

                    parentMap[neighbor] = (currentNode, edge);
                    priorityQueue.Add((newDistance, neighbor));


                    edge.VisitEdge();
                    if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

                }
            }
        }

        // If the destination is unreachable
        callback(null);
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