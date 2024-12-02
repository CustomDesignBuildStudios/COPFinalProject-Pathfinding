using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Creates adjlists 
/// By random, grid, or terrain
/// </summary>
public class AdjListCreator
{

    public static Graph CreateGraph()
    {
        AdjList graph = new AdjList();

        //Random
        if (SettingsManager.Instance.graphTypes == GraphTypes.Random)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(SettingsManager.Instance.GetSize())); 

            //Creates all nodes in the grid
            for (int i = 0; i < SettingsManager.Instance.GetSize(); i++)
            {
                int x = (i % columns) * SettingsManager.Instance.gridSize; // Column
                int z = (i / columns) * SettingsManager.Instance.gridSize;
                Node node = graph.AddNode(new Vector3((int)x, 0, (int)z));
            }

   
            Dictionary<string, Node> nodes = graph.GetNodes();
            //For each node randomly create edges
            foreach (var (key, node) in nodes)
            {
                //no disconnected nodes
                int edges = (int)UnityEngine.Random.Range(1, SettingsManager.Instance.density);
                while (edges > 0)
                {
                    //Connec to random node
                    System.Random random = new System.Random();
                    int index = random.Next(nodes.Count); 
                    var randomNode = nodes.ElementAt(index); 

                    if (randomNode.Value != node)
                    {
                        bool success = graph.AddEdge(node, randomNode.Value, UnityEngine.Random.Range(SettingsManager.Instance.minWeight, SettingsManager.Instance.maxWeight));
                        if (success)
                        {
                            edges--;
                        }
                    }
                }
            }
        }
        //Grid creation
        else if (SettingsManager.Instance.graphTypes == GraphTypes.EightGrid || SettingsManager.Instance.graphTypes == GraphTypes.FourGrid)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(SettingsManager.Instance.GetSize())); 
            //create all the nodes in the grid
            for (int i = 0; i < SettingsManager.Instance.GetSize(); i++)
            {
                int x = (i % columns) * SettingsManager.Instance.gridSize; 
                int z = (i / columns) * SettingsManager.Instance.gridSize;
                graph.AddNode(new Vector3((int)x, 0, (int)z));
            }
     

            Dictionary<string, Node> nodes = graph.GetNodes();

            //connect to the 8 directions, left, right, top, bottom, and 45s
            foreach (var (key, node) in nodes)
            {

                for (int x = -1; x < 2; x++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (x == 0 && z == 0)
                        {

                        }
                        //Only left right, top bottom
                        else if (SettingsManager.Instance.graphTypes == GraphTypes.FourGrid && (z == 0 || x == 0))
                        {
                            string newNodeKey = Utilities.CreateKey(node.GetX() + (x * SettingsManager.Instance.gridSize), node.GetZ() + (z * SettingsManager.Instance.gridSize));
                            if (nodes.ContainsKey(newNodeKey))
                            {
                                //Debug.Log("ssdf");
                                graph.AddEdge(node, nodes[newNodeKey], SettingsManager.Instance.minWeight);
                            }
                        }
                        //All 8 directions
                        else if (SettingsManager.Instance.graphTypes == GraphTypes.EightGrid)
                        {
                            string newNodeKey = Utilities.CreateKey(node.GetX() + (x * SettingsManager.Instance.gridSize), node.GetZ() + (z * SettingsManager.Instance.gridSize));
                            if (nodes.ContainsKey(newNodeKey))
                            {
                                graph.AddEdge(node, nodes[newNodeKey], SettingsManager.Instance.minWeight);
                            }
                        }
                    }
                }
            }
        }
        //Terrain graph creation
        else if ((int)(SettingsManager.Instance.graphTypes) > 2)
        {
            TerrainData terrainData = SettingsManager.Instance.GetTerrain().terrainData;
            int width = terrainData.heightmapResolution;
            int height = terrainData.heightmapResolution;

            float[,] heights = terrainData.GetHeights(0, 0, width, height);
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            //Divide terrain based on size
            int resolution = Mathf.CeilToInt(Mathf.Sqrt(SettingsManager.Instance.GetSize()));

            float stepX = terrainWidth / (resolution - 1);
            float stepZ = terrainLength / (resolution - 1);

            //Create grid of nodes on the terrain
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float worldX = x * stepX;
                    float worldZ = z * stepZ;

                    int heightX = Mathf.RoundToInt(x * (width - 1) / (resolution - 1));
                    int heightZ = Mathf.RoundToInt(z * (height - 1) / (resolution - 1));
                    float elevation = heights[heightX, heightZ] * terrainData.size.y;

                    Vector3 position = new Vector3(worldZ, elevation, worldX);
                    Node newNode = graph.AddNode(position);

                }
            }
            //Create edges for each node
            foreach (var nodePair in graph.GetNodes())
            {
                Node currentNode = nodePair.Value;
                //Create edges for the 8 directions from node
                for (int x = -1; x < 2; x++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (x == 0 && z == 0)
                        {

                        }
                        else
                        {
                            string newNodeKey = Utilities.CreateKey(new Vector3(currentNode.GetPosition().x + (x * stepX), 0, currentNode.GetPosition().z + (z * stepZ)));
                            if (graph.GetNodes().ContainsKey(newNodeKey))
                            {
                                //Based on slope of the edges 
                                //Set the weight
                                //THe steeper the slope the bigger the weight to travel

                                Vector3 newNodePosition = graph.GetNodes()[newNodeKey].GetPosition();
                                float elevationDiff = currentNode.GetPosition().y - newNodePosition.y;

                                float horizontalDistance = Mathf.Sqrt(
                                    Mathf.Pow(currentNode.GetPosition().x - newNodePosition.x, 2) +
                                    Mathf.Pow(currentNode.GetPosition().z - newNodePosition.z, 2)
                                );

                                float slopePercentage = (horizontalDistance > 0 ? Mathf.Abs(elevationDiff / horizontalDistance) * 100 : 100f) / 100f;
                                //Debug.Log(slopePercentage);
                                float weight = slopePercentage * (SettingsManager.Instance.maxWeight - SettingsManager.Instance.minWeight);


                                //Debug.Log(weight);
                                graph.AddEdge(currentNode, graph.GetNodes()[newNodeKey], weight);
                            }
                        }
                    }
                }
            }
        }
        return graph;
    }



}