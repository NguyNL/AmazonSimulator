using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RobotTask : ITask<Robot>
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
        /// Add paths to the robot tasks.
        /// </summary>
        /// <param name="path">The path the robot has to take.</param>
        public RobotTask(Node[] path) => this.path = path;

        /// <summary>
        /// Start the task for the robot.
        /// </summary>
        /// <param name="r">Robot object</param>
        public void StartTask(Robot r) => r.MoveOverPath(this.path);

        /// <summary>
        /// Check if all the tasks have been completed.
        /// </summary>
        /// <param name="r">Robot object</param>
        /// <returns>True or False</returns>
        public bool TaskComplete(Robot r) => path.Length == 0;

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
