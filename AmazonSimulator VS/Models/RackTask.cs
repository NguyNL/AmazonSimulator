using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RackTask : ITask<Rack>
    {
        private bool startupComplete = false;
        private bool complete = false;

        private Node[] path;

        public RackTask(Node[] path) => this.path = path;

        public void StartTask(Rack r) => r.MoveOverPath(this.path);

        public bool TaskComplete(Rack r) => path.Length == 0;

        public void RemovePath()
        {
            List<Node> pathList = path.ToList();
            pathList.RemoveAt(0);
            path = pathList.ToArray();
        }
    }
}
