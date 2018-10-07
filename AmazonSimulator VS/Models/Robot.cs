using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Robot : Mesh, IUpdatable
    {
        #region Variables
        // Create list of tasks for robot.
        private List<RobotTask> Tasks = new List<RobotTask>();
        // Set speed.
        private double Speed = 1;
        // Set bool for rotation animation.
        private bool InRotationAnimation = false;
        // Set bool for first movement.
        private bool FirstMovement = true;
        #endregion

        #region Properties
        /// <summary>
        /// Get and set position.
        /// </summary>
        public string Position { get; private set; }
        /// <summary>
        /// Get and set current position.
        /// </summary>
        public string CurrentPos { get; private set; }
        /// <summary>
        /// Get and set next position.
        /// </summary>
        public string NextPos { get; private set; }
        /// <summary>
        /// Get and set if robot has rack.
        /// </summary>
        public bool hasRack { get; set; }
        /// <summary>
        /// Get and set if robot is waiting.
        /// </summary>
        public bool isWaiting { get; set; }
        /// <summary>
        /// Get and set id of rack which the robot is carrying.
        /// </summary>
        public Guid rackGuid { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Robot constructor with custom values.
        /// </summary>
        /// <param name="x">Axis-X position</param>
        /// <param name="y">Axis-Y position</param>
        /// <param name="z">Axis-Z position</param>
        /// <param name="rotationX">Rotation axis-X</param>
        /// <param name="rotationY">Rotation axis-Y</param>
        /// <param name="rotationZ">Rotation axis-Z</param>
        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            // Create new unique id.
            this.guid = Guid.NewGuid();
            // Set type to rack.
            this.type = "robot";
            // Set start position.
            this.Position = Manager.StartPoint;
            // Set current position.
            this.CurrentPos = Manager.StartPoint;
            // Set next position.
            this.NextPos = null;
            // Set that robot doesn't have a rack.
            this.hasRack = false;
            // Set the robot status to true.
            this.isWaiting = true;
        }

        /// <summary>
        /// Robot constructor without pre-set values.
        /// </summary>
        public Robot()
        {
            // Create new unique id.
            this.guid = Guid.NewGuid();
            // Set type to rack.
            this.type = "robot";
            // Set start position.
            this.Position = Manager.StartPoint;
            // Set current position.
            this.CurrentPos = Manager.StartPoint;
            // Set next position.
            this.NextPos = null;
            // Set that robot doesn't have a rack.
            this.hasRack = false;
            // Set the robot status to true.
            this.isWaiting = true;

            // Set axis-X position.
            this.x = Manager.StartPointNode.x;
            // Set axis-Y position.
            this.y = Manager.StartPointNode.y;
            // Set axis-Z position.
            this.z = Manager.StartPointNode.z;

            // Set rotation axis-X.
            this.rotationX = 0;
            // Set rotation axis-Y.
            this.rotationY = -90 * Math.PI / 180;
            // Set rotation axis-Z.
            this.rotationZ = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check if robot can move.
        /// </summary>
        /// <param name="path">Array of nodes for the path</param>
        /// <returns>True or false</returns>
        private bool CheckMoveMent(Node[] path)
        {
            // Set if robot is allowed to move.
            bool AllowedToMove = true;

            // Set currentrow of robot.
            int CurrentRow = (int)Math.Floor(this.z / 25);
            // Set currentcolumn of robot.
            int CurrentColumn = (int)Math.Floor(this.x / 25);

            // Check if currentrow and column is the same as the current position.
            if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
            {
                // Set current position to currentrow and currentcolumn.
                CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();
            }

            // Create variable.
            int NextRow;
            // Create variable.
            int NextColumn;
            // Set plusX.
            int plusX = 0;
            // Set plusZ.
            int plusZ = 0;

            // Check if path x is the same as robot x position and path z is the same as robot z position.
            if (path.First().x == this.x && path.First().z == this.z)
            {
                // If path has more than 1 value.
                if (path.Length > 1)
                {
                    // Check if robot x position is lower than path x position.
                    if (this.x < path[1].x)
                        // Set plusX.
                        plusX = 25;

                    // Check if robot x position is higher than path x position.
                    if (this.x > path[1].x)
                        // Set plusX.
                        plusX = -25;

                    // Check if robot z position is lower than path z position.
                    if (this.z < path[1].z)
                        // Set plusZ.
                        plusZ = 25;

                    // Check if robot z position is higher than path z position.
                    if (this.z > path[1].z)
                        // Set plusZ.
                        plusZ = -25;
                }
                else
                {
                    // Set robot next move to nothing.
                    this.NextPos = null;
                }

            }
            else
            {
                // Check if robot x position is lower than path x position.
                if (this.x < path.First().x)
                    // Set plusX.
                    plusX = 25;

                // Check if robot x position is higher than path x position.
                if (this.x > path.First().x)
                    // Set plusX.
                    plusX = -25;

                // Check if robot z position is lower than path z position.
                if (this.z < path.First().z)
                    // Set plusZ.
                    plusZ = 25;

                // Check if robot z position is higher than path z position.
                if (this.z > path.First().z)
                    // Set plusZ.
                    plusZ = -25;
            }

            // NextRow is current row + value.
            NextRow = (int)Math.Floor((this.z + plusZ) / 25);
            // NextColumn is current column + value.
            NextColumn = (int)Math.Floor((this.x + plusX) / 25);

            // Check if next position is not the same.
            if ((NextRow.ToString() + NextColumn.ToString()) != NextPos)
                // Set next position.
                NextPos = NextRow.ToString() + NextColumn.ToString();

            // Get list of robots.
            List<Robot> RobotList = Manager.AllRobots
                .Where(
                    robot => robot.CurrentPos == this.CurrentPos
                ).ToList();

            // Check if list of robots has value.
            if (RobotList.Count > 0)
                // Check if first robot unique id is not the same as the robots unique id and has a movement.
                if (RobotList[0].guid != this.guid && path.Length > 1)
                    // Check if currentrow is not waiting place.
                    if (CurrentRow != 4)
                        // Set allowed move to false.
                        AllowedToMove = false;

            // List of robot next step.
            List<Robot> RobotListNextStep = Manager.AllRobots
                .Where(
                    robot => robot.CurrentPos == this.NextPos &&
                    robot.guid != this.guid
                ).ToList();

            // Check if robot list has next step.
            if (RobotListNextStep.Count > 0)
            {
                // Check if robot is not at waiting position
                if (CurrentRow != 4)
                {
                    // Set robot allowed to move to false.
                    AllowedToMove = false;

                    // If robot has 1 path left.
                    if (path.Length == 1)
                    {
                        // Remove path.
                        Tasks.First().RemovePath();
                        // set position to current position.
                        this.Position = CurrentPos;
                    }
                }
            }
            // Return if robot is allowed to move.
            return AllowedToMove;
        }

        /// <summary>
        /// Move the robot and the rack.
        /// </summary>
        /// <param name="path">Array of nodes for the path</param>
        /// <param name="rackGuid">Unique id of the rack.</param>
        /// <returns>True or false</returns>
        public void MoveOverPath(Node[] path, Guid rackGuid)
        {
            // Set that the robot has a rack.
            this.hasRack = true;

            // Check if robot x and z is in range of the door.
            if ((this.x >= 100 && this.x < 104) && this.z >= 0 && this.z < 110)
                // Open the loading deck door.
                Manager.Doors.Open();
            else
                // Close the loading deck door.
                Manager.Doors.Close();

            // Check if this is the first move.
            if (FirstMovement)
                // Check rotation position.
                CheckRotationPosition(path[0]);

            // Check if robot is not rotating.
            if (!InRotationAnimation)
            {
                // Check if robot is allowed to move.
                if (CheckMoveMent(path))
                {
                    // Move the robot Z position.
                    this.z = path.First().x == this.x ?
                        path.First().z > this.z ?
                        this.z += Speed :
                        this.z -= Speed :
                        this.z;

                    // Move the robot X position.
                    this.x = path.First().z == this.z ?
                        path.First().x > this.x ?
                        this.x += Speed :
                        this.x -= Speed :
                        this.x;
                }

                // Set new current position of robot.
                setNewCurrentPosition();
            }

            // Check if robot is at position.
            if (path.First().x == this.x && path.First().z == this.z)
            {
                // Check if robot has more paths.
                if (path.Length > 1)
                    // Check robot rotation.
                    CheckRotationPosition(path[1]);
                else
                    // Set robot's first move.
                    FirstMovement = true;
            }

            // Check if robot has arrived and isn't rotating.
            if (path.First().x == this.x && path.First().z == this.z && !InRotationAnimation)
                // Remove his path.
                Tasks.First().RemovePath();

            // Get data of all racks.
            var data = Manager.AllRacks.Where(x => x.guid == rackGuid).ToList();

            // If there is data.
            if (data.Count > 0)
            {
                // Get rack.
                Rack rack = data.First();
                // Move rack.
                rack.MoveOverPath(this.x, this.z, this.rotationY);
            }
        }

        /// <summary>
        /// Move the robot.
        /// </summary>
        /// <param name="path">Array of nodes for the path</param>
        /// <returns>True or false</returns>
        public void MoveOverPath(Node[] path)
        {
            // Set that the robot has no rack.
            this.hasRack = false;

            // Check if robot x and z is in range of the door.
            if ((this.x >= 100 && this.x < 104) && this.z >= 0 && this.z < 110)
                // Open the loading deck door.
                Manager.Doors.Open();
            else
                // Close the loading deck door.
                Manager.Doors.Close();

            // Check if this is the first move.
            if (FirstMovement)
                // Check rotation position.
                CheckRotationPosition(path[0]);

            // Check if robot is not rotating.
            if (!InRotationAnimation)
            {
                // Check if robot is allowed to move.
                if (CheckMoveMent(path))
                {
                    // Move the robot Z position.
                    this.z = path.First().x == this.x ?
                        path.First().z > this.z ?
                        this.z += Speed :
                        this.z -= Speed :
                        this.z;

                    // Move the robot X position.
                    this.x = path.First().z == this.z ?
                        path.First().x > this.x ?
                        this.x += Speed :
                        this.x -= Speed :
                        this.x;
                }

                // Set new current position of robot.
                setNewCurrentPosition();
            }

            // Check if robot is at position.
            if (path.First().x == this.x && path.First().z == this.z)
            {
                // Check if robot has more paths.
                if (path.Length > 1)
                    // Check robot rotation.
                    CheckRotationPosition(path[1]);
                else
                    // Set robot's first move.
                    FirstMovement = true;
            }

            // Check if robot has arrived and isn't rotating.
            if (path.First().x == this.x && path.First().z == this.z && !InRotationAnimation)
                // Remove his path.
                Tasks.First().RemovePath();
        }

        /// <summary>
        /// Set robot his new current position.
        /// </summary>
        private void setNewCurrentPosition()
        {
            // Get robot current row.
            int CurrentRow = (int)Math.Floor(this.z / 25);
            // Get robot current column.
            int CurrentColumn = (int)Math.Floor(this.x / 25);

            // Check if current position is not the same.
            if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
                // Set current position.
                CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();

            // Check if robot is not in loading deck.
            if (CurrentRow > 4 && this.guid == Manager.RobotInLoadingDeckGuid)
                // Set robot robot in loading deck to false.
                Manager.RobotInLoadingDeckArea = false;
        }

        /// <summary>
        /// Check if robot has to rotate.
        /// </summary>
        /// <param name="node">Node</param>
        private void CheckRotationPosition(Node node)
        {
            // Check if robot has to rotate to left.
            if (this.x > node.x && this.z == node.z)
                RotateObject(-90);

            // Check if robot has to rotate to right.
            else if (this.x < node.x && this.z == node.z)
                RotateObject(90);

            // Check if robot has to rotate to top.
            else if (this.z < node.z && this.x == node.x)
                RotateObject(0);

            // Check if robot has to rotate to bottom.
            else if (this.z > node.z && this.x == node.x)
                RotateObject(180);
        }

        /// <summary>
        /// Rotate the robot.
        /// </summary>
        /// <param name="degrees">Amount of degrees</param>
        private void RotateObject(int degrees)
        {
            // Get current degrees the robot is facing.
            int currentDegrees = (int)(this.rotationY / Math.PI * 180);

            // Check if robot is not over rotating.
            if (currentDegrees < 0 && degrees == 180)
                // Set degrees.
                degrees *= -1;

            // Check if robot is not over rotating.
            if (currentDegrees >= 180 && degrees == -90)
                // Set degrees.
                degrees = 270;

            // Check if current degrees is not the same as the new degrees.
            if (currentDegrees != degrees)
            {
                // Set robot is in rotation animation
                InRotationAnimation = true;
                // Check if robot rotation is higher than new degrees.
                if (this.rotationY > (degrees * Math.PI / 180))
                {
                    // Set robot rotationY.
                    this.rotationY -= 1 * Math.PI / 180;
                }
                else
                {
                    // Set robot rotationY.
                    this.rotationY += 1 * Math.PI / 180;
                }
            }
            else
            {
                // Check if current degrees is -180.
                if (currentDegrees == -180)
                    // Set robot rotationY.
                    this.rotationY = 180 * Math.PI / 180;

                // Check if current degrees is 270.
                if (currentDegrees == 270)
                    // Set robot rotationY.
                    this.rotationY = -90 * Math.PI / 180;

                // Robot is not rotating.
                InRotationAnimation = false;
                // This is not the robot's first move.
                FirstMovement = false;
            }
        }

        /// <summary>
        /// Move the robot.
        /// </summary>
        /// <param name="path">Path the robot has to take</param>
        /// <param name="position">Position where to robot has to go to</param>
        public void Move(Node[] path, string position)
        {
            // Add new task.
            Tasks.Add(new RobotTask(path));
            // Set position.
            this.Position = position;
        }

        /// <summary>
        /// Move the robot.
        /// </summary>
        /// <param name="path">Path the robot has to take</param>
        /// <param name="position">Position where to robot has to go to</param>
        /// <param name="rack">Unique id of the rack the robot is carrying</param>
        public void Move(Node[] path, string position, Guid rack)
        {
            // Add new task.
            Tasks.Add(new RobotTask(path, rack));
            // Set position.
            this.Position = position;
        }

        /// <summary>
        /// Delete robot.
        /// </summary>
        public void Delete()
        {
            // Set robot action to delete.
            this.action = "delete";
        }

        /// <summary>
        /// Clear robot tasks.
        /// </summary>
        private void Clear()
        {
            // Clear tasks of the robot.
            Tasks.Clear();
        }

        /// <summary>
        /// Update function for the boat.
        /// </summary>
        /// <param name="tick">Tick speed</param>
        /// <returns>True or False.</returns>
        public override bool Update(int tick)
        {
            // Check if robot action is deleting.
            if (this.action == "delete")
            {
                // Clear robot.
                Clear();
                // Return true.
                return true;
            }

            // Check if robot has tasks.
            if (Tasks.Count > 0)
            {
                // Set is waiting to false.
                this.isWaiting = false;

                // If robot completed his task.
                if (Tasks.First().TaskComplete(this))
                    // Remove the task.
                    Tasks.RemoveAt(0);

                // If robot has more tasks.
                if (Tasks.Count > 0)
                    // Start new task.
                    Tasks.First().StartTask(this);
                // Return true.
                return true;
            }
            else
            {
                // Set is waiting to true.
                this.isWaiting = true;
            }
            // Return false.
            return false;
        }
        #endregion
    }
}