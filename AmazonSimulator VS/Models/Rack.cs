using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Rack : Mesh, IUpdatable
    {
        #region Variables
        // Create list of boxes.
        public List<Box> Boxes = new List<Box>();
        #endregion

        #region Constructors
        /// <summary>
        /// Rack constructor with custom values.
        /// </summary>
        /// <param name="x">Axis-X position</param>
        /// <param name="y">Axis-Y position</param>
        /// <param name="z">Axis-Z position</param>
        /// <param name="rotationX">Rotation axis-X</param>
        /// <param name="rotationY">Rotation axis-Y</param>
        /// <param name="rotationZ">Rotation axis-Z</param>
        public Rack(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            // Create new unique id.
            this.guid = Guid.NewGuid();
            // Set type to rack.
            this.type = "rack";
            // Set rotation X.
            this.rotationX = 180 * Math.PI / 180;
            // Fill the rack with boxes.
            FillRack();
            // Set update to true.
            needsUpdate = true;
        }

        /// <summary>
        /// Rack constructor without pre-set values.
        /// </summary>
        public Rack()
        {
            // Create new unique id.
            this.guid = Guid.NewGuid();
            // Set type to rack.
            this.type = "rack";

            // Set axis-X position.
            this.x = Manager.StartPointNode.x;
            // Set axis-Y position.
            this.y = Manager.StartPointNode.y;
            // Set axis-Z position.
            this.z = Manager.StartPointNode.z;

            // Set rotation axis-X.
            this.rotationX = 180 * Math.PI / 180;
            // Set rotation axis-Y.
            this.rotationY = -90 * Math.PI / 180;
            // Set rotation axis-Z.
            this.rotationZ = 0;

            // Fill the rack with boxes.
            FillRack();
            // Set update to true.
            needsUpdate = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill the rack with boxes.
        /// </summary>
        private void FillRack()
        {
            // Rack Floors Y positions  floor:[1,2,3,4]
            double[] floorY = { -0.86, -0.3035, 0.2552, 0.8058 };

            // startpoint, endpoint, currentpoint
            double[] cordX = { -0.3, 0.3, -0.3 };
            double[] cordZ = { 0.3, -0.38, 0.3 };

            // Small [size, scale], medium [size,scale], big[size, scale]
            double[] boxSizes = { 0.25, 0.25, 0.32 };
            double[,] boxSizeScale = {
                {0.01, 0.01, 0.01},
                {0.01, 0.015, 0.01},
                {0.013, 0.01, 0.013}
            };

            // Create randomizer.
            Random rnd = new Random();

            // Fill floors on a rack
            for (var i = 0; i < floorY.Length; i++)
            {
                cordZ[2] = cordZ[0];
                cordX[2] = cordX[0];

                // Max 9 Cardboard boxes
                for (var j = 0; j < 9; j++)
                {
                    // Box index is random.
                    int boxIndex = rnd.Next(0, 3);

                    // Check if box index is 2.
                    if (boxIndex == 2)
                    {
                        // Set X coordinates.
                        cordX[2] += 0.04;
                        cordZ[2] -= 0.02;
                    }

                    // Check if coordinate X of box 2 is higher than coordinate X of box 1.
                    if (cordX[2] > cordX[1])
                    {
                        // Check if coordinate Z of box 2 - box size is lower than coordinates Z box 1.
                        if ((cordZ[2] - boxSizes[boxIndex]) < cordZ[1]) break;

                        // Set Z coordinates.
                        cordZ[2] -= boxSizes[boxIndex];
                        cordX[2] = cordX[0];

                        // Check if box index is 2.
                        if (boxIndex == 2)
                        {
                            // Set Z coordinates.
                            cordX[2] += 0.04;
                            cordZ[2] -= 0.02;
                        }
                    }

                    // Create new box.
                    var newBox = new Box(cordX[2], floorY[i], cordZ[2], rnd.Next(0, 50) / 1000, 0, 0, boxSizeScale[boxIndex, 0], boxSizeScale[boxIndex, 1], boxSizeScale[boxIndex, 2], this.guid);

                    // Set X coordinates.
                    cordX[2] += boxSizes[boxIndex];

                    // Check if index of box is 2.
                    if (boxIndex == 2)
                    {
                        // Set Z coordinates.
                        cordZ[2] -= 0.06;
                    }

                    // Add boxes to list.
                    Boxes.Add(newBox);
                }
            }
        }

        /// <summary>
        /// Move rack over path.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="z">Axis-Z</param>
        /// <param name="rotationY">Roation axis-Y</param>
        public void MoveOverPath(double x, double z, double rotationY)
        {
            // Set rack axis-X.
            this.x = x;
            // Set rack axis-Z.
            this.z = z;
            // Set rack rotation axis-Y.
            this.rotationY = -rotationY;

            needsUpdate = true;
        }

        /// <summary>
        /// Delete rack.
        /// </summary>
        public void Delete()
        {
            // Set rack action to delete.
            this.action = "delete";
        }

        /// <summary>
        /// Update function for the boat.
        /// </summary>
        /// <param name="tick">Tick speed</param>
        /// <returns>True or False.</returns>
        public override bool Update(int tick)
        {
            // Check if rack action is deleting.
            if (this.action == "delete")
            {
                // Return true.
                return true;
            }

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