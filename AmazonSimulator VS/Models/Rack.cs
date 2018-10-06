using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Rack : Mesh, IUpdatable
    {
        private List<RackTask> Tasks = new List<RackTask>();
        public List<Box> Boxes = new List<Box>();
        private double Speed = 1;
        public string Position { get; private set; }
        public string CurrentPos { get; private set; }
        private bool InRotationAnimation = false;
        private bool FirstMovement = true;

        public Rack(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.guid = Guid.NewGuid();
            this.type = "rack";
            this.Position = Manager.StartPoint;
            this.CurrentPos = Manager.StartPoint;

            this.rotationX = 180 * Math.PI / 180;

            FillRack();
        }

        public Rack()
        {
            this.guid = Guid.NewGuid();
            this.type = "rack";
            this.Position = Manager.StartPoint;
            this.CurrentPos = Manager.StartPoint;

            this.x = Manager.StartPointNode.x;
            this.y = Manager.StartPointNode.y;
            this.z = Manager.StartPointNode.z;

            this.rotationX = 180 * Math.PI / 180;
            this.rotationY = 90 * Math.PI / 180;
            this.rotationZ = 0;

            FillRack();
        }

        private void FillRack()
        {
            // RACKS & BOXES
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

            Random rnd = new Random();

            // Fill floors on a rack
            for (var i = 0; i < floorY.Length; i++)
            {
                cordZ[2] = cordZ[0];
                cordX[2] = cordX[0];

                // Max 9 Cardboard boxes
                for (var j = 0; j < 9; j++)
                {
                    int boxIndex = rnd.Next(0,3);

                    if (boxIndex == 2)
                    {
                        cordX[2] += 0.04;
                        cordZ[2] -= 0.02;
                    }

                    if (cordX[2] > cordX[1])
                    {
                        if ((cordZ[2] - boxSizes[boxIndex]) < cordZ[1]) break;

                        cordZ[2] -= boxSizes[boxIndex];
                        cordX[2] = cordX[0];

                        if (boxIndex == 2)
                        {
                            cordX[2] += 0.04;
                            cordZ[2] -= 0.02;
                        }
                    }

                    var newBox = new Box(cordX[2], floorY[i], cordZ[2], rnd.Next(0, 50) / 1000, 0, 0, boxSizeScale[boxIndex,0], boxSizeScale[boxIndex, 1], boxSizeScale[boxIndex, 2], this.guid);
                    
                    //newBox.position.y = floorY[i];
                    //newBox.position.x = cordX[2];
                    //newBox.position.z = cordZ[2];

                    //newBox.rotation.y = Math.round(Math.random() * 50) / 1000;

                    //newBox.scale.set(boxSizes[boxIndex][1][0], boxSizes[boxIndex][1][1], boxSizes[boxIndex][1][2]);

                    cordX[2] += boxSizes[boxIndex];

                    if (boxIndex == 2)
                    {
                        cordZ[2] -= 0.06;
                    }

                    Boxes.Add(newBox);
                    //rack.add(newobject);
                    //boxes.push(newobject);
                }
            }
        }

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
}
}
