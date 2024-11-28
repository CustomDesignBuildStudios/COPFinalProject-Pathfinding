using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BreadthFirstSearch : MonoBehaviour
{
    public static IEnumerator BFS_OnGraph(Graph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        if (graph is AdjList adjListGraph)
        {
            yield return BFS_AdjList(graph as AdjList, source, destination, callback);
            yield break;
        }
        yield return null;
    }



    private static IEnumerator BFS_AdjList(AdjList graph, Node source, Node destination, Action<List<Node>> callback)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, (Node toNode, Edge withEdge)> parentMap = new Dictionary<Node, (Node toNode, Edge withEdge)>();

        queue.Enqueue(source);
        visited.Add(source);
        source.VisitNode();

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == destination)
            {
                yield return ReconstructPath(parentMap, destination, callback);
                yield break;
            }

            List<Edge> neighbors = current.GetNeighbors();
            foreach (var edge in neighbors)
            {
                Node neighbor = edge.GetDestination();

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

    private static IEnumerator ReconstructPath(Dictionary<Node, (Node toNode, Edge withEdge)> parentMap, Node destination, Action<List<Node>> callback)
    {
        List<Node> path = new List<Node>();
        Node currentNode = destination;
        Edge currentEdge = null;


        while (currentNode != null)
        {
            path.Add(currentNode);
            if (currentEdge != null) currentEdge.TraverseEdge();
            if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());

            currentNode.TraverseNode();
            if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());


            if (parentMap.TryGetValue(currentNode, out (Node parentNode, Edge parentEdge) parentInfo))
            {
                currentNode = parentInfo.parentNode;
                currentEdge = parentInfo.parentEdge;
            }
            else
            {
                break; 
            }
        }


        path.Reverse();
        callback(path);
    }
}