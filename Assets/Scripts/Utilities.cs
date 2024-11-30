using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utilities
{
    public static float GetPercentage(float value, float min, float max)
    {
        if (max == min)
        {
            return 1;
        }
        return ((value - min) / (max - min));
    }

    public static string CreateKey(Vector3 pos)
    {
        return ((int)pos.x) + " " + ((int)pos.z);
    }
    public static string CreateKey(int x, int z)
    {
        return x + " " + z;
    }

    public static int GetKeyX(Vector3 pos)
    {
        return ((int)pos.x);
    }
    public static int GetKeyZ(Vector3 pos)
    {
        return ((int)pos.z);
    }

    public static Node GetClosestNode(Graph graph, Vector3 targetPosition)
    {
        if (graph == null) return null;
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var nodePair in graph.GetNodes())
        {
            Node currentNode = nodePair.Value;
            float distance = Vector3.Distance(currentNode.GetPosition(), targetPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = currentNode;
            }
        }

        return closestNode;
    }











}
