using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        edges = new List<GameObject>();
        nodes = new List<GameObject>();
    }



    public List<GameObject> edges;
    public List<GameObject> nodes;
    public List<NPCAgent> agents;

    public int maxNodesAllowed = 10000;
    public int maxEdgesAllowed = 10000;

    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject linePrefab;


    public NPCAgent GetAgent()
    {
        foreach (var agent in agents)
        {
            if (!agent.gameObject.activeSelf)
            {
                agent.EnableAgent();
                return agent;  // Return an inactive node to reuse
            }
        }

        GameObject newNPCGO = Instantiate(agentPrefab,GameManager.Instance.GetRandomNodeOnGraph(false).GetPosition(),Quaternion.identity);
        NPCAgent npc = newNPCGO.GetComponent<NPCAgent>();   

        agents.Add(npc);
        return npc;
    }

    public GameObject GetNode()
    {
        foreach (var node in nodes)
        {
            if (!node.activeSelf)
            {
                node.SetActive(true);
                return node;  // Return an inactive node to reuse
            }
        }

        //if (nodes.Count >= maxNodesAllowed) return null;

        // If no inactive nodes are available, create a new one
        GameObject newNodeGO = Instantiate(nodePrefab);
        nodes.Add(newNodeGO);
        return newNodeGO;
    }
    public GameObject GetEdge()
    {
        foreach (var edge in edges)
        {
            if (!edge.activeSelf)
            {
                edge.SetActive(true);
                return edge;  // Return an inactive node to reuse
            }
        }

        //if (edges.Count >= maxEdgesAllowed) return null;

        // If no inactive nodes are available, create a new one
        GameObject newEdgeGO = Instantiate(linePrefab,Vector3.zero,Quaternion.identity);
        edges.Add(newEdgeGO);
        return newEdgeGO;
    }

}
