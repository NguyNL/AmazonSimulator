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
        // Boolean to see if robot has a rack.
        public bool HasRack = false;
        // Unique id of the rack the robot has.
        public Guid rackGuid;
        #endregion

        #region Constructors
        /// <summary>
        /// Add paths to the robot tasks.
        /// </summary>
        /// <param name="path">The path the robot has to take.</param>
        public RobotTask(Node[] path)
        {
            // Set path.
            this.path = path;
        }

        /// <summary>
        /// Add paths to the robot and rack tasks.
        /// </summary>
        /// <param name="path">The path the robot has to take.</param>
        /// <param name="rackGuid">Id of the rack.</param>
        public RobotTask(Node[] path, Guid rackGuid)
        {
            // Set path.
            this.path = path;
            // Set rack id.
            this.rackGuid = rackGuid;
            // Set that robot has a rack.
            this.HasRack = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start the task for the robot.
        /// </summary>
        /// <param name="r">Robot object</param>
        public void StartTask(Robot r)
        {
            // If robot has a rack.
            if (this.HasRack)
                // Move robot and rack.
                r.MoveOverPath(this.path, rackGuid);
            else
                // Move robot only.
                r.MoveOverPath(this.path);
        }

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
            // List of nodes.
            List<Node> pathList = path.ToList();
            // Remove first item.
            pathList.RemoveAt(0);
            // Convert list to array.
            path = pathList.ToArray();
        }
        #endregion
    }
}