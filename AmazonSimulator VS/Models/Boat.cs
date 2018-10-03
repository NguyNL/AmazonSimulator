using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Boat : Mesh, IUpdatable
    {
        private double MaxSpeed = 0.03;
        private double CurrentSpeed = 0.03;
        private bool MovingToCrane = false;
        private bool MovingAwayFromCrane = false;
        public string Position { get; private set; }

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

            this.x = 0;
            this.y = 0;
            this.z = 30;

            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;
        }

        public void MoveToCrane() => MovingToCrane = true;
        public void MoveAwayFromCrane() => MovingAwayFromCrane = true;

        private void MoveToLoadStation()
        {
            if (MovingToCrane && !MovingAwayFromCrane)
            {
                if (this.z > 8)
                {
                    this.z -= CurrentSpeed;
                    needsUpdate = true;
                }
                else if (this.z > 0)
                {
                    if (CurrentSpeed < 0.0002)
                        CurrentSpeed = 0.0002;
                    else
                        CurrentSpeed -= CurrentSpeed / (this.z / CurrentSpeed);

                    if (this.z < 0.001)
                    {
                        this.z = 0;
                        MovingToCrane = false;
                        this.MoveAwayFromCrane();
                    }
                    else
                        this.z -= CurrentSpeed;

                    needsUpdate = true;
                }
            }
        }

        private void MoveAwayFromLoadStation()
        {
            if (MovingAwayFromCrane && !MovingToCrane)
            {
                CurrentSpeed += 0.00005;
                if (CurrentSpeed > MaxSpeed)
                    CurrentSpeed = MaxSpeed;

                this.z -= CurrentSpeed;

                if (this.z < -30)
                {
                    this.z = 30;
                    MovingAwayFromCrane = false;
                    this.MoveToCrane();
                }

                needsUpdate = true;
            }
        }

        public override bool Update(int tick)
        {
            MoveToLoadStation();
            MoveAwayFromLoadStation();
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}