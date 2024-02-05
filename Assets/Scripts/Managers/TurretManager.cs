using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{

    private Node[] nodes;

    private void OnEnable()
    {
        LevelManager.OnRestartGame += ResetAllTurrets;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetNodesFromScene();
    }

    private void ResetAllTurrets()
    {
        foreach (Node node in nodes)
        {
            node.ResetTurret();
        }
    }

    private void GetNodesFromScene()
    {
        nodes = FindObjectsOfType<Node>();
    }

    public Node[] GetNodes()
    {
        return nodes;
    }
}
