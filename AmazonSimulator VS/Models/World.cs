using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Microsoft.EntityFrameworkCore.Internal;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Robot> worldObjects = new List<Robot>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private Dictionary<string, Node> Map = new Dictionary<string, Node>();
        private Dictionary<int, string> RackPositions = new Dictionary<int, string>();
        //private List<Node> nodeList = new List<Node>();


        public World() {
            //Robot r = CreateRobot(0,0,0);
            //Robot r1 = CreateRobot(0, 0, 20);
            //r.Move(4.6, 0, 13);

            Path();
        }

        private void Path()
        {
            Graph g = new Graph();
            int stepX = 17;
            int stepZ = 20;

            int rows = 13;
            int columns = 9;

            int breaksAfterLoadDeck = 7;

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
                            if (row % 2 != 0)
                            {
                                // Add all paths on both sides of the middle point
                                for (int i = 0; i < columns; i++)
                                    if (i != column)
                                        vertexDictonary.Add(row.ToString() + i.ToString(), new Node
                                        {
                                            x = i * stepX,
                                            y = 0,
                                            z = row * stepZ
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
                        if (row != 0 && row % 2 == 0)
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
                        else if (row != 0 && row % 2 != 0)
                        {
                            // Get the corners
                            if (column == 0 || column == (columns-1))
                            {
                                // get all the corners under it
                                for (int i = (row + 2); i < rows; i = i + 2)
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

            Robot rr = CreateRobot(0);
            rr.Move(g.shortest_path("04", "111"));
        }

        private Robot CreateRobot(double x, double y, double z, int ID) {
            Robot r = new Robot(x,y,z,0,0,0, ID);
            worldObjects.Add(r);
            return r;
        }

        private Robot CreateRobot(int ID)
        {
            Robot r = new Robot(ID);
            worldObjects.Add(r);
            return r;
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
            foreach(Robot m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for(int i = 0; i < worldObjects.Count; i++) {
                Robot u = worldObjects[i];

                if(u is IUpdatable) {
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