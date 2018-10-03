using System;

namespace Models
{
    public class Box : Mesh, IUpdatable
    {
        private Guid ParentGuid { get; set; }

        private double ScaleX { get; set; }
        private double ScaleY { get; set; }
        private double ScaleZ { get; set; }

        public Box(double x, double y, double z, double rotationX, double rotationY, double rotationZ, double scaleX, double scaleY, double scaleZ, Guid parentGuid) : base(x, y, z, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ)
        {
            this.guid = Guid.NewGuid();
            this.type = "box";
            this.rotationX = 180 * Math.PI / 180;

            this.ParentGuid = parentGuid;
        }
    }
}