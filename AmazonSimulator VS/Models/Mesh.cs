using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Mesh : IUpdatable {
        private int _ID;

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
        private double _rX = 0;
        private double _rY = 0;
        private double _rZ = 0;

        public int ID { get { return _ID; } protected set { _ID = value; } }
        public string type { get; protected set; }
        public Guid guid { get; protected set; }
        public double x { get { return _x; } protected set { _x = value; } }
        public double y { get { return _y; } protected set { _y = value; } }
        public double z { get { return _z; } protected set { _z = value; } }
        public double rotationX { get { return _rX; } protected set { _rX = value; } }
        public double rotationY { get { return _rY; } protected set { _rY = value; } }
        public double rotationZ { get { return _rZ; } protected set { _rZ = value; } }

        public bool needsUpdate = true;

        public Mesh(double x, double y, double z, double rotationX, double rotationY, double rotationZ, int ID)
        {
            this.guid = Guid.NewGuid();
            this._ID = ID;

            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;
        }

        public Mesh(int ID)
        {
            this._ID = ID;
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

