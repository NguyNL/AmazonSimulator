using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RobotTask : IRobotTask
    {
        private bool startupComplete = false;
        private bool complete = false;

        private Node[] path;

        public RobotTask(Node[] path)
        {
            this.path = path;
        }

        public void StartTask(Robot r)
        {
            r.MoveOverPath(this.path);
        }

        public bool TaskComplete(Robot r)
        {
            return path.Length == 0;
        }

        public void RemovePath()
        {
            List<Node> pathList = path.ToList();
            pathList.RemoveAt(0);
            path = pathList.ToArray();
        }
    }
}
