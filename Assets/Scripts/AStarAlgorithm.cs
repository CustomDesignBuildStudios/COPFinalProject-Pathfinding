using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    public static IEnumerator AStar_OnGraph(Graph graph, Node source, Node destination, System.Action<List<Node>> callback)
    {
        Debug.Log("A");
        if (graph is AdjList adjListGraph)
        {
            Debug.Log("B");
            yield return AStar_AdjList(graph as AdjList, source, destination, callback);
            yield break;
        }
        yield return null;
    }

    private static IEnumerator AStar_AdjList(AdjList graph, Node source, Node destination, System.Action<List<Node>> callback)
    {
        var priorityQueue = new SortedSet<AStarNode>(new AStarNodeComparer());
        Debug.Log("C");

        var gCosts = new Dictionary<Node, float>();
        var hCosts = new Dictionary<Node, float>();
        var parentMap = new Dictionary<Node, (Node parentNode, Edge parentEdge)>();
        var visited = new HashSet<Node>();

        Dictionary<string, Node> nodes = graph.GetNodes();


        float startTime = Time.realtimeSinceStartup;

        Debug.Log("SC");


        foreach (var node in nodes)
        {
            gCosts[node.Value] = float.MaxValue;
            hCosts[node.Value] = Heuristic(node.Value, destination);  // Calculate heuristic for each node
        }
        gCosts[source] = 0;
        Debug.Log("cc");

        priorityQueue.Add(new AStarNode(source, hCosts[source], 0)); // Add source node with initial fCost

        Debug.Log("D");

        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"Time taken: {endTime - startTime} seconds");

                
        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Min;
            priorityQueue.Remove(currentNode);

            // If the destination node is reached, reconstruct the path
            if (currentNode.node == destination)
            {
                endTime = Time.realtimeSinceStartup;
                Debug.Log($"Time taken: {endTime - startTime} seconds");
                yield return ReconstructPath(parentMap, destination, callback);
                yield break;
            }

            if (visited.Contains(currentNode.node) || currentNode.node.GetIsWalkable() == false) continue;

            visited.Add(currentNode.node);
            currentNode.node.VisitNode();

            foreach (var edge in currentNode.node.GetNeighbors())
            {
                Node neighbor = edge.GetDestination();
                if (visited.Contains(neighbor) || neighbor.GetIsWalkable() == false){

                }
                else
                {
                    float newGCost = currentNode.gCost + edge.GetWeight();

                    // If the new path to the neighbor is better, update it
                    if (newGCost < gCosts[neighbor])
                    {
                        gCosts[neighbor] = newGCost;
                        float fCost = newGCost + hCosts[neighbor]; // f = g + h
                        parentMap[neighbor] = (currentNode.node, edge);

                        // Add the neighbor to the priority queue with updated costs
                        priorityQueue.Add(new AStarNode(neighbor, hCosts[neighbor], newGCost));

                        edge.VisitEdge();
                        if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    }
                }

            }
        }
            Debug.Log("E");
        endTime = Time.realtimeSinceStartup;
        Debug.Log($"Time taken: {endTime - startTime} seconds");
        // If the destination is unreachable
        callback(null);
    }

    private static float Heuristic(Node node, Node destination)
    {
        // Here, we are using Euclidean distance as an example of heuristic
        // Modify this based on your needs (Manhattan, Diagonal, etc.)
        return Vector3.Distance(node.GetPosition(), destination.GetPosition());
    }

    private static IEnumerator ReconstructPath(Dictionary<Node, (Node parentNode, Edge parentEdge)> parentMap, Node destination, System.Action<List<Node>> callback)
    {
        float startTime = Time.realtimeSinceStartup;
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


        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"Time taken: {endTime - startTime} seconds");
    }
}

public class AStarNode
{
    public Node node;
    public float fCost;
    public float gCost;

    public AStarNode(Node node, float hCost, float gCost)
    {
        this.node = node;
        this.fCost = gCost + hCost;
        this.gCost = gCost;
    }
}

public class AStarNodeComparer : IComparer<AStarNode>
{
    public int Compare(AStarNode a, AStarNode b)
    {
        int fCostComparison = a.fCost.CompareTo(b.fCost);
        if (fCostComparison != 0) return fCostComparison;

        // Use g-cost to break ties if needed
        int gCostComparison = a.gCost.CompareTo(b.gCost);
        return gCostComparison != 0 ? gCostComparison : string.Compare(a.node.GetKey(), b.node.GetKey(), StringComparison.Ordinal);
    }
}