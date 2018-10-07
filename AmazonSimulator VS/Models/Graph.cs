using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Graph
    {
        #region Variables
        // Create dictionary vertices.
        Dictionary<string, Dictionary<string, Node>> vertices = new Dictionary<string, Dictionary<string, Node>>();
        // Create dictionary nodesSmall.
        Dictionary<string, Node> nodesSmall = new Dictionary<string, Node>();
        #endregion

        #region Methods

        /// <summary>
        /// Adding a vertex.
        /// </summary>
        /// <param name="name">Name of the vertex</param>
        /// <param name="edges">Nodes of the vertex</param>
        public void add_vertex(string name, Dictionary<string, Node> edges)
        {
            vertices[name] = edges;

            edges.ToList().ForEach(
                x => {
                    if (!nodesSmall.ContainsKey(x.Key))
                        nodesSmall.Add(x.Key, x.Value);
                });
        }

        /// <summary>
        /// Find shortest path
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="finish">End position</param>
        /// <returns>Array of positions</returns>
        public Node[] shortest_path(string start, string finish)
        {
            var previous = new Dictionary<string, string>();
            var distances = new Dictionary<string, double>();
            var nodes = new List<String>();

            List<Node> path = null;

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

               nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => (int)distances[x] - (int)distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<Node>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(nodesSmall[smallest]);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value.x + neighbor.Value.z;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            path.Reverse();
            return path.ToArray();
        }

        #endregion
    }
}
