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
        // Boolean for robot in loading deck.
        public static bool RobotInLoadingDeckArea = false;
        // Boolean for racks to unload is set.
        private bool RacksToUnloadSet = false;
        #endregion

        #region Properties
        /// <summary>
        /// Creating grid map.
        /// </summary>
        public Dictionary<string, Node> Map { get; private set; }
        /// <summary>
        /// All the places where you can put racks.
        /// </summary>
        public Dictionary<int, PlaceInfo> RackPlaces { get; private set; }
        /// <summary>
        /// All the wait places for the robot.
        /// </summary>
        public Dictionary<int, PlaceInfo> RobotWaitPlaces { get; private set; }
        /// <summary>
        /// All the positions of the robots.
        /// </summary>
        public Dictionary<int, string> RobotPositions { get; private set; }

        /// <summary>
        /// List of all robots
        /// </summary>
        public static List<Robot> AllRobots { get; private set; }
        /// <summary>
        /// List of all racks.
        /// </summary>
        public static List<Rack> AllRacks { get; private set; }
        /// <summary>
        /// Get guid of robot in loading deck.
        /// </summary>
        public static Guid RobotInLoadingDeckGuid { get; private set; }
        /// <summary>
        /// Check amount of racks to unload.
        /// </summary>
        private int RacksToUnload { get; set; }

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
            this.RackPlaces = new Dictionary<int, PlaceInfo>();
            // Creating dictionary of all the robot wait places.
            this.RobotWaitPlaces = new Dictionary<int, PlaceInfo>();
            // Creating dictionary of all robot positions.
            this.RobotPositions = new Dictionary<int, string>();

            // Creating a list of all robots.
            AllRobots = new List<Robot>();
            // Creating a list of all racks.
            AllRacks = new List<Rack>();
            // Set amount of racks to unload.
            this.RacksToUnload = -1;

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
                // Check if crane status is free.
                if (_Crane._CraneState == CraneState.free)
                    // Check if loading deck is free.
                    if (this._LoadingDeck == LoadingDeck.free)
                    {
                        // Crane is loading the boat container.
                        _Crane.Load("boat", _Boat.guid);
                        // Start a new thread with a delay.
                        Thread t = new Thread(new ThreadStart(CraneStartLoadingVehicle));
                        t.Start();
                    }
            }

            // Check if loading deck is loading and crane status is loading.
            if (this._LoadingDeck == LoadingDeck.isLoading && _Crane._CraneState == CraneState.loading)
            {
                this.UnLoadingContainer();
            }

            // Check if loading deck is unloading and crane status is loading.
            if (this._LoadingDeck == LoadingDeck.isUnloading && _Crane._CraneState == CraneState.loading)
            {
                // Unload crane.
                _Crane.Unload();
                // Start a new thread with a delay.
                Thread t = new Thread(new ThreadStart(CraneStartUnLoading));
                t.Start();
            }

            // Create robot in waiting places to list.
            var fullWaitingPlaces = RobotWaitPlaces.Where(x => x.Value.HasMeshOnIt == true).ToList();

            // Get all data.
            var data = AllRobots.Where(x => x.isWaiting && x.z != 100 &&
                fullWaitingPlaces.Where(j => j.Value.HasMeshOnIt == true && j.Value.guid == x.guid).ToList().Count == 0
            ).ToList();

            // Check if there is data.
            if (data.Count > 0)
            {
                // Get all robots in data.
                foreach (Robot robot in data)
                {
                    // Set waiting position.
                    var waitingPosition = FreeWaitingPostion();

                    // Check if waiting position is not empty.
                    if (waitingPosition != "")
                    {
                        // Move robot.
                        robot.Move(g.shortest_path(robot.Position, waitingPosition), waitingPosition);

                        // Set waiting position update.
                        var waitingPositionupdate = RobotWaitPlaces.Where(x => x.Value.Coord == waitingPosition).ToList().First();

                        // Set robot on waiting position true.
                        waitingPositionupdate.Value.HasMeshOnIt = true;
                        // Set robot on waiting position unique id.
                        waitingPositionupdate.Value.guid = robot.guid;
                    }
                }
            }
        }

        /// <summary>
        /// Return robot waiting position coordinates.
        /// </summary>
        /// <returns>Coordinates or empty</returns>
        private string FreeWaitingPostion()
        {
            // Loop through robot wait places.
            for (int i = 0; i < RobotWaitPlaces.Count; i++)
            {
                // Set place index to count.
                int placeIndex = i;
                // Check if count + 1 is higher than robot places count / 2.
                if ((i + 1) > (RobotWaitPlaces.Count() / 2))
                    // Set placeindex.
                    placeIndex = (RobotWaitPlaces.Count - i) + (RobotWaitPlaces.Count() / 2) - 1;

                // Check if robot wait places has no robot on it.
                if (!RobotWaitPlaces[placeIndex].HasMeshOnIt)
                    // Return coordinates of wait places.
                    return RobotWaitPlaces[placeIndex].Coord;

            }
            // Return null.
            return "";
        }

        /// <summary>
        /// Start loading.
        /// </summary>
        private void CraneStartLoadingVehicle()
        {
            // Wait for 13 seconds.
            Thread.Sleep(13000);
            // Set loading deck to loading.
            this._LoadingDeck = LoadingDeck.isLoading;
        }

        /// <summary>
        /// Start unloading.
        /// </summary>
        private void CraneStartUnLoading()
        {
            // Wait for 13 seconds.
            Thread.Sleep(13000);
            // Set loading deck to free.
            this._LoadingDeck = LoadingDeck.free;
            
            // Check if crane vehicle is truck.
            if (_Crane.vehicle == "truck")
                // Move truck away from crane.
                _Truck.MoveAwayFromCrane();

            // Check if crane vehicle is boat.
            if (_Crane.vehicle == "boat")
                // Move truck away from crane.
                _Boat.MoveAwayFromCrane();
        }

        /// <summary>
        /// Unload racks from container with robots.
        /// </summary>
        private void UnLoadingContainer()
        {
            // Check if vehicle is a truck and racks to unload is not set.
            if (_Crane.vehicle == "truck" && !RacksToUnloadSet)
            {
                // Racks to unload have been set.
                RacksToUnloadSet = true;
                // Racks to unload is amount of racks the truck has.
                RacksToUnload = _Truck.NumberOfRacksLoaded;
            }
            // Check if vehicle is boat and racks to unload is not set.
            else if (_Crane.vehicle == "boat" && !RacksToUnloadSet)
            {
                // Racks to unload have been set.
                RacksToUnloadSet = true;
                // Racks to unload is amount of racks the boat has.
                RacksToUnload = _Boat.NumberOfRacksLoaded;
            }

            // If amount of racks that need to be unloaded is higher than 0.
            if (RacksToUnload > 0)
            {
                // Get all waiting robots.
                var waitingRobots = RobotWaitPlaces
               .Where(x => x.Value.HasMeshOnIt == true).ToList();

                // Check if robot is not in loading deck area.
                if (!RobotInLoadingDeckArea)
                {
                    // Check if amount of waiting robots is higher than 0.
                    if (waitingRobots.Count > 0)
                    {
                        // Set data to list.
                        var data = AllRobots.Where(x => x.guid == waitingRobots.Last().Value.guid).ToList();
                        // Check if there is data.
                        if (data.Count > 0)
                        {
                            // Set robot.
                            Robot robot = data.First();
                            // Set the robot unique id into the loading deck.
                            RobotInLoadingDeckGuid = robot.guid;
                            // Move the robot.
                            robot.Move(g.shortest_path(robot.Position, StartPoint), StartPoint);
                            // Set that there is a robot in the loading deck.
                            RobotInLoadingDeckArea = true;
                            // Set that the robot waiting spot has been emptied.
                            waitingRobots.Last().Value.HasMeshOnIt = false;
                        }
                    }
                }
                else
                {
                    // Get robot data.
                    var data = AllRobots.Where(x => x.guid == RobotInLoadingDeckGuid && x.CurrentPos == StartPoint).ToList();
                    // Check if there is data.
                    if (data.Count > 0)
                    {
                        // Set robot.
                        Robot robot = data.First();
                        // Check if robot doesn't have a rack.
                        if (!robot.hasRack)
                        {
                            // Get all places for the racks.
                            var places = RackPlaces
                           .Where(x => x.Value.HasMeshOnIt == false)
                           .OrderBy(x => _Random.Next()).ToList();

                            // Check if there is a spot open for the racks.
                            if (places.Count > 0)
                            {
                                // Create a rack.
                                Rack rack = World.CreateRack();

                                // Move the robot with the rack to the empty place.
                                robot.Move(g.shortest_path(robot.Position, places.First().Value.Coord), places.First().Value.Coord, rack.guid);

                                // Set the empty spot to taken.
                                places.First().Value.HasMeshOnIt = true;
                                // Set the unique id of the rack on the spot.
                                places.First().Value.guid = rack.guid;
                                // Remove one item of the racks that need to be unloaded.
                                this.RacksToUnload--;
                            }
                        }
                    }
                }
            }
            else
            {
                // Check if all racks is lower than 8.
                if (AllRacks.Count < 8)
                {
                    // Racks to unload have not been set.
                    RacksToUnloadSet = false;
                    // Set loading deck to unloading.
                    this._LoadingDeck = LoadingDeck.isUnloading;
                }
                else
                    // Loading container.
                    LoadingContainer();
            }
        }

        /// <summary>
        /// Load a container with racks.
        /// </summary>
        private void LoadingContainer()
        {
            Console.WriteLine("Here");
        }
        #endregion
    }
}
