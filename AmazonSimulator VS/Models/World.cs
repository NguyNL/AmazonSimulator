using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Controllers;
using Microsoft.EntityFrameworkCore.Internal;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        static private List<Mesh> worldObjects = new List<Mesh>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        private static Manager manager;

        public World()
        {
            manager = new Manager(
                CreateTruck(),
                CreateBoat(),
                CreateCrane()
            );

            for (int i = 0; i < 4; i++)
                CreateRobot();

            Manager.Doors = CreateDoors();

            LoadGridMap();

            RobotsToWaitPosition();

            //Manager.AllRobots[0].Move(manager.g.shortest_path(Manager.AllRobots[0].Position, "101"), "101");
            //Manager.AllRobots[1].Move(manager.g.shortest_path(Manager.AllRobots[1].Position, "102"), "102");
            //Manager.AllRobots[2].Move(manager.g.shortest_path(Manager.AllRobots[2].Position, "102"), "102");


            //Manager.AllRobots[0].Move(manager.g.shortest_path(Manager.AllRobots[0].Position, "07"), "07");
            ////Manager.AllRobots[1].Move(manager.g.shortest_path(Manager.AllRobots[1].Position, "07"), "07");
            //Manager.AllRobots[2].Move(manager.g.shortest_path(Manager.AllRobots[2].Position, "07"), "07");
        }

        private void RobotsToWaitPosition()
        {
            var WaitingPlaces = manager.RobotWaitPlaces
               .Where(x => x.Value.HasMeshOnIt == false).ToList();

            for (int i = 0; i < Manager.AllRobots.Count(); i++)
            {
                int placeIndex = i;
                if ((i + 1) > (WaitingPlaces.Count() / 2))
                {
                    placeIndex = (WaitingPlaces.Count - i) + (WaitingPlaces.Count() / 2) - 1;
                }

                Manager.AllRobots[i].Move(manager.g.shortest_path(Manager.AllRobots[i].Position, WaitingPlaces[placeIndex].Value.Coord), WaitingPlaces[placeIndex].Value.Coord);
                WaitingPlaces[placeIndex].Value.HasMeshOnIt = true;
                WaitingPlaces[placeIndex].Value.guid = Manager.AllRobots[i].guid;
            }

        }

        private void LoadGridMap()
        {
            int stepX = 25;
            int stepZ = 25;

            int rows = 11;
            int columns = 9;

            int breaksAfterLoadDeck = 5;

            int rackPlaceIndex = 0;
            int robotPlaceIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                // Dont start adding before break is done
                if (row != 0 && row < (breaksAfterLoadDeck - 1)) continue;

                for (int column = 0; column < columns; column++)
                {
                    int center = (int)Math.Floor((decimal)(columns / 2));

                    // current key
                    string key = row.ToString() + column.ToString();

                    // keep track of it need a point
                    bool needsPoint = false;

                    // tempory path points where you can go to from this point
                    Dictionary<string, Node> vertexDictonary = new Dictionary<string, Node>();

                    // Add the position to the map
                    manager.Map.Add(key, new Node
                    {
                        x = column * stepX,
                        y = 0,
                        z = row * stepZ
                    });

                    if (row == (breaksAfterLoadDeck - 1))
                    {
                        if (center != column && (column < center - 1 || column > center + 1))
                        {

                            // Add new robot waiting position
                            manager.RobotWaitPlaces.Add(
                                robotPlaceIndex,
                                new PlaceInfo
                                {
                                    HasMeshOnIt = false,
                                    Coord = key
                                });

                            robotPlaceIndex++;

                            vertexDictonary.Add(row.ToString() + center.ToString(), new Node
                            {
                                x = center * stepX,
                                y = 0,
                                z = row * stepZ
                            });

                            needsPoint = true;
                        }

                        if (center == column)
                        {
                            // Add all paths on both sides of the middle point
                            for (int i = 0; i < columns; i++)
                                if (i != column && (i < center - 1 || i > center + 1))
                                    vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                    {
                                        x = i * stepX,
                                        y = 0,
                                        z = row * stepZ
                                    });

                            // Add behind doors point
                            vertexDictonary.Add("04", new Node
                            {
                                x = 4 * stepX,
                                y = 0,
                                z = 0
                            });

                            needsPoint = true;
                        }
                    }
                    else
                    {
                        // Middle point
                        if (Math.Floor((decimal)(columns / 2)) == column)
                        {
                            // Load deck path
                            if (row == 0)
                            {
                                // Add all the paths down path
                                for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                    vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                    {
                                        x = column * stepX,
                                        y = 0,
                                        z = i * stepZ
                                    });

                                // Add begin point
                                vertexDictonary.Add(Manager.StartPoint, Manager.StartPointNode);

                                // Add Waiting Places robots
                                vertexDictonary.Add((breaksAfterLoadDeck - 1).ToString() + column.ToString(), new Node
                                {
                                    x = column * stepX,
                                    y = 0,
                                    z = (breaksAfterLoadDeck - 1) * stepZ
                                });

                                needsPoint = true;
                            }
                            else
                                if (row % 2 == breaksAfterLoadDeck % 2)
                            {
                                // Add behind doors point
                                vertexDictonary.Add("04", new Node
                                {
                                    x = 4 * stepX,
                                    y = 0,
                                    z = 0
                                });

                                // Add Waiting Places robots
                                vertexDictonary.Add((breaksAfterLoadDeck - 1).ToString() + column.ToString(), new Node
                                {
                                    x = column * stepX,
                                    y = 0,
                                    z = (breaksAfterLoadDeck - 1) * stepZ
                                });

                                // Add all paths on both sides of the middle point
                                for (int i = 0; i < columns; i++)
                                    if (i != column)
                                        vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                        {
                                            x = i * stepX,
                                            y = 0,
                                            z = row * stepZ
                                        });

                                // Add all paths from bottom - top middle point
                                for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                    if (i != row)
                                        vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                        {
                                            x = column * stepX,
                                            y = 0,
                                            z = i * stepZ
                                        });

                                needsPoint = true;
                            }
                            // Not a middle point
                        }
                        else
                        {
                            // StartPoint
                            if (row == Manager.StartPointCoord[0] && column == Manager.StartPointCoord[1])
                            {
                                // Add behind doors point tot the begin point
                                vertexDictonary.Add("04", new Node
                                {
                                    x = 4 * stepX,
                                    y = 0,
                                    z = 0
                                });

                                needsPoint = true;
                            }

                            // Rack row
                            if (row != 0 && row % 2 != breaksAfterLoadDeck % 2)
                            {
                                // Add only 2X3 rack places per even row
                                if ((column > 0 && column < 4) || (column > 4 && column < 8))
                                {
                                    // Make connection to upper row
                                    vertexDictonary.Add((row - 1).ToString() + column.ToString(), new Node
                                    {
                                        x = column * stepX,
                                        y = 0,
                                        z = (row - 1) * stepZ
                                    });

                                    // Add new rack position
                                    manager.RackPlaces.Add(
                                        rackPlaceIndex,
                                        new PlaceInfo
                                        {
                                            HasMeshOnIt = false,
                                            Coord = key
                                        });

                                    // rack index up
                                    rackPlaceIndex++;

                                    needsPoint = true;
                                }


                            }// Normal row
                            else if (row != 0 && row % 2 == breaksAfterLoadDeck % 2)
                            {
                                // Get the corners
                                if (column == 0 || column == (columns - 1))
                                {
                                    // get all the corners under it
                                    for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                        if (i != row)
                                            vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                            {
                                                x = column * stepX,
                                                y = 0,
                                                z = i * stepZ
                                            });

                                    // Get all the points on same row
                                    for (int i = 0; i < columns; i++)
                                        if (i != column)
                                            vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                            {
                                                x = i * stepX,
                                                y = 0,
                                                z = row * stepZ
                                            });
                                }
                                // Get the points between corners and middle
                                else
                                {
                                    // Get all the points on same row
                                    for (int i = 0; i < columns; i++)
                                        if (i != column)
                                            vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                            {
                                                x = i * stepX,
                                                y = 0,
                                                z = row * stepZ
                                            });

                                    if ((column > 0 && column < 4) || (column > 4 && column < 8))
                                        // Add rack position under it
                                        vertexDictonary.Add((row + 1).ToString() + column.ToString(), new Node
                                        {
                                            x = column * stepX,
                                            y = 0,
                                            z = (row + 1) * stepZ
                                        });
                                }

                                needsPoint = true;
                            }
                        }
                    }

                    if (needsPoint)
                        manager.g.add_vertex(key, vertexDictonary);
                }
            }
            //Thread t = new Thread(new ThreadStart(startFilling));
            //t.Start();
        }

        //public void startFilling()
        //{
        //    foreach (KeyValuePair<int, RackPlace> coord in manager.RackPlaces)
        //    {
        //        Robot r = CreateRobot();
        //        Rack ra = CreateRack();

        //        worldObjects.Add(r);
        //        worldObjects.Add(ra);

        //        r.Move(g.shortest_path(r.Position, coord.Value.Coord), coord.Value.Coord);
        //        ra.Move(g.shortest_path(ra.Position, coord.Value.Coord), coord.Value.Coord);

        //        if (coord.Key == 2)
        //            break;

        //        Thread.Sleep(10000);
        //    }

        //    manager.AllRobots[0].Move(g.shortest_path(manager.AllRobots[0].Position, "07"), "07");
        //    manager.AllRacks[0].Move(g.shortest_path(manager.AllRacks[0].Position, "07"), "07");

        //    manager.AllRobots[1].Delete();
        //}

        private Robot CreateRobot(double x, double y, double z)
        {
            Robot robot = new Robot(x, y, z, 0, 0, 0);
            worldObjects.Add(robot);
            Manager.AllRobots.Add(robot);
            return robot;
        }

        private Robot CreateRobot()
        {
            Robot robot = new Robot();
            worldObjects.Add(robot);
            Manager.AllRobots.Add(robot);
            return robot;
        }

        private Rack CreateRack(double x, double y, double z)
        {
            Rack rack = new Rack(x, y, z, 0, 0, 0);
            worldObjects.Add(rack);
            Manager.AllRacks.Add(rack);
            return rack;
        }

        public static Rack CreateRack()
        {
            Rack rack = new Rack();
            worldObjects.Add(rack);
            Manager.AllRacks.Add(rack);
            return rack;
        }

        private Truck CreateTruck(double x, double y, double z)
        {
            Truck truck = new Truck(x, y, z, 0, 0, 0);
            worldObjects.Add(truck);
            return truck;
        }

        private Truck CreateTruck()
        {
            Truck truck = new Truck();
            worldObjects.Add(truck);
            return truck;
        }

        private Boat CreateBoat(double x, double y, double z)
        {
            Boat boat = new Boat(x, y, z, 0, 0, 0);
            worldObjects.Add(boat);
            return boat;
        }

        private Boat CreateBoat()
        {
            Boat boat = new Boat();
            worldObjects.Add(boat);
            return boat;
        }

        private Crane CreateCrane()
        {
            Crane crane = new Crane();
            worldObjects.Add(crane);
            return crane;
        }

        private LoadDeckDoors CreateDoors()
        {
            LoadDeckDoors doors = new LoadDeckDoors();
            worldObjects.Add(doors);
            return doors;
        }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Mesh m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d, m3d.action));

                if (m3d.action == "delete")
                    worldObjects.Remove(m3d);
            }
        }

        public bool Update(int tick)
        {
            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            GC.WaitForPendingFinalizers();

            manager.Start();

            for (int i = 0; i < worldObjects.Count; i++)
            {
                Mesh u = worldObjects[i];

                if (u is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if (needsCommand)
                    {
                        SendCommandToObservers(new UpdateModel3DCommand(u, u.action));

                        if (u.action == "delete")
                            worldObjects.Remove(u);
                    }
                }
            }

            return true;
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}