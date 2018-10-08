using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Boat : Mesh, IUpdatable
    {
        #region Variables
        // Set maximum speed of boat.
        private double MaxSpeed = 0.03;
        // Set current speed of boat.
        private double CurrentSpeed = 0.03;
        // Integer for the amount of racks loaded.
        public int NumberOfRacksLoaded = 3;
        // Boolean to see if boat is moving to crane.
        private bool MovingToCrane = false;
        // Boolean to see if boat is moving away from the crane.
        private bool MovingAwayFromCrane = false;

        #endregion

        #region Properties
        /// <summary>
        /// Position of boat.
        /// </summary>
        public Transport Position { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Boat constructor with custom parameters.
        /// </summary>
        /// <param name="x">X-axis</param>
        /// <param name="y">Y-axis</param>
        /// <param name="z">Z-axis</param>
        /// <param name="rotationX">Rotation X-axis</param>
        /// <param name="rotationY">Rotation Y-axis</param>
        /// <param name="rotationZ">Rotation Z-axis</param>
        public Boat(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            // Create unique id for the boat.
            this.guid = Guid.NewGuid();
            // Set type to boat.
            this.type = "boat";
            // Set position.
            this.Position = Transport.created;
        }

        /// <summary>
        /// Boat constructor with static parameters.
        /// </summary>
        public Boat()
        {
            // Create unique id for the boat.
            this.guid = Guid.NewGuid();
            // Set type to boat.
            this.type = "boat";
            // Set position.
            this.Position = Transport.created;

            // Set X-axis to 0.
            this.x = 0;
            // Set Y-axis to 0.
            this.y = 0;
            // Set Z-axis to 30.
            this.z = 30;

            // Set rotation X-axis to 0;
            this.rotationX = 0;
            // Set rotation Y-axis to 0;
            this.rotationY = 0;
            // Set rotation Z-axis to 0;
            this.rotationZ = 0;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Function to set MovingToCrane variable to true.
        /// </summary>
        public void MoveToCrane() => MovingToCrane = true;

        /// <summary>
        /// Function to set MovingAwayFromCrane variable to true.
        /// </summary>
        public void MoveAwayFromCrane() => MovingAwayFromCrane = true;

        private void MoveToLoadStation()
        {
            // check if MovingToCrane is true and MovingAwayFromCrane is false.
            if (MovingToCrane && !MovingAwayFromCrane)
            {
                // Check if truck is at loading deck.
                if (this.Position != Transport.toLoadingDeck)
                    // Set position to loading deck.
                    this.Position = Transport.toLoadingDeck;

                // check is the boat's Z position is higher than 8
                if (this.z > 8)
                {
                    // lower the boat's Z position by deducting CurrentSpeed.
                    this.z -= CurrentSpeed;
                    // Set needsUpdate variable to true.
                    needsUpdate = true;
                }
                // check if boat's Z position is higher than 0.
                else if (this.z > 0)
                {
                    // check if Currentspeed is lower than 0.0002 otherwise.
                    if (CurrentSpeed < 0.0002)
                        // Set CurrentSpeed to 0.0002.
                        CurrentSpeed = 0.0002;
                    else
                        // Set CurrentSpeed to CurrentSpeed devided the steps that are left (Slow the CurrentSpeed).
                        CurrentSpeed -= CurrentSpeed / (this.z / CurrentSpeed);
                    // check if boat's Z is lower than 0.001.
                    if (this.z < 0.001)
                    {
                        // Set boat's Z position to 0.
                        this.z = 0;
                        // Set MovingToCrane to false.
                        MovingToCrane = false;

                        // Check if boat is not at loadingdeck.
                        if (this.Position != Transport.loadingDeck)
                            // Set boat position at loadingdeck.
                            this.Position = Transport.loadingDeck;
                    }
                    else
                        // Set boat's Z position by deducting Z - CurrentSpeed.
                        this.z -= CurrentSpeed;
                    // Set needsUpdate to true.
                    needsUpdate = true;
                }
            }
        }

        /// <summary>
        /// Function to move the boat away from the load station / dock
        /// </summary>
        private void MoveAwayFromLoadStation()
        {
            // Check if MovingAwayFromCrane is true and MovingToCrane is false.
            if (MovingAwayFromCrane && !MovingToCrane)
            {
                // Check if boat position is not fromLoadingdeck.
                if (this.Position != Transport.fromLoadingDeck)
                    // Set boat position to fromLoadingDeck.
                    this.Position = Transport.fromLoadingDeck;

                // Repeatedly add 0.00005 to CurrentSpeed.
                CurrentSpeed += 0.00005;
                // Check if Currentspeed is higher than the MaxSpeed.
                if (CurrentSpeed > MaxSpeed)
                    // Set CurrentSpeed to MaxSpeed.
                    CurrentSpeed = MaxSpeed;
                // Lower boat's Z position with CurrentSpeed.
                this.z -= CurrentSpeed;
                // Check if boat's Z position is lower than -30.
                if (this.z < -30)
                {
                    // Set boat's Z position to 30.
                    this.z = 30;
                    // Set MovingAwayFromCrane to false.
                    MovingAwayFromCrane = false;

                    // Check if transport is not done.
                    if (this.Position != Transport.finish)
                        // Set transport to done.
                        this.Position = Transport.finish;
                }
                // Set needsUpdate to true.
                needsUpdate = true;
            }
        }

        /// <summary>
        /// Update function for the boat.
        /// </summary>
        /// <param name="tick">Tick speed</param>
        /// <returns>True or False.</returns>
        public override bool Update(int tick)
        {
            // Call move to load station.
            MoveToLoadStation();
            // Call move away from load station.
            MoveAwayFromLoadStation();
            // Check if needsUpdate is true.
            if (needsUpdate)
            {
                // Set needsUpdate to false.
                needsUpdate = false;
                // Return true.
                return true;
            }
            // Return false.
            return false;
        }
        #endregion
    }
}