using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    interface ITask<T>
    {
        void StartTask(T r);
        bool TaskComplete(T r);
    }
}
