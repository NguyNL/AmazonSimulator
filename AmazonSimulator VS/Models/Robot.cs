using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Mesh, IUpdatable {
        private List<RobotTask> Tasks = new List<RobotTask>();
        private double Speed = 1;
        public string Position { get; private set; }
        private bool InRotationAnimation = false;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ, int ID) : base(x,y,z,rotationX,rotationY,rotationZ, ID) {
            this.type = "robot";
            this.Position = "07";
        }
        
        public Robot(int ID) : base(ID)
        {
            this.type = "robot";
            this.Position = "07";

            this.x =  119;
            this.y =    0;
            this.z =    0;

            this.rotationX = 0;
            this.rotationY = -90 * Math.PI / 180;
            this.rotationZ = 0;
        }

        bool RobotPositionCheck = true;

        public void MoveOverPath(Node[] path)
        {
            if ((this.x >= 68 && this.x < 70) && this.z >= 0 && this.z < 110)
                World.Doors.Open();
            else
                World.Doors.Close();

            if (RobotPositionCheck)
                checkRobotRotationPosition(path[0]);

            if(!InRotationAnimation)
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

            if (path.First().x == this.x && path.First().z == this.z)
            {
                if (path.Length > 1)
                    checkRobotRotationPosition(path[1]); 
                else
                   RobotPositionCheck = true;
            }

            if (path.First().x == this.x && path.First().z == this.z && !InRotationAnimation)
                Tasks.First().RemovePath();
        }

        private void checkRobotRotationPosition(Node node)
        {
            if (this.x > node.x && this.z == node.z)
                RotateRobot(-90);

            else if (this.x < node.x && this.z == node.z)
                RotateRobot(90);

            else if (this.z < node.z && this.x == node.x)
                RotateRobot(0);

            else if (this.z > node.z && this.x == node.x)
                RotateRobot(180);
        }

        public void RotateRobot(int degrees)
        {
            int currentDegrees = (int)(this.rotationY / Math.PI * 180);

            if (currentDegrees < 0 && degrees == 180)
                degrees *= -1;

            if (currentDegrees >= 180 && degrees == -90)
                degrees = 270;

            if (currentDegrees != degrees)
            {
                Console.WriteLine(degrees);
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
                RobotPositionCheck = false;
            }
        }

       
        public void Move(Node[] path, string position)
        {
            Tasks.Add(new RobotTask(path));
            this.Position = position;
        }


        public override bool Update(int tick)
        {
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