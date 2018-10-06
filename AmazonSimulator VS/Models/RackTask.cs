using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RackTask : ITask<Rack>
    {
        #region Properties
        // Boolean to see if startup is complete.
        private bool startupComplete = false;
        // Boolean to see if the task is complete.
        private bool complete = false;
        // Nodes array for all the possible paths.
        private Node[] path;
        #endregion

        #region Methods
        /// <summary>
        /// Add paths to the rack tasks.
        /// </summary>
        /// <param name="path">The path the rack has to take.</param>
        public RackTask(Node[] path) => this.path = path;

        /// <summary>
        /// Start the task for the rack.
        /// </summary>
        /// <param name="r">Rack object</param>
        public void StartTask(Rack r) => r.MoveOverPath(this.path);

        /// <summary>
        /// Check if all the tasks have been completed.
        /// </summary>
        /// <param name="r">Rack object</param>
        /// <returns>True or False</returns>
        public bool TaskComplete(Rack r) => path.Length == 0;

        /// <summary>
        /// Remove first path.
        /// </summary>
        public void RemovePath()
        {
            List<Node> pathList = path.ToList();
            pathList.RemoveAt(0);
            path = pathList.ToArray();
        }
        #endregion
    }
}
