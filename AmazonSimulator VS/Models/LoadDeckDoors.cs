using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class LoadDeckDoors  : Mesh, IUpdatable
    {
        #region properties
        /// <summary>
        /// Open door progress.
        /// </summary>
        private bool OpeningProgress { get; set; }
        /// <summary>
        /// Close door progress.
        /// </summary>
        private bool ClosingProgress { get; set; }

        /// <summary>
        /// NeedUpdate.
        /// </summary>
        private bool NeedUpdate { get; set; }
        /// <summary>
        /// Door open position X.
        /// </summary>
        private double OpenPositionX { get; set; }
        /// <summary>
        /// Door close position X.
        /// </summary>
        private double ClosePositionX { get; set; }

        /// <summary>
        /// Door animation step.
        /// </summary>
        private double DoorAnimationStep { get; set; }
        #endregion

        #region Constructors
        public LoadDeckDoors()
        {
            // Create unique id.
            this.guid = Guid.NewGuid();
            // Opening is false.
            this.OpeningProgress = false;
            // Close is false.
            this.ClosingProgress = false;

            // Type is doors.
            this.type = "doors";
            // NeedUpdate is false.
            this.NeedUpdate = false;

            // open door position X.
            this.OpenPositionX = 11.156;//9.25;
            // Close door position X.
            this.ClosePositionX = 9.856;//7.95; //9.856; 1.906
            // Set X to closed position.
            this.x = ClosePositionX; 
            // Set Y to 7.
            this.y = 7;
            // Set Z to -1.67.
            this.z = -1.67;

            // Set no rotation.
            this.rotationX = 0;
            // Set no rotation.
            this.rotationY = 0;
            // Set no rotation.
            this.rotationZ = 0;

            // Set door animation step.
            this.DoorAnimationStep = 0.05;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Open the doors.
        /// </summary>
        public void Open()
        {
            // Check if door is closing.
            if (ClosingProgress)
                // Set closing progress to false.
                ClosingProgress = false;
            // Set opening progress to true.
            OpeningProgress = true;
        }

        /// <summary>
        /// Close the doors.
        /// </summary>
        public void Close()
        {
            // Check if doors are opening.
            if (OpeningProgress) return;
            // Set closing progress to true.
            ClosingProgress = true;
        }

        /// <summary>
        /// Move the doors.
        /// </summary>
        private void Move()
        {
            // Check if doors are in progress of opening.
            if (OpeningProgress) {
                // Check if OpenPosition is lower or equal to the current position.
                if (OpenPositionX <= x)
                    // Cancel opening progress.
                    OpeningProgress = false;
                else
                {
                    // Open the doors.
                    x += DoorAnimationStep;
                    // Set update to true.
                    NeedUpdate = true;
                }
                    
            }

            // Check if doors are in progress of closing.
            if (ClosingProgress)
            {
                // Check if ClosePosition is higher or equal to the current position.
                if (ClosePositionX >= x)
                    // Cancel closing progress.
                    ClosingProgress = false;
                else
                {
                    // Close the doors.
                    x -= DoorAnimationStep;
                    // Set update to true.
                    NeedUpdate = true;
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="tick">Tick timer</param>
        /// <returns>True or false</returns>
        bool IUpdatable.Update(int tick)
        {
            // Start move function.
            Move();

            // Check if doors need to update.
            if (NeedUpdate)
            {
                // Set update to false.
                NeedUpdate = false;
                // Return true.
                return true;
            }
            // Return false.
            return false;
        }
        #endregion
    }
}
