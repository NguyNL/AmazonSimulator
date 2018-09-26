using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : Mesh, IUpdatable {
        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x,y,z,rotationX,rotationY,rotationZ) {
            
        }

        public void Move(string xz)
        {
            char[] positions = xz.ToCharArray();

            this.x = ((double)positions[0] - 48) *20;
            this.z = ((double)positions[1] - 48) *20;

            needsUpdate = true;
        }
    }
}