﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Truck : Mesh, IUpdatable
    {
        private double MaxSpeed = 0.08;
        private double CurrentSpeed = 0.08;
        private bool MovingToCrane = false;
        private bool MovingAwayFromCrane = false;
        public int NumberOfRacksLoaded = 4;
        public Transport Position { get; private set; }

        public Truck(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "truck";
            this.guid = Guid.NewGuid();
            this.Position = Transport.created;
        }

        public Truck()
        {
            this.guid = Guid.NewGuid();
            this.type = "truck";
            this.Position = Transport.created;

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
                if (this.Position != Transport.toLoadingDeck)
                    this.Position = Transport.toLoadingDeck;

                if (this.z > 3.5)
                {
                    this.z -= CurrentSpeed;
                    needsUpdate = true;
                }
                else if (this.z > 0)
                {
                    if (CurrentSpeed < 0.00004)
                        CurrentSpeed = 0.00004;
                    else
                        CurrentSpeed -= CurrentSpeed / (this.z / CurrentSpeed);

                    if (this.z < 0.0001)
                    {
                        this.z = 0;
                        MovingToCrane = false;

                        if (this.Position != Transport.loadingDeck)
                            this.Position = Transport.loadingDeck;

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
            if(MovingAwayFromCrane && !MovingToCrane)
            {
                if (this.Position != Transport.fromLoadingDeck)
                    this.Position = Transport.fromLoadingDeck;

                CurrentSpeed += 0.0005;
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