using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    interface IRobotTask
    {
        void StartTask(Robot r);
        bool TaskComplete(Robot r);
    }
}
