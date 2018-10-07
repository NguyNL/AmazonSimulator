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
        public void MoveOverPath(Node[] path)
        {
            if ((this.x >= 100 && this.x < 104) && this.z >= 0 && this.z < 110)
                Manager.Doors.Open();
            else
                Manager.Doors.Close();

            if (FirstMovement)
                CheckRotationPosition(path[0]);

            if (!InRotationAnimation)
            {
                bool AllowedToMove = true;

                int CurrentRow = (int)Math.Floor(this.z / 25);
                int CurrentColumn = (int)Math.Floor(this.x / 25);

                if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
                {
                    CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();
                }

                int NextRow;
                int NextColumn;
                int plusX = 0;
                int plusZ = 0;

                if (path.First().x == this.x && path.First().z == this.z)
                {
                    if (path.Length > 1)
                    {
                        if (this.x < path[1].x)
                            plusX = 25;

                        if (this.x > path[1].x)
                            plusX = +25;

                        if (this.z < path[1].z)
                            plusZ = 25;

                        if (this.z > path[1].z)
                            plusZ = -25;
                    }
                    else
                    {
                        this.NextPos = null;
                    }

                }
                else
                {
                    if (this.x < path.First().x)
                        plusX = 25;

                    if (this.x > path.First().x)
                        plusX = -25;

                    if (this.z < path.First().z)
                        plusZ = 25;

                    if (this.z > path.First().z)
                        plusZ = -25;
                }

                NextRow = (int)Math.Floor((this.z + plusZ) / 25);
                NextColumn = (int)Math.Floor((this.x + plusX) / 25);

                if ((NextRow.ToString() + NextColumn.ToString()) != NextPos)
                    NextPos = NextRow.ToString() + NextColumn.ToString();

                List<Robot> RobotList = Manager.AllRobots
                    .Where(
                        robot => robot.CurrentPos == this.CurrentPos
                    ).ToList();

                if (RobotList.Count > 0)

                    if (RobotList[0].guid != this.guid && path.Length > 1)
                        AllowedToMove = false;

                List<Robot> RobotListNextStep = Manager.AllRobots
                    .Where(
                        robot => robot.CurrentPos == this.NextPos &&
                        robot.guid != this.guid
                    ).ToList();

                if (RobotListNextStep.Count > 0)
                {
                    AllowedToMove = false;

                    if (path.Length == 1)
                        Tasks.First().RemovePath();
                }

                if (AllowedToMove)
                {
                    this.z = path.First().x == this.x ?
                        path.First().z > this.z ?
                        this.z += Speed :
                        this.z -= Speed :
                        this.z;

                    this.x = path.First().z == this.z ?
                        path.First().x > this.x ?
                        this.x += Speed :
                        this.x -= Speed :
                        this.x;
                }

                CurrentRow = (int)Math.Floor(this.z / 25);
                CurrentColumn = (int)Math.Floor(this.x / 25);

                if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
                    CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();
            }

            if (path.First().x == this.x && path.First().z == this.z)
            {
                if (path.Length > 1)
                    CheckRotationPosition(path[1]);
                else
                    FirstMovement = true;
            }

            if (path.First().x == this.x && path.First().z == this.z && !InRotationAnimation)
                Tasks.First().RemovePath();
        }

        private void CheckRotationPosition(Node node)
        {
            if (this.x > node.x && this.z == node.z)
                RotateObject(-90);

            else if (this.x < node.x && this.z == node.z)
                RotateObject(90);

            else if (this.z < node.z && this.x == node.x)
                RotateObject(0);

            else if (this.z > node.z && this.x == node.x)
                RotateObject(180);
        }

        private void RotateObject(int degrees)
        {
            int currentDegrees = (int)(this.rotationY / Math.PI * 180);

            if (currentDegrees < 0 && degrees == 180)
                degrees *= -1;

            if (currentDegrees >= 180 && degrees == -90)
                degrees = 270;

            if (currentDegrees != degrees)
            {
                InRotationAnimation = true;
                if (this.rotationY > (degrees * Math.PI / 180))
                {
                    this.rotationY -= 1 * Math.PI / 180;
                }
                else
                {
                    this.rotationY += 1 * Math.PI / 180;
                }
            }
            else
            {
                if (currentDegrees == -180)
                    this.rotationY = 180 * Math.PI / 180;

                if (currentDegrees == 270)
                    this.rotationY = -90 * Math.PI / 180;

                InRotationAnimation = false;
                FirstMovement = false;
            }
        }

        public void Move(Node[] path, string position)
        {
            Tasks.Add(new RobotTask(path));
            this.Position = position;
        }

        public void Delete()
        {
            this.action = "delete";
        }

        private void Clear()
        {
            Tasks.Clear();
        }

        public override bool Update(int tick)
        {
            if (this.action == "delete")
            {
                Clear();
                return true;
            }


            if (Tasks.Count > 0)
            {
                if (Tasks.First().TaskComplete(this))
                    Tasks.RemoveAt(0);

                if (Tasks.Count > 0)
                    Tasks.First().StartTask(this);

                return true;
            }



            return false;
        }
        #endregion
    }
}