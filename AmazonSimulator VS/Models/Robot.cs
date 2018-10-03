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
        private bool FirstMovement = true;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x,y,z,rotationX,rotationY,rotationZ) {
            this.type = "robot";
            this.Position = "07";
            this.guid = Guid.NewGuid();
        }
        
        public Robot()
        {
            this.guid = Guid.NewGuid();
            this.type = "robot";
            this.Position = "07";

            this.x =  119;
            this.y =    0;
            this.z =    0;

            this.rotationX = 0;
            this.rotationY = -90 * Math.PI / 180;
            this.rotationZ = 0;
        }

        public void MoveOverPath(Node[] path)
        {
            if ((this.x >= 100 && this.x < 104) && this.z >= 0 && this.z < 110)
                World.Doors.Open();
            else
                World.Doors.Close();

            if (FirstMovement)
                CheckRotationPosition(path[0]);

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

        public void RotateObject(int degrees)
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