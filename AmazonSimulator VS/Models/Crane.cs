using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Crane : Mesh, IUpdatable
    {
        #region Properties
        /// <summary>
        /// Status of the crane.
        /// </summary>
        public CraneState _CraneState { get; private set; }
        /// <summary>
        /// Which vehicle.
        /// </summary>
        public string vehicle { get; private set; }
        /// <summary>
        /// Id of the vehicle.
        /// </summary>
        public Guid vehicleID { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the crane.
        /// </summary>
        public Crane()
        {
            // Set type to crane.
            this.type = "crane";
            // Set crane status to free.
            this._CraneState = CraneState.free;
            // Create unique id.
            this.guid = Guid.NewGuid();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle">Vehicle type</param>
        /// <param name="vehicleID">Vehicle id</param>
        public void Load(string vehicle, Guid vehicleID)
        {
            // Get and set vehicle type.
            this.vehicle = vehicle;
            // Get and set vehicle id.
            this.vehicleID = vehicleID;
            // Set crane status to loading.
            _CraneState = CraneState.loading;
            // Set needsUpdate to true.
            needsUpdate = true;
        }
        
        /// <summary>
        /// Unload the boxes.
        /// </summary>
        public void Unload()
        {
            // Set crane status to unloading.
            _CraneState = CraneState.unloading;
            // Set needsUpdate to true.
            needsUpdate = true;
        }

        /// <summary>
        /// Unload the boxes.
        /// </summary>
        public void Free()
        {
            // Set crane status to free.
            _CraneState = CraneState.free;
        }

        /// <summary>
        /// Update function for the Crane.
        /// </summary>
        /// <param name="tick">Tick speed</param>
        /// <returns>True or False.</returns>
        public override bool Update(int tick)
        {
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