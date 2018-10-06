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
        public Guid vehicleID { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the crane.
        /// </summary>
        public Crane()
        {
            this.type = "crane";
            this._CraneState = CraneState.free;
            this.guid = Guid.NewGuid();
        }
        #endregion

        #region Methods
        public void Load(string vehicle, Guid vehicleID)
        {
            this.vehicle = vehicle;
            this.vehicleID = vehicleID;
            _CraneState = CraneState.loading;
            needsUpdate = true;
        }

        public void Unload()
        {
            _CraneState = CraneState.unloading;
            needsUpdate = true;
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
