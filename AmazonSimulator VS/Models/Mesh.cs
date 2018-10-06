using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Mesh : IUpdatable {
        #region Properties
        // 
        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
        private double _rX = 0;
        private double _rY = 0;
        private double _rZ = 0;
        private double _sX = 0;
        private double _sY = 0;
        private double _sZ = 0;

        private string _action = "update";

        public string type { get; protected set; }
        public Guid guid { get; protected set; }
        public double x { get { return _x; } protected set { _x = value; } }
        public double y { get { return _y; } protected set { _y = value; } }
        public double z { get { return _z; } protected set { _z = value; } }
        public double rotationX { get { return _rX; } protected set { _rX = value; } }
        public double rotationY { get { return _rY; } protected set { _rY = value; } }
        public double rotationZ { get { return _rZ; } protected set { _rZ = value; } }
        public double scaleX { get { return _sX; } protected set { _sX = value; } }
        public double scaleY { get { return _sY; } protected set { _sY = value; } }
        public double scaleZ { get { return _sZ; } protected set { _sZ = value; } }

        public bool needsUpdate = true;
        public string action { get { return _action; } protected set { _action = value; } }
        #endregion

        public Mesh(double x, double y, double z, double rotationX, double rotationY, double rotationZ)
        {
            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;
        }

        public Mesh(double x, double y, double z, double rotationX, double rotationY, double rotationZ, double scaleX, double scaleY, double scaleZ)
        {
            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            this._sX = scaleX;
            this._sY = scaleY;
            this._sZ = scaleZ;
        }

        public Mesh()
        {
            
        }

        public virtual void Move(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;

            needsUpdate = true;
        }
       

        public virtual void Rotate(double rotationX, double rotationY, double rotationZ)
        {
            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            needsUpdate = true;
        }

        public virtual bool Update(int tick)
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}

