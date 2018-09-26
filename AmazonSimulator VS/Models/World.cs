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
            int x = 0;
            int z = 0;

            int stepX = 17;
            int stepZ = 20;

            //for (int row = 0; row < 8; row++)
            //{
            //    for (int column = 0; column < 9; column++)
            //    {
            //        if (column == 0 || column == 9)
            //            g.add_vertex(
            //                row.ToString() + column.ToString(),
            //                new Dictionary<string, Node>() {
            //                    { "B",  new Node {
            //                            x = column * stepX,
            //                            y = 0,
            //                            z = row * stepZ
            //                        }
            //                    }
            //                });
            //    }
            //}

            Robot rr = CreateRobot(0);

            g.add_vertex("03", new Dictionary<string, Node>() {
                { "73", new Node{ x = 68, z = 160 } },
                { "93",  new Node{ x = 68, z = 200 } },
                { "113",  new Node{ x = 68, z = 240 } }
            });

            g.add_vertex("04", new Dictionary<string, Node>() {
                { "03", new Node{ x = 68, z = 0 } }
            });

            g.add_vertex("70", new Dictionary<string, Node>() {
                { "120", new Node{ x = 0, z = 240 } }
            });

            g.add_vertex("120", new Dictionary<string, Node>() {});

            g.add_vertex("73", new Dictionary<string, Node>() {
                { "70", new Node{ x = 0, z = 160 } }
            });

            g.add_vertex("93", new Dictionary<string, Node>() {});
            g.add_vertex("113", new Dictionary<string, Node>() { });

            //g.shortest_path("04", "03").ForEach(
            //        xy => Console.WriteLine(xy)
            //    );

            rr.Move(g.shortest_path("04", "120"));

            //g.shortest_path("04", "120").ForEach(
            //       zx => rr.Move(zx)
            //   );

            //g.add_vertex("B", new Dictionary<string, Node>() { { "A", 7 }, { "F", 2 } });
            //g.add_vertex("C", new Dictionary<string, Node>() { { "A", 8 }, { "F", 6 }, { "G", 4 } });
            //g.add_vertex("D", new Dictionary<string, Node>() { { "F", 8 } });
            //g.add_vertex("E", new Dictionary<string, Node>() { { "H", 1 } });
            //g.add_vertex("F", new Dictionary<string, Node>() { { "B", 2 }, { "C", 6 }, { "D", 8 }, { 'G', 9 }, { 'H', 3 } });
            //g.add_vertex("G", new Dictionary<string, Node>() { { "C", 4 }, { "F", 9 } });
            //g.add_vertex("H", new Dictionary<string, Node>() { { "E", 1 }, { "F", 3 } });

            //g.shortest_path('A', 'H')
            //    .ForEach(
            //        x => Console.WriteLine(x)
            //    );
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