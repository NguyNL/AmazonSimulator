using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class Manager
    {
        #region Properties

        public Dictionary<string, Node> Map { get; private set; }
        public Dictionary<int, RackPlace> RackPlaces { get; private set; }
        public Dictionary<int, string> RobotPositions { get; private set; }

        static public LoadDeckDoors Doors;

        // Grid starting points
        public static readonly string StartPoint = "07";
        public static readonly int[] StartPointCoord = { 0 , 7 };
        public static readonly Node StartPointNode = new Node { x = 175, y = 0, z = 0 };

        public static List<Robot> AllRobots { get; private set; }
        public static List<Rack> AllRacks { get; private set; }

        public Truck _Truck { get; private set; }
        public Boat _Boat { get; private set; }
        public Crane _Crane { get; private set; }

        Random _Random = new Random();

        private LoadingDeck LoadingDeck { get; set; }

        private string Vehicle { get; set; }

        public Graph g { get; set; }
        #endregion

        #region Constructors
        public Manager(Truck truck, Boat boat, Crane crane)
        {
            this.Map = new Dictionary<string, Node>();
            this.RackPlaces = new Dictionary<int, RackPlace>();
            this.RobotPositions = new Dictionary<int, string>();

            AllRobots = new List<Robot>();
            AllRacks = new List<Rack>();

            this._Truck = truck;
            this._Boat = boat;
            this._Crane = crane;

            this.g = new Graph();

            this.LoadingDeck = LoadingDeck.free;
        }
        #endregion

        #region Methods

        public void Start()
        {
            if(_Boat.Position == Transport.created)
                _Boat.MoveToCrane();

            if(_Truck.Position == Transport.created)
                _Truck.MoveToCrane();

            if (_Truck.Position == Transport.loadingDeck)
            {
                if(_Crane._CraneState == CraneState.free)
                    if (this.LoadingDeck == LoadingDeck.free)
                    {
                        _Crane.Load("truck", _Truck.guid);
                        Thread t = new Thread(new ThreadStart(CraneStartLoadingVehicle));
                        t.Start();
                    }
            }

            if (_Boat.Position == Transport.loadingDeck)
            {
                _Boat.NumberOfRacksLoaded = _Random.Next(1, 4);

                if (this.LoadingDeck == LoadingDeck.free)
                    this.LoadingDeck = LoadingDeck.isLoading;
            }

            if (this.LoadingDeck == LoadingDeck.isLoading && _Crane._CraneState == CraneState.loading)
            {
                this.MoveRobots();
                this.LoadingDeck = LoadingDeck.isUnloading;
            }
        }

        private void CraneStartLoadingVehicle()
        {
            Thread.Sleep(13000);
            this.LoadingDeck = LoadingDeck.isLoading;
        }

        private void MoveRobots()
        {
            var places = RackPlaces
                .Where(x => x.Value.HasRackOnIt == false)
                .OrderBy(x => _Random.Next()).ToList();

            int amount = _Truck.NumberOfRacksLoaded;

            for(int i = 0; i < amount; i++)
            {
                if(AllRobots[i].Position != StartPoint)
                {
                    AllRobots[i].Move(g.shortest_path(AllRobots[i].Position, StartPoint), StartPoint);
                }

                Robot robot = AllRobots[i];
                Rack rack = World.CreateRack();

                AllRobots[i].Move(g.shortest_path(AllRobots[i].Position, places[i].Value.Coord), places[i].Value.Coord);

                rack.Move(g.shortest_path(rack.Position, places[i].Value.Coord), places[i].Value.Coord);
            }
        }
        #endregion
    }
}
