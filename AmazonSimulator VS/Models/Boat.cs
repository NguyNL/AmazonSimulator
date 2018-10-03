using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Boat : Mesh, IUpdatable
    {
        private double Speed = 1;
        public string Position { get; private set; }
        private bool InRotationAnimation = false;
        private bool FirstMovement = true;

        public Boat(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "boat";
            this.Position = "07";
            this.guid = Guid.NewGuid();
        }

        public Boat()
        {
            this.guid = Guid.NewGuid();
            this.type = "boat";
            this.Position = "07";

            this.x = 119;
            this.y = 0;
            this.z = 0;

            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;
        }

        public void MoveOverPath(Node[] path)
        {
           
        }

        public void Move(Node[] path, string position)
        {
            //Tasks.Add(new RobotTask(path));
            //this.Position = position;
        }


        public override bool Update(int tick)
        {
            //if (Tasks.Count > 0)
            //{
            //    if (Tasks.First().TaskComplete(this))
            //        Tasks.RemoveAt(0);

            //    if (Tasks.Count > 0)
            //        Tasks.First().StartTask(this);

            //    return true;
            //}

            return false;
        }
    }
}