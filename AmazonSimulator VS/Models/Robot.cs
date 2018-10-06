using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Mesh, IUpdatable {
        private List<RobotTask> Tasks = new List<RobotTask>();
        private double Speed = 1;
        public string Position { get; private set; }
        public string CurrentPos { get; private set; }
        private bool InRotationAnimation = false;
        private bool FirstMovement = true;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x,y,z,rotationX,rotationY,rotationZ) {
            this.type = "robot";
            this.Position = Manager.StartPoint;
            this.CurrentPos = Manager.StartPoint;
            this.guid = Guid.NewGuid();
        }
        
        public Robot()
        {
            this.guid = Guid.NewGuid();
            this.type = "robot";
            this.Position = Manager.StartPoint;
            this.CurrentPos = Manager.StartPoint;

            this.x = Manager.StartPointNode.x;
            this.y = Manager.StartPointNode.y;
            this.z = Manager.StartPointNode.z;

            this.rotationX = 0;
            this.rotationY = -90 * Math.PI / 180;
            this.rotationZ = 0;
        }

        public void MoveOverPath(Node[] path)
        {
            if ((this.x >= 100 && this.x < 104) && this.z >= 0 && this.z < 110)
                Manager.Doors.Open();
            else
                Manager.Doors.Close();

            if (FirstMovement)
                CheckRotationPosition(path[0]);

            if(!InRotationAnimation)
            {
                bool AllowedToMove = true;
                int CurrentRow = (int)Math.Floor(this.z / 25);
                int CurrentColumn = (int)Math.Floor(this.x / 25);

                if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
                {
                    CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();
                }

                List<Robot> RobotList = Manager.AllRobots
                    .Where(
                        robot => robot.CurrentPos == this.CurrentPos
                    ).ToList();

                if (RobotList.Count > 0)
                    if (RobotList[0].guid != this.guid)
                        AllowedToMove = false;

                if (AllowedToMove)
                {
                    int NextRow = path.First().x == this.x ? path.First().z > this.z ? CurrentRow - 1 : CurrentRow + 1 : CurrentRow;
                    int NextColumn = path.First().z == this.z ? path.First().x > this.x ? CurrentColumn + 1 : CurrentColumn - 1 : CurrentColumn;

                    List<Robot> RobotListNextStep = Manager.AllRobots
                        .Where(
                            robot => robot.CurrentPos == (NextRow.ToString() + NextColumn.ToString()) &&
                            robot.guid != this.guid
                        ).ToList();

                    if (RobotListNextStep.Count > 0)
                        AllowedToMove = false;
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

        public void Delete() {
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
    }
}