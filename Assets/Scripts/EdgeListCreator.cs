using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeListCreator
{

    public static Graph CreateGraph()
    {
        EdgeListGraph graph = new EdgeListGraph();
        if (SettingsManager.Instance.graphTypes == GraphTypes.Random)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(SettingsManager.Instance.GetSize())); 

            for (int i = 0; i < SettingsManager.Instance.GetSize(); i++)
            {
                int x = (i % columns) * SettingsManager.Instance.gridSize; 
                int z = (i / columns) * SettingsManager.Instance.gridSize;
                Node node = graph.AddNode(new Vector3(x, 0, z));
            }
            Dictionary<string,Node> nodes = graph.GetNodes();
            foreach (var (key, node) in nodes)
            {
                int edges = (int)((UnityEngine.Random.Range(0.0f, SettingsManager.Instance.density) / 100.0f) * (nodes.Count));

                while (edges > 0)
                {
                    System.Random random = new System.Random();
                    int index = random.Next(nodes.Count); 
                    var randomNode = nodes.ElementAt(index); 

                    if(randomNode.Value != node)
                    {
                        graph.AddEdge(node, randomNode.Value, UnityEngine.Random.Range(SettingsManager.Instance.minWeight, SettingsManager.Instance.maxWeight));
                        edges--;
                    }
                }
            } 
        }
        return graph;
    }
}
