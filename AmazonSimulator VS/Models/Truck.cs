using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models
{
    public class Truck : Mesh, IUpdatable
    {
        #region Properties
        // Set maximum speed of truck.
        private double MaxSpeed = 0.08;
        // Set current speed of truck.
        private double CurrentSpeed = 0.08;
        // Boolean to see if truck is moving to crane.
        private bool MovingToCrane = false;
        // Boolean to see if truck is moving away from the crane.
        private bool MovingAwayFromCrane = false;
        // Integer for the amount of racks loaded.
        public int NumberOfRacksLoaded = 4;
        /// <summary>
        /// Position of truck.
        /// </summary>
        public Transport Position { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Truck constructor with custom parameters.
        /// </summary>
        /// <param name="x">X-axis</param>
        /// <param name="y">Y-axis</param>
        /// <param name="z">Z-axis</param>
        /// <param name="rotationX">Rotation X-axis</param>
        /// <param name="rotationY">Rotation Y-axis</param>
        /// <param name="rotationZ">Rotation Z-axis</param>
        public Truck(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            // Set type to truck.
            this.type = "truck";
            // Set position.
            this.Position = Transport.created;
            // Create unique id for the truck.
            this.guid = Guid.NewGuid();
        }

        /// <summary>
        /// Truck constructor with static parameters.
        /// </summary>
        public Truck()
        {
            // Create unique id for the truck.
            this.guid = Guid.NewGuid();
            // Set type to truck.
            this.type = "truck";
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

        /// <summary>
        /// Function to move the truck to the Load station / dock
        /// </summary>
        private void MoveToLoadStation()
        {
            // Check if MovingToCrane is true and MovingAwayFromCrane is false.
            if (MovingToCrane && !MovingAwayFromCrane)
            {
                // Check if truck is at loading deck.
                if (this.Position != Transport.toLoadingDeck)
                    // Set position to loading deck.
                    this.Position = Transport.toLoadingDeck;

                // Check is the truck's Z position is higher than 3.5
                if (this.z > 3.5)
                {
                    // Lower the truck's Z position by deducting CurrentSpeed.
                    this.z -= CurrentSpeed;
                    // Set needsUpdate variable to true.
                    needsUpdate = true;
                }
                // Check if truck's Z position is higher than 0.
                else if (this.z > 0)
                {
                    // Check if Currentspeed is lower than 0.00004 otherwise.
                    if (CurrentSpeed < 0.00004)
                        // Set CurrentSpeed to 0.00004.
                        CurrentSpeed = 0.00004;
                    else
                        // Set CurrentSpeed to CurrentSpeed devided the steps that are left (Slow the CurrentSpeed).
                        CurrentSpeed -= CurrentSpeed / (this.z / CurrentSpeed);

                    // Check if truck's Z is lower than 0.001.
                    if (this.z < 0.0001)
                    {
                        // Set truck's Z position to 0.
                        this.z = 0;
                        // Set MovingToCrane to false.
                        MovingToCrane = false;

                        // Check if truck position is at loading deck.
                        if (this.Position != Transport.loadingDeck)
                            // Set truck position is at loading deck.
                            this.Position = Transport.loadingDeck;
                    }
                    else
                        // Set truck's Z position by deducting Z - CurrentSpeed.
                        this.z -= CurrentSpeed;

                    // Set needsUpdate to true.
                    needsUpdate = true;
                }
            }
        }

        /// <summary>
        /// Function to move the truck away from the load station / dock
        /// </summary>
        private void MoveAwayFromLoadStation()
        {
            // Check if MovingAwayFromCrane is true and MovingToCrane is false.
            if (MovingAwayFromCrane && !MovingToCrane)
            {
                // Check if truck position is away from the loading dock.
                if (this.Position != Transport.fromLoadingDeck)
                    // Set truck position to from the loading deck.
                    this.Position = Transport.fromLoadingDeck;

                // Repeatedly add 0.0005 to CurrentSpeed.
                CurrentSpeed += 0.0005;
                // Check if Currentspeed is higher than the MaxSpeed.
                if (CurrentSpeed > MaxSpeed)
                    // Set CurrentSpeed to MaxSpeed.
                    CurrentSpeed = MaxSpeed;

                // Lower truck's Z position with CurrentSpeed.
                this.z -= CurrentSpeed;

                // Check if truck's Z position is lower than -30.
                if (this.z < -30)
                {
                    // Set truck's Z position to 30.
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
        /// Update function for the truck.
        /// </summary>
        /// <param name="tick">Tick speed</param>
        /// <returns>True or False.</returns>
        public override bool Update(int tick)
        {
            // Move truck to load station.
            MoveToLoadStation();
            // Move truck aray from load station.
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