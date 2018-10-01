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


        public void MoveOverPath(Node[] path)
        {
            if ((this.x >= 68 && this.x < 70) && this.z >= 0 && this.z < 110)
                World.Doors.Open();
            else
                World.Doors.Close();

            if (this.x > path[0].x && this.z == path[0].z)
            {
                Console.WriteLine("left: {0} > {1}", this.x, path[0].x);
                RotateToLeft();
            }
            else if(this.x < path[0].x && this.z == path[0].z)
            {
                Console.WriteLine("right: {0} < {1}", this.x, path[0].x);
                RotateToRight();
            }
            else if(this.z < path[0].z && this.x == path[0].x)
            {
                RotateToUp();
            }
            else if(this.z > path[0].z && this.x == path[0].x)
            {
                Console.WriteLine("right: {0} < {1}", this.x, path[0].x);
                RotateToDown();
            }

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
                Tasks.First().RemovePath();
        }

        public void RotateToLeft()
        {
            if(this.rotationY != (-90 * Math.PI / 180))
            {
                InRotationAnimation = true;
                this.rotationY -= 1 * Math.PI / 180;
            }
            else
            {
                InRotationAnimation = false;
            }
            Console.WriteLine("Left: {0}", this.rotationY);
        }

        public void RotateToRight()
        {
            if (this.rotationY != (90 * Math.PI / 180))
            {
                this.rotationY += 1 * Math.PI / 180;
            }
            else
            {
                InRotationAnimation = false;
            }
            Console.WriteLine("Right: {0}", this.rotationY);
        }

        public void RotateToUp()
        {
            this.rotationY = 180 * Math.PI / 180;
            InRotationAnimation = false;
            Console.WriteLine("Up: {0}", this.rotationY);
        }

        public void RotateToDown()
        {
            this.rotationY = 0 * Math.PI / 180;
            InRotationAnimation = false;
            Console.WriteLine("Down: {0}", this.rotationY);
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