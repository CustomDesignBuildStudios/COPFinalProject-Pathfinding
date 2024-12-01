using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainAlgorithm : MonoBehaviour
{

    //Reconstruct path to the start point
    //And visualize it
    public static IEnumerator ReconstructPath(RunReport report,  Dictionary<Node, (Node toNode, Edge withEdge)> parentMap, Node destination, Action<List<Node>> callback)
    {
        List<Node> path = new List<Node>();
        Node currentNode = destination;
        Edge currentEdge = null;

        int pathLength = 0;
        while (currentNode != null)
        {
            path.Add(currentNode);
            pathLength += 1;


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
        report.FinishRunReport(pathLength);
        path.Reverse();
        callback(path);
    }
}
