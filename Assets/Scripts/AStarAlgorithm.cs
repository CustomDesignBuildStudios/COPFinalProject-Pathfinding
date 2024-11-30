using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;



//Helper class to add gcost and fcost to node
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
//Helper class that compares two astar nodes
public class AStarNodeComparer : IComparer<AStarNode>
{
    //Compares a to b. If less than return -1, equal = 0, morethan = 1
    public int Compare(AStarNode a, AStarNode b)
    {
        int fCostComparison = a.fCost.CompareTo(b.fCost);
        if (fCostComparison != 0) return fCostComparison;

        //If tie
        //compare gcost
        int gCostComparison = a.gCost.CompareTo(b.gCost);
        //If tie
        //compare key
        return gCostComparison != 0 ? gCostComparison : string.Compare(a.node.GetKey(), b.node.GetKey(), StringComparison.Ordinal);
    }
}



/// <summary>
/// Handles AStar for all data structures
/// </summary>
public class AStarAlgorithm : MainAlgorithm
{
    //Main function to call
    public static IEnumerator AStar_OnGraph(Graph graph, Node source, Node destination, System.Action<List<Node>> callback)
    {
        //Depending on graph type run AStar with minor differences
        if (graph is AdjList adjListGraph)
        {
            yield return AStar_AdjList(graph as AdjList, source, destination, callback);
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

    //AStar on a adjMatrix
    private static IEnumerator BFS_AdjMatrix(AdjMatrix graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }
    //AStar on a EdgeList
    private static IEnumerator BFS_EdgeList(EdgeListGraph graph, Node source, Node destination, Action<List<Node>> callback)
    {
        yield return null;
        yield break;
    }

    //AStar on a AdjList
    private static IEnumerator AStar_AdjList(AdjList graph, Node source, Node destination, System.Action<List<Node>> callback)
    {
        //Setup queue, costs, visited
        var priorityQueue = new SortedSet<AStarNode>(new AStarNodeComparer());
        var gCosts = new Dictionary<Node, float>();
        var hCosts = new Dictionary<Node, float>();
        var parentMap = new Dictionary<Node, (Node parentNode, Edge parentEdge)>();
        var visited = new HashSet<Node>();
        Dictionary<string, Node> nodes = graph.GetNodes();

        float startTime = Time.realtimeSinceStartup;

        //Setup costs to maxValues
        foreach (var node in nodes)
        {
            gCosts[node.Value] = float.MaxValue;
            hCosts[node.Value] = Heuristic(node.Value, destination); 
        }
        //Add source 
        gCosts[source] = 0;
        priorityQueue.Add(new AStarNode(source, hCosts[source], 0)); // Add source node with initial fCost

        //float endTime = Time.realtimeSinceStartup;
        //Debug.Log($"Time taken: {endTime - startTime} seconds");

               
        while (priorityQueue.Count > 0)
        {
            //Get min of priority queue
            var currentNode = priorityQueue.Min;
            priorityQueue.Remove(currentNode);

            //if destination reach reconstruct path
            if (currentNode.node == destination)
            {
                yield return ReconstructPath(parentMap, destination, callback);
                yield break;
            }

            //check if already visited 
            if (visited.Contains(currentNode.node) || currentNode.node.GetIsWalkable() == false) continue;

            //add if not visited
            visited.Add(currentNode.node);
            currentNode.node.VisitNode();

            //check neighbors
            foreach (var edge in currentNode.node.GetNeighbors())
            {
                Node neighbor = edge.GetDestination();
                if (visited.Contains(neighbor) || neighbor.GetIsWalkable() == false){

                }
                else
                {
                    float newGCost = currentNode.gCost + edge.GetWeight();

                    //If new path if better, update it
                    if (newGCost < gCosts[neighbor])
                    {
                        gCosts[neighbor] = newGCost;
                        float fCost = newGCost + hCosts[neighbor];
                        parentMap[neighbor] = (currentNode.node, edge);

                        //Add neighbor to queue
                        priorityQueue.Add(new AStarNode(neighbor, hCosts[neighbor], newGCost));

                        edge.VisitEdge();
                        if (SettingsManager.Instance.visualize) yield return new WaitForSeconds(SettingsManager.Instance.GetVisualizeSpeed());
                    }
                }

            }
        }
        // If the destination is unreachable
        callback(null);
    }

    //the heuristic is the distance between nodes in meters
    private static float Heuristic(Node node, Node destination)
    {
        return Vector3.Distance(node.GetPosition(), destination.GetPosition());
    }
    //Reconstruct path to the start point
    //And visualize it
    //private static IEnumerator ReconstructPath(Dictionary<Node, (Node parentNode, Edge parentEdge)> parentMap, Node destination, System.Action<List<Node>> callback)
    //{
    //    float startTime = Time.realtimeSinceStartup;
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


    //    float endTime = Time.realtimeSinceStartup;
    //    Debug.Log($"Time taken: {endTime - startTime} seconds");
    //}
}

