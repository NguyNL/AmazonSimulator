using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    interface ITask<T>
    {
        #region Methods
        /// <summary>
        /// Start task.
        /// </summary>
        /// <param name="r">Object</param>
        void StartTask(T r);

        /// <summary>
        /// Check if task has been completed.
        /// </summary>
        /// <param name="r">Object</param>
        /// <returns>True or false</returns>
        bool TaskComplete(T r);
        #endregion
    }
}