using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PoolManager manages a pool for nodes, edges, and agents
/// </summary>
public class PoolManager : MonoBehaviour
{
    //All variables
    private List<GameObject> edges;
    private List<GameObject> nodes;
    public List<NPCAgent> agents;

    public int maxNodesAllowed = 10000;
    public int maxEdgesAllowed = 10000;

    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject linePrefab;


    //Singleton design pattern
    public static PoolManager Instance { get; private set; }
    //Init function
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        agents = new List<NPCAgent>();
        edges = new List<GameObject>();
        nodes = new List<GameObject>();

        //StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        List<GameObject> newEdges = new List<GameObject>();
        List<GameObject> newNodes = new List<GameObject>();

        for (int i = 0; i < 50000; i++)
        {
            GameObject newNodeGO = Instantiate(nodePrefab);
            newNodeGO.SetActive(false);
            nodes.Add(newNodeGO);

            GameObject newEdgeGO = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            newEdgeGO.SetActive(false);
            edges.Add(newEdgeGO);

            yield return new WaitForSeconds(0.001f);
        }


    }



    //Return agent from pool or create new one if all are being used
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
        NPCAgent npc = null;
        for (int i = 0; i < 10; i ++)
        {
            GameObject newNPCGO = Instantiate(agentPrefab, GameManager.Instance.GetRandomNodeOnGraph(false).GetPosition(), Quaternion.identity);
            npc = newNPCGO.GetComponent<NPCAgent>();
            agents.Add(npc);
            newNPCGO.SetActive(false);

        }

        npc.gameObject.SetActive(true);
        return npc;
    }

    //Return node from pool or create new one if all are being used
    public GameObject GetNode()
    {
        foreach (var node in nodes)
        {
            if (!node.activeSelf)
            {
                node.SetActive(true);
                return node; 
            }
        }

        GameObject newNodeGO = Instantiate(nodePrefab);
        nodes.Add(newNodeGO);
        return newNodeGO;
    }
    //Return edge from pool or create new one if all are being used
    public GameObject GetEdge()
    {
        foreach (var edge in edges)
        {
            if (!edge.activeSelf)
            {
                edge.SetActive(true);
                return edge;  
            }
        }
        GameObject newEdgeGO = Instantiate(linePrefab,Vector3.zero,Quaternion.identity);
        edges.Add(newEdgeGO);
        return newEdgeGO;
    }

}
