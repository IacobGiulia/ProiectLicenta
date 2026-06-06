using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static List<Node> Dijkstra(Node start, Node target)
    {
        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();
        var unvisited = new List<Node>();

        Node[] allNodes = Object.FindObjectsOfType<Node>();

        foreach (Node node in allNodes)
        {
            dist[node] = float.MaxValue;
            prev[node] = null;
            unvisited.Add(node);
        }

        dist[start] = 0;

        while (unvisited.Count > 0)
        {
            Node current = null;
            float minDist = float.MaxValue;
            foreach (Node n in unvisited)
            {
                if (dist[n] < minDist)
                {
                    minDist = dist[n];
                    current = n;
                }
            }

            if (current == null)
                break;

            if (current == target)
                break;

            unvisited.Remove(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (!unvisited.Contains(neighbor)) continue;

                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;
                }
            }
        }

        var path = new List<Node>();
        Node pathNode = target;
        while (pathNode != null)
        {
            path.Insert(0, pathNode);
            pathNode = prev[pathNode];
        }

        if(path.Count == 1 && path[0] != start)
        {
            return null;
        }

        return path;
    }
}