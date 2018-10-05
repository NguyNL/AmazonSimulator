using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Manager
    {
        public Dictionary<string, Node> Map { get; private set; }
        public Dictionary<int, RackPlace> RackPlaces { get; private set; }
        public Dictionary<int, string> RobotPositions { get; private set; }

        static public LoadDeckDoors Doors;

        // Grid starting points
        public static readonly string StartPoint = "07";
        public static readonly int[] StartPointCoord = { 0 , 7 };
        public static readonly Node StartPointNode = new Node { x = 175, y = 0, z = 0 };

        public List<Robot> AllRobots { get; private set; }
        public List<Rack> AllRacks { get; private set; }

        public Truck Truck { get; private set; }
        public Boat Boat { get; private set; }

        private LoadingDeck LoadingDeck { get; set; }

        public Manager(Truck truck, Boat boat)
        {
            this.Map = new Dictionary<string, Node>();
            this.RackPlaces = new Dictionary<int, RackPlace>();
            this.RobotPositions = new Dictionary<int, string>();

            this.AllRobots = new List<Robot>();
            this.AllRacks = new List<Rack>();

            this.Truck = truck;
            this.Boat = boat;

            this.LoadingDeck = LoadingDeck.free;
        }

        public void Start()
        {
            if(Boat.Position == Transport.created)
                Boat.MoveToCrane();

            if (Truck.Position == Transport.created)
                Truck.MoveToCrane();


        }
    }
}
