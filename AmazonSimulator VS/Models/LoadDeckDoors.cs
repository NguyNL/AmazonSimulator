using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class LoadDeckDoors  : Mesh, IUpdatable
    {
        private bool OpeningProgress { get; set; }
        private bool ClosingProgress { get; set; }

        private bool NeedUpdate { get; set; }
        private double OpenPositionX { get; set; }
        private double ClosePositionX { get; set; }

        private double DoorAnimationStep { get; set; }


        public LoadDeckDoors()
        {
            this.guid = Guid.NewGuid();
            this.OpeningProgress = false;
            this.ClosingProgress = false;

            this.type = "doors";
            this.NeedUpdate = false;

            this.OpenPositionX = 11.156;//9.25;
            this.ClosePositionX = 9.856;//7.95; //9.856; 1.906
            this.x = ClosePositionX; 
            this.y = 7;
            this.z = -1.67;

            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;

            this.DoorAnimationStep = 0.05;
        }

        public void Open()
        {
            if (ClosingProgress)
                ClosingProgress = false;

            OpeningProgress = true;
        }

        public void Close()
        {
            if (OpeningProgress) return;

            ClosingProgress = true;
        }

        private void Move()
        {
            if (OpeningProgress) {
                if (OpenPositionX <= x)
                    OpeningProgress = false;
                else
                {
                    x += DoorAnimationStep;
                    NeedUpdate = true;
                }
                    
            }

            if (ClosingProgress)
            {
                if (ClosePositionX >= x)
                    ClosingProgress = false;
                else
                {
                    x -= DoorAnimationStep;
                    NeedUpdate = true;
                }
            }
        }

        bool IUpdatable.Update(int tick)
        {
            Move();

            if (NeedUpdate)
            {
                NeedUpdate = false;

                return true;
            }

            return false;
        }
    }
}
