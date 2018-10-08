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
        #region Variables
        // List of meshes in the world.
        static private List<Mesh> worldObjects = new List<Mesh>();
        // List of observers.
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        // Calling the manager class.
        private static Manager manager;
        #endregion

        #region Constructors
        /// <summary>
        /// World constructor.
        /// </summary>
        public World()
        {
            // Create new manager.
            manager = new Manager(
                // Create truck.
                CreateTruck(),
                // Create boat.
                CreateBoat(),
                // Create crane.
                CreateCrane()
            );

            // Create 4 robots.
            for (int i = 0; i < 4; i++)
                CreateRobot();

            // Create doors.
            Manager.Doors = CreateDoors();

            // Load grid map.
            LoadGridMap();

            // Set robots to wait position.
            RobotsToWaitPosition();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Move robots to waiting positions.
        /// </summary>
        private void RobotsToWaitPosition()
        {
            // Get list of waiting places.
            var WaitingPlaces = manager.RobotWaitPlaces
               .Where(x => x.Value.HasMeshOnIt == false).ToList();

            for (int i = 0; i < Manager.AllRobots.Count(); i++)
            {
                // Set place index.
                int placeIndex = i;
                if ((i + 1) > (WaitingPlaces.Count() / 2))
                {
                    // Set place index.
                    placeIndex = (WaitingPlaces.Count - i) + (WaitingPlaces.Count() / 2) - 1;
                }
                // Move robot to waiting place.
                Manager.AllRobots[i].Move(manager.g.shortest_path(Manager.AllRobots[i].Position, WaitingPlaces[placeIndex].Value.Coord), WaitingPlaces[placeIndex].Value.Coord);
                // Set waiting place to taken.
                WaitingPlaces[placeIndex].Value.HasMeshOnIt = true;
                // Set unique id on the waiting place.
                WaitingPlaces[placeIndex].Value.guid = Manager.AllRobots[i].guid;
            }

        }

        /// <summary>
        /// Create a grid for the racks and paths.
        /// </summary>
        private void LoadGridMap()
        {
            // Set X for each step.
            int stepX = 25;
            // Set Z for each step.
            int stepZ = 25;

            // Set amount of rows.
            int rows = 11;
            // Set amount of columns.
            int columns = 9;

            // Set amount of breaks after loading deck.
            int breaksAfterLoadDeck = 5;

            // Set rack place index.
            int rackPlaceIndex = 0;
            // Set robot place index.
            int robotPlaceIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                // Don't start adding before break is done.
                if (row != 0 && row < (breaksAfterLoadDeck - 1)) continue;

                for (int column = 0; column < columns; column++)
                {
                    // Get center.
                    int center = (int)Math.Floor((decimal)(columns / 2));

                    // Current key.
                    string key = row.ToString() + column.ToString();

                    // Keep track of it need a point.
                    bool needsPoint = false;

                    // Temporary path points where you can go to from this point.
                    Dictionary<string, Node> vertexDictonary = new Dictionary<string, Node>();

                    // Add the position to the map.
                    manager.Map.Add(key, new Node
                    {
                        x = column * stepX,
                        y = 0,
                        z = row * stepZ
                    });
                    // Add robot waiting position.
                    if (row == (breaksAfterLoadDeck - 1))
                    {
                        // Check if the column isn't at center.
                        if (center != column && (column < center - 1 || column > center + 1))
                        {

                            // Add new robot waiting position,
                            manager.RobotWaitPlaces.Add(
                                robotPlaceIndex,
                                new PlaceInfo
                                {
                                    HasMeshOnIt = false,
                                    Coord = key
                                });

                            robotPlaceIndex++;

                            // Add coordinates into the vertex dictionary.
                            vertexDictonary.Add(row.ToString() + center.ToString(), new Node
                            {
                                x = center * stepX,
                                y = 0,
                                z = row * stepZ
                            });

                            // Set needs point.
                            needsPoint = true;
                        }

                        // Check if center is column.
                        if (center == column)
                        {
                            // Add all paths on both sides of the middle point.
                            for (int i = 0; i < columns; i++)
                                if (i != column && (i < center - 1 || i > center + 1))
                                    // Add coordinates to vertex dictionary.
                                    vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                    {
                                        x = i * stepX,
                                        y = 0,
                                        z = row * stepZ
                                    });

                            // Add behind doors point.
                            vertexDictonary.Add("04", new Node
                            {
                                x = 4 * stepX,
                                y = 0,
                                z = 0
                            });

                            // Add all the paths down path
                            for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                {
                                    x = column * stepX,
                                    y = 0,
                                    z = i * stepZ
                                });

                            // Set needs point.
                            needsPoint = true;
                        }
                    }
                    else
                    {
                        // Middle point.
                        if (Math.Floor((decimal)(columns / 2)) == column)
                        {
                            // Load deck path.
                            if (row == 0)
                            {
                                // Add all the paths down path.
                                for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                    // Add coordinates to vertex dictionary.
                                    vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                    {
                                        x = column * stepX,
                                        y = 0,
                                        z = i * stepZ
                                    });

                                // Add begin point.
                                vertexDictonary.Add(Manager.StartPoint, Manager.StartPointNode);

                                // Add Waiting Places robots.
                                vertexDictonary.Add((breaksAfterLoadDeck - 1).ToString() + column.ToString(), new Node
                                {
                                    x = column * stepX,
                                    y = 0,
                                    z = (breaksAfterLoadDeck - 1) * stepZ
                                });

                                // Set needs point.
                                needsPoint = true;
                            }
                            else
                            {
                                if (row % 2 == breaksAfterLoadDeck % 2)
                                {
                                    // Add behind doors point.
                                    vertexDictonary.Add("04", new Node
                                    {
                                        x = 4 * stepX,
                                        y = 0,
                                        z = 0
                                    });

                                    // Add Waiting Places robots.
                                    vertexDictonary.Add((breaksAfterLoadDeck - 1).ToString() + column.ToString(), new Node
                                    {
                                        x = column * stepX,
                                        y = 0,
                                        z = (breaksAfterLoadDeck - 1) * stepZ
                                    });

                                    // Add all paths on both sides of the middle point.
                                    for (int i = 0; i < columns; i++)
                                        if (i != column)
                                            // Add coordinates to vertex dictionary.
                                            vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                            {
                                                x = i * stepX,
                                                y = 0,
                                                z = row * stepZ
                                            });

                                    // Add all paths from bottom - top middle point.
                                    for (int i = breaksAfterLoadDeck; i < rows; i = i + 2)
                                        if (i != row)
                                            // Add coordinates to vertex dictionary.
                                            vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                            {
                                                x = column * stepX,
                                                y = 0,
                                                z = i * stepZ
                                            });

                                    // Set needs point.
                                    needsPoint = true;
                                }
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

                                // Set needs point.
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

                                    // Set needs point.
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
                                            // Add coordinates to vertex dictionary.
                                            vertexDictonary.Add(i.ToString() + column.ToString(), new Node
                                            {
                                                x = column * stepX,
                                                y = 0,
                                                z = i * stepZ
                                            });

                                    // Get all the points on same row
                                    for (int i = 0; i < columns; i++)
                                        if (i != column)
                                            // Add coordinates to vertex dictionary.
                                            vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                            {
                                                x = i * stepX,
                                                y = 0,
                                                z = row * stepZ
                                            });
                                }
                                else // Get the points between corners and middle
                                {
                                    // Get all the points on same row
                                    for (int i = 0; i < columns; i++)
                                        if (i != column)
                                            // Add coordinates to vertex dictionary.
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
                                // Set needs point.
                                needsPoint = true;
                            }
                        }
                    }

                    if (needsPoint)
                        manager.g.add_vertex(key, vertexDictonary);
                }
            }
        }

        /// <summary>
        /// Create a robot with custom position values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <returns>Object robot</returns>
        private Robot CreateRobot(double x, double y, double z)
        {
            // Create new robot object.
            Robot robot = new Robot(x, y, z, 0, 0, 0);
            // Add robot to the world.
            worldObjects.Add(robot);
            // Add robot to the robots list.
            Manager.AllRobots.Add(robot);
            // Return robot object.
            return robot;
        }

        /// <summary>
        /// Create a robot.
        /// </summary>
        /// <returns>Object robot</returns>
        private Robot CreateRobot()
        {
            // Create new robot object.
            Robot robot = new Robot();
            // Add robot to the world.
            worldObjects.Add(robot);
            // Add robot to the robots list.
            Manager.AllRobots.Add(robot);
            // Return robot object.
            return robot;
        }

        /// <summary>
        /// Create a rack with custom position values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <returns>Object rack</returns>
        private Rack CreateRack(double x, double y, double z)
        {
            // Create new rack object.
            Rack rack = new Rack(x, y, z, 0, 0, 0);
            // Add rack to the world.
            worldObjects.Add(rack);
            // Add rack to the robots list.
            Manager.AllRacks.Add(rack);
            // Return rack object.
            return rack;
        }

        /// <summary>
        /// Create a rack.
        /// </summary>
        /// <returns>Object rack</returns>
        public static Rack CreateRack()
        {
            // Create new rack object.
            Rack rack = new Rack();
            // Add rack to the world.
            worldObjects.Add(rack);
            // Add rack to the robots list.
            Manager.AllRacks.Add(rack);
            // Return rack object.
            return rack;
        }

        /// <summary>
        /// Create a truck with custom position values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <returns>Object truck</returns>
        private Truck CreateTruck(double x, double y, double z)
        {
            // Create new truck object.
            Truck truck = new Truck(x, y, z, 0, 0, 0);
            // Add truck to the world.
            worldObjects.Add(truck);
            // Return truck object.
            return truck;
        }

        /// <summary>
        /// Create a truck.
        /// </summary>
        /// <returns>Object truck</returns>
        private Truck CreateTruck()
        {
            // Create new truck object.
            Truck truck = new Truck();
            // Add truck to the world.
            worldObjects.Add(truck);
            // Return truck object.
            return truck;
        }

        /// <summary>
        /// Create a boat with custom position values.
        /// </summary>
        /// <param name="x">Axis-X</param>
        /// <param name="y">Axis-Y</param>
        /// <param name="z">Axis-Z</param>
        /// <returns>Object boat</returns>
        private Boat CreateBoat(double x, double y, double z)
        {
            // Create new boat object.
            Boat boat = new Boat(x, y, z, 0, 0, 0);
            // Add boat to the world.
            worldObjects.Add(boat);
            // Return boat object.
            return boat;
        }

        /// <summary>
        /// Create a boat.
        /// </summary>
        /// <returns>Object boat</returns>
        private Boat CreateBoat()
        {
            // Create new boat object.
            Boat boat = new Boat();
            // Add boat to the world.
            worldObjects.Add(boat);
            // Return boat object.
            return boat;
        }

        /// <summary>
        /// Create a crane.
        /// </summary>
        /// <returns>Object crane</returns>
        private Crane CreateCrane()
        {
            // Create new crane object.
            Crane crane = new Crane();
            // Add crane to the world.
            worldObjects.Add(crane);
            // Return crane object.
            return crane;
        }

        /// <summary>
        /// Create a door.
        /// </summary>
        /// <returns>Object door</returns>
        private LoadDeckDoors CreateDoors()
        {
            // Create new door object.
            LoadDeckDoors doors = new LoadDeckDoors();
            // Add door to the world.
            worldObjects.Add(doors);
            // Return door object.
            return doors;
        }

        /// <summary>
        /// Server.
        /// </summary>
        /// <param name="observer">Observer</param>
        /// <returns>Unsubscriber class</returns>
        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        /// <summary>
        /// Send command to server.
        /// </summary>
        /// <param name="c">Command</param>
        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        /// <summary>
        /// Send creation command to server.
        /// </summary>
        /// <param name="obs">Iobserver command</param>
        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Mesh m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d, m3d.action));

                if (m3d.action == "delete")
                    worldObjects.Remove(m3d);
            }
        }

        /// <summary>
        /// Update function.
        /// </summary>
        /// <param name="tick">Tick timer</param>
        /// <returns>True or false.</returns>
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
        #endregion
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