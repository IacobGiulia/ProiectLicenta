using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors = new List<Node>();

    public void AddNeighbor(Node node)
    {
        if (!neighbors.Contains(node))
            neighbors.Add(node);

        if (!node.neighbors.Contains(this))
            node.neighbors.Add(this);
    }
}