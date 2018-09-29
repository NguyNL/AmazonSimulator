using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Mesh, IUpdatable {
        private List<RobotTask> Tasks = new List<RobotTask>();
        private double Speed = 1; 

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ, int ID) : base(x,y,z,rotationX,rotationY,rotationZ, ID) {
            this.type = "robot";
        }
        
        public Robot(int ID) : base(ID)
        {
            this.type = "robot";

            this.x =  119;
            this.y =    0;
            this.z =    0;

            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;
        }


        public void MoveOverPath(Node[] path)
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

            if (path.First().x == this.x && path.First().z == this.z)
                Tasks.First().RemovePath();
        }

        public void Move(Node[] path)
        {
            Tasks.Add(new RobotTask(path));
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