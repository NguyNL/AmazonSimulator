using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Rack : Mesh, IUpdatable
    {
        #region Variables
        // Create list of tasks for rack.
        private List<RackTask> Tasks = new List<RackTask>();
        // Create list of boxes for rack.
        public List<Box> Boxes = new List<Box>();
        // Set speed.
        private double Speed = 1;
        // Set bool for rotation animation.
        private bool InRotationAnimation = false;
        // Set bool for first movement.
        private bool FirstMovement = true;
        #endregion

        #region Properties
        /// <summary>
        /// Get and set position.
        /// </summary>
        public string Position { get; private set; }
        /// <summary>
        /// Get and set current position.
        /// </summary>
        public string CurrentPos { get; private set; }
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
            // Set start position.
            this.Position = Manager.StartPoint;
            // Set current position.
            this.CurrentPos = Manager.StartPoint;
            // Set rotation X.
            this.rotationX = 180 * Math.PI / 180;
            // Fill the rack with boxes.
            FillRack();
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
            // Set position.
            this.Position = Manager.StartPoint;
            // Set current position.
            this.CurrentPos = Manager.StartPoint;

            // Set axis-X position.
            this.x = Manager.StartPointNode.x;
            // Set axis-Y position.
            this.y = Manager.StartPointNode.y;
            // Set axis-Z position.
            this.z = Manager.StartPointNode.z;

            // Set rotation axis-X.
            this.rotationX = 180 * Math.PI / 180;
            // Set rotation axis-Y.
            this.rotationY = 90 * Math.PI / 180;
            // Set rotation axis-Z.
            this.rotationZ = 0;

            // Fill the rack with boxes.
            FillRack();
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
                    int boxIndex = rnd.Next(0,3);

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
                    var newBox = new Box(cordX[2], floorY[i], cordZ[2], rnd.Next(0, 50) / 1000, 0, 0, boxSizeScale[boxIndex,0], boxSizeScale[boxIndex, 1], boxSizeScale[boxIndex, 2], this.guid);

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
        /// <param name="path">All the paths</param>
        public void MoveOverPath(Node[] path)
        {
            if (FirstMovement)
                CheckRotationPosition(path[0]);

            if (!InRotationAnimation)
            {
                bool AllowedToMove = true;
                int CurrentRow = (int)Math.Floor(this.z / 25);
                int CurrentColumn = (int)Math.Floor(this.x / 25);

                if ((CurrentRow.ToString() + CurrentColumn.ToString()) != CurrentPos)
                {
                    CurrentPos = CurrentRow.ToString() + CurrentColumn.ToString();
                }

                List<Rack> RackList = Manager.AllRacks
                    .Where(
                        rack => rack.CurrentPos == this.CurrentPos
                    ).ToList();

                if (RackList.Count > 0)
                    if (RackList[0].guid != this.guid)
                        AllowedToMove = false;

                if (AllowedToMove)
                {
                    int NextRow = path.First().x == this.x ? path.First().z > this.z ? CurrentRow - 1 : CurrentRow + 1 : CurrentRow;
                    int NextColumn = path.First().z == this.z ? path.First().x > this.x ? CurrentColumn + 1 : CurrentColumn - 1 : CurrentColumn;

                    List<Rack> RackListNextStep = Manager.AllRacks
                        .Where(
                            rack => rack.CurrentPos == (NextRow.ToString() + NextColumn.ToString()) &&
                            rack.guid != this.guid
                        ).ToList();

                    if (RackListNextStep.Count > 0)
                        AllowedToMove = false;
                }

                if (AllowedToMove)
                {
                    this.z = path.First().x == this.x ?
                        path.First().z > this.z ?
                        this.z += Speed :
                        this.z -= Speed :
                        this.z;

                    this.x = path.First().z == this.z ?
                        path.First().x > this.x ?
                        this.x += Speed :
                        this.x -= Speed :
                        this.x;
                }
            }

            if (path.First().x == this.x && path.First().z == this.z)
            {
                if (path.Length > 1)
                    CheckRotationPosition(path[1]);
                else
                    FirstMovement = true;
            }

            if (path.First().x == this.x && path.First().z == this.z && !InRotationAnimation)
                Tasks.First().RemovePath();
        }

        private void CheckRotationPosition(Node node)
        {
            if (this.x > node.x && this.z == node.z)
                RotateObject(90);

            else if (this.x < node.x && this.z == node.z)
                RotateObject(-90);

            else if (this.z < node.z && this.x == node.x)
                RotateObject(0);

            else if (this.z > node.z && this.x == node.x)
                RotateObject(180);
        }

        public void RotateObject(int degrees)
        {
            int currentDegrees = (int)(this.rotationY / Math.PI * 180);

            if (currentDegrees < 0 && degrees == 180)
                degrees *= -1;

            if (currentDegrees >= 180 && degrees == -90)
                degrees = 270;

            if (currentDegrees != degrees)
            {
                InRotationAnimation = true;
                if (this.rotationY > (degrees * Math.PI / 180))
                {
                    this.rotationY -= 1 * Math.PI / 180;
                }
                else
                {
                    this.rotationY += 1 * Math.PI / 180;
                }
            }
            else
            {
                if (currentDegrees == -180)
                    this.rotationY = 180 * Math.PI / 180;

                if (currentDegrees == 270)
                    this.rotationY = -90 * Math.PI / 180;

                InRotationAnimation = false;
                FirstMovement = false;
            }
        }

        public void Move(Node[] path, string position)
        {
            Tasks.Add(new RackTask(path));
            this.Position = position;
        }


        public override bool Update(int tick)
        {
            //foreach (Box box in Boxes)
            //    box.Update(tick);

            if (Tasks.Count > 0)
            {
                if (Tasks.First().TaskComplete(this))
                    Tasks.RemoveAt(0);

                if (Tasks.Count > 0)
                    Tasks.First().StartTask(this);

                return true;
            }

            return false;
        }
        #endregion
    }
}
