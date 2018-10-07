using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Mesh : IUpdatable {
        #region Variables
        // Axis-X position.
        private double _x = 0;
        // Axis-Y position.
        private double _y = 0;
        // Axis-Z position.
        private double _z = 0;
        // Rotation axis-X position.
        private double _rX = 0;
        // Rotation axis-y position.
        private double _rY = 0;
        // Rotation axis-z position.
        private double _rZ = 0;
        // Scale axis-X position.
        private double _sX = 0;
        // Scale axis-Y position.
        private double _sY = 0;
        // Scale axis-Z position.
        private double _sZ = 0;

        // Action.
        private string _action = "update";

        // Set boolean needsUpdate.
        public bool needsUpdate = true;
        #endregion

        #region Properties
        /// <summary>
        /// Getting and setting type.
        /// </summary>
        public string type { get; protected set; }
        /// <summary>
        /// Getting and setting unique id.
        /// </summary>
        public Guid guid { get; protected set; }
        /// <summary>
        /// Getting and setting axis-X.
        /// </summary>
        public double x { get { return _x; } protected set { _x = value; } }
        /// <summary>
        /// Getting and setting axis-Y.
        /// </summary>
        public double y { get { return _y; } protected set { _y = value; } }
        /// <summary>
        /// Getting and setting axis-Z.
        /// </summary>
        public double z { get { return _z; } protected set { _z = value; } }
        /// <summary>
        /// Getting and setting rotation axis-X.
        /// </summary>
        public double rotationX { get { return _rX; } protected set { _rX = value; } }
        /// <summary>
        /// Getting and setting rotation axis-Y.
        /// </summary>
        public double rotationY { get { return _rY; } protected set { _rY = value; } }
        /// <summary>
        /// Getting and setting rotation axis-Z.
        /// </summary>
        public double rotationZ { get { return _rZ; } protected set { _rZ = value; } }
        /// <summary>
        /// Getting and setting scale axis-X.
        /// </summary>
        public double scaleX { get { return _sX; } protected set { _sX = value; } }
        /// <summary>
        /// Getting and setting scale axis-Y.
        /// </summary>
        public double scaleY { get { return _sY; } protected set { _sY = value; } }
        /// <summary>
        /// Getting and setting scale axis-Z.
        /// </summary>
        public double scaleZ { get { return _sZ; } protected set { _sZ = value; } }
        /// <summary>
        /// Getting and setting action.
        /// </summary>
        public string action { get { return _action; } protected set { _action = value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Mesh constructor with various values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <param name="rotationX">Rotation axis-X</param>
        /// <param name="rotationY">Rotation axis-Y</param>
        /// <param name="rotationZ">Rotation axis-Z</param>
        public Mesh(double x, double y, double z, double rotationX, double rotationY, double rotationZ)
        {
            // Set mesh X position to parameter X.
            this._x = x;
            // Set mesh Y position to parameter Y.
            this._y = y;
            // Set mesh Z position to parameter Z.
            this._z = z;

            // Set mesh rotation X position to parameter rotation X.
            this._rX = rotationX;
            // Set mesh rotation Y position to parameter rotation Y.
            this._rY = rotationY;
            // Set mesh rotation Z position to parameter rotation Z.
            this._rZ = rotationZ;
        }

        /// <summary>
        /// Mesh constructor with various values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <param name="rotationX">Rotation axis-X</param>
        /// <param name="rotationY">Rotation axis-Y</param>
        /// <param name="rotationZ">Rotation axis-Z</param>
        /// <param name="scaleX">Scale axis-X</param>
        /// <param name="scaleY">Scale axis-Y</param>
        /// <param name="scaleZ">Scale axis-Z</param>
        public Mesh(double x, double y, double z, double rotationX, double rotationY, double rotationZ, double scaleX, double scaleY, double scaleZ)
        {
            // Set mesh X position to parameter X.
            this._x = x;
            // Set mesh Y position to parameter Y.
            this._y = y;
            // Set mesh Z position to parameter Z.
            this._z = z;

            // Set mesh rotation X position to parameter rotation X.
            this._rX = rotationX;
            // Set mesh rotation Y position to parameter rotation Y.
            this._rY = rotationY;
            // Set mesh rotation Z position to parameter rotation Z.
            this._rZ = rotationZ;

            // Set mesh scale X position to parameter scale X.
            this._sX = scaleX;
            // Set mesh scale Y position to parameter scale Y.
            this._sY = scaleY;
            // Set mesh scale Z position to parameter scale Z.
            this._sZ = scaleZ;
        }
        /// <summary>
        /// Empty mesh constructor.
        /// </summary>
        public Mesh()
        {
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Move function.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        public virtual void Move(double x, double y, double z)
        {
            // Set axis-X.
            this._x = x;
            // Set axis-Y.
            this._y = y;
            // Set axis-Z.
            this._z = z;

            // Set needsUpdate.
            needsUpdate = true;
        }

        /// <summary>
        /// Rotate function.
        /// </summary>
        /// <param name="rotationX">Rotate axis-X</param>
        /// <param name="rotationY">Rotate axis-Y</param>
        /// <param name="rotationZ">Rotate axis-Z</param>
        public virtual void Rotate(double rotationX, double rotationY, double rotationZ)
        {
            // Set rotation axis-X.
            this._rX = rotationX;
            // Set rotation axis-Y.
            this._rY = rotationY;
            // Set rotation axis-Z.
            this._rZ = rotationZ;

            // Set needsUpdate.
            needsUpdate = true;
        }

        /// <summary>
        /// Update function.
        /// </summary>
        /// <param name="tick">Tick timer</param>
        /// <returns>True or false</returns>
        public virtual bool Update(int tick)
        {
            // Check if update is needed.
            if (needsUpdate)
            {
                // Set update to false.
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

