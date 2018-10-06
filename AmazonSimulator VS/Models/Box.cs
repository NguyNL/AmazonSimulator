using System;

namespace Models
{
    public class Box : Mesh, IUpdatable
    {
        #region Properties
        // Get and set parent guid.
        private Guid ParentGuid { get; set; }
        // get and set X-axis scale.
        private double ScaleX { get; set; }
        // get and set Y-axis scale.
        private double ScaleY { get; set; }
        // get and set Z-axis scale.
        private double ScaleZ { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the box class.
        /// </summary>
        /// <param name="x">X-axis</param>
        /// <param name="y">Y-axis</param>
        /// <param name="z">Z-axis</param>
        /// <param name="rotationX">Rotation X-axis</param>
        /// <param name="rotationY">Rotation Y-axis</param>
        /// <param name="rotationZ">Rotation Z-axis</param>
        /// <param name="scaleX">Scale X-axis</param>
        /// <param name="scaleY">Scale Y-axis</param>
        /// <param name="scaleZ">Scale Z-axis</param>
        /// <param name="parentGuid">Parent guid</param>
        public Box(double x, double y, double z, double rotationX, double rotationY, double rotationZ, double scaleX, double scaleY, double scaleZ, Guid parentGuid) : base(x, y, z, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ)
        {
            // Set box guid to a new guid.
            this.guid = Guid.NewGuid();
            // Set type to box.
            this.type = "box";
            // set rotation X-axis.
            this.rotationX = 180 * Math.PI / 180;
            // Set parent guid.
            this.ParentGuid = parentGuid;
        }
        #endregion
    }
}