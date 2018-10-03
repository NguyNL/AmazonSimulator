using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Controllers;
using Microsoft.EntityFrameworkCore.Internal;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Object> worldObjects = new List<Object>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private Dictionary<string, Node> Map = new Dictionary<string, Node>();
        private Dictionary<int, string> RackPositions = new Dictionary<int, string>();
        private Dictionary<int, string> RobotPositions = new Dictionary<int, string>();
        static public LoadDeckDoors Doors;
        private Graph g { get; set; }
        private List<Robot> allRobots = new List<Robot>();
        private List<Rack> allRacks = new List<Rack>();
        //private List<Node> nodeList = new List<Node>();


        public World() {
            Doors = CreateDoors();

            Path();
        }

        private void Path()
        {
            g = new Graph();
            int stepX = 25;
            int stepZ = 25;

            int rows = 11;
            int columns = 9;

            int breaksAfterLoadDeck = 5;

            int rackPlaceIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                // Dont start adding before break is done
                if (row != 0 && row < breaksAfterLoadDeck) continue;

                for (int column = 0; column < columns; column++)
                {
                    // current key
                    string key = row.ToString() + column.ToString();

                    // keep track of it need a point
                    bool needsPoint = false;

                    // tempory path points where you can go to from this point
                    Dictionary<string, Node> vertexDictonary = new Dictionary<string, Node>();

                    // Add the position to the map
                    Map.Add(key, new Node {
                        x = column * stepX,
                        y = 0,
                        z = row * stepZ
                    });

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
                            vertexDictonary.Add("07", new Node
                            {
                                x = 7 * stepX,
                                y = 0,
                                z = 0
                            });

                            needsPoint = true;
                        }
                        else
                            if (row % 2 == breaksAfterLoadDeck % 2)
                            {
                                // Add begin point
                                vertexDictonary.Add("04", new Node
                                {
                                    x = 4 * stepX,
                                    y = 0,
                                    z = 0
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
                    } else
                    {
                        // StartPoint
                        if (row == 0 && column == 7)
                        {
                            // Add starting point
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
                                RackPositions.Add(rackPlaceIndex, key);

                                // rack index up
                                rackPlaceIndex++;

                                needsPoint = true;
                            }


                        }// Normal row
                        else if (row != 0 && row % 2 == breaksAfterLoadDeck % 2)
                        {
                            // Get the corners
                            if (column == 0 || column == (columns-1))
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
                    
                    if(needsPoint)
                        g.add_vertex(key, vertexDictonary);
                }
            }

            Thread t = new Thread(new ThreadStart(startFilling));
            t.Start();
        }

        public void startFilling()
        {
            foreach (KeyValuePair<int, string> coord in RackPositions)
            {
                Robot r = CreateRobot();
                Rack ra = CreateRack();

                worldObjects.Add(r);
                worldObjects.Add(ra);

                r.Move(g.shortest_path(r.Position, coord.Value), coord.Value);
                ra.Move(g.shortest_path(ra.Position, coord.Value), coord.Value);

                Thread.Sleep(6000);
            }

            allRobots[0].Move(g.shortest_path(allRobots[0].Position, "07"), "07");
            allRacks[0].Move(g.shortest_path(allRacks[0].Position, "07"), "07");
        }

        private Robot CreateRobot(double x, double y, double z) {
            Robot robot = new Robot(x,y,z,0,0,0);
            worldObjects.Add(robot);
            allRobots.Add(robot);
            return robot;
        }

        private Robot CreateRobot()
        {
            Robot robot = new Robot();
            worldObjects.Add(robot);
            allRobots.Add(robot);
            return robot;
        }

        private Rack CreateRack(double x, double y, double z)
        {
            Rack rack = new Rack(x, y, z, 0, 0, 0);
            worldObjects.Add(rack);
            allRacks.Add(rack);
            return rack;
        }

        private Rack CreateRack()
        {
            Rack rack = new Rack();
            worldObjects.Add(rack);
            allRacks.Add(rack);
            return rack;
        }

        private LoadDeckDoors CreateDoors()
        {
            LoadDeckDoors doors = new LoadDeckDoors();
            worldObjects.Add(doors);
            return doors;
        }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(Object m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for(int i = 0; i < worldObjects.Count; i++) {
                var u = worldObjects[i];
                
                if (u is IUpdatable) {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if(needsCommand) {
                        SendCommandToObservers(new UpdateModel3DCommand(u));
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