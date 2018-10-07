using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class Manager
    {
        #region Variables
        // Randomizer.
        Random _Random = new Random();

        // Loading deck doors.
        static public LoadDeckDoors Doors;

        // Grid start point.
        public static readonly string StartPoint = "07";
        // Grid start point coordinates.
        public static readonly int[] StartPointCoord = { 0, 7 };
        // Grid start point pixels.
        public static readonly Node StartPointNode = new Node { x = 175, y = 0, z = 0 };
        #endregion

        #region Properties
        /// <summary>
        /// Creating grid map.
        /// </summary>
        public Dictionary<string, Node> Map { get; private set; }
        /// <summary>
        /// All the places where you can put racks.
        /// </summary>
        public Dictionary<int, RackPlace> RackPlaces { get; private set; }
        /// <summary>
        /// All the positions of the robots.
        /// </summary>
        public Dictionary<int, string> RobotPositions { get; private set; }

        // List of all robots.
        public static List<Robot> AllRobots { get; private set; }
        // List of all racks.
        public static List<Rack> AllRacks { get; private set; }

        // Truck.
        public Truck _Truck { get; private set; }
        // Boat.
        public Boat _Boat { get; private set; }
        // Crane.
        public Crane _Crane { get; private set; }

        // Loading deck.
        private LoadingDeck _LoadingDeck { get; set; }

        // Vehicle.
        private string Vehicle { get; set; }

        // Dijkstra's method.
        public Graph g { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Manager constructor.
        /// </summary>
        /// <param name="truck">Truck object</param>
        /// <param name="boat">Boat object</param>
        /// <param name="crane">Crane object</param>
        public Manager(Truck truck, Boat boat, Crane crane)
        {
            // Creating the grid map.
            this.Map = new Dictionary<string, Node>();
            // Creating dictionary of all the rack places.
            this.RackPlaces = new Dictionary<int, RackPlace>();
            // Creating dictionary of all robot positions.
            this.RobotPositions = new Dictionary<int, string>();

            // Creating a list of all robots.
            AllRobots = new List<Robot>();
            // Creating a list of all racks.
            AllRacks = new List<Rack>();

            // Setting the truck object.
            this._Truck = truck;
            // Setting the boat object.
            this._Boat = boat;
            // Setting the crane object.
            this._Crane = crane;

            // Calling the Dijkstra's method.
            this.g = new Graph();

            // Setting loading deck to status free.
            this._LoadingDeck = LoadingDeck.free;
        }
        #endregion

        #region Methods

        public void Start()
        {
            // Check if boat is created.
            if(_Boat.Position == Transport.created)
                // Move boat to crane.
                _Boat.MoveToCrane();

            // Check if truck is created.
            if(_Truck.Position == Transport.created)
                // Move truck to crane.
                _Truck.MoveToCrane();

            // Check is truck is at loading deck.
            if (_Truck.Position == Transport.loadingDeck)
            {
                // Check if crane status is free.
                if(_Crane._CraneState == CraneState.free)
                    // Check if loading deck is free.
                    if (this._LoadingDeck == LoadingDeck.free)
                    {
                        // Crane is loading the truck container.
                        _Crane.Load("truck", _Truck.guid);
                        // Start a new thread with a delay.
                        Thread t = new Thread(new ThreadStart(CraneStartLoadingVehicle));
                        t.Start();
                    }
            }
            // Check if boat is at loading deck.
            if (_Boat.Position == Transport.loadingDeck)
            {
                // Create random amount of racks for the next truck.
                _Boat.NumberOfRacksLoaded = _Random.Next(1, 4);
                // Check if loading deck is free.
                if (this._LoadingDeck == LoadingDeck.free)
                    // Set loading deck to is loading.
                    this._LoadingDeck = LoadingDeck.isLoading;
            }

            // If loading deck is loading and crane status is loading.
            if (this._LoadingDeck == LoadingDeck.isLoading && _Crane._CraneState == CraneState.loading)
            {
                //this.MoveRobots();
                this._LoadingDeck = LoadingDeck.isUnloading;
            }
        }

        private void CraneStartLoadingVehicle()
        {
            Thread.Sleep(13000);
            this._LoadingDeck = LoadingDeck.isLoading;
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
