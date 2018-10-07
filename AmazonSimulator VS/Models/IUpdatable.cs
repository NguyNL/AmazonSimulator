using System;
using System.Collections.Generic;
using System.Linq;

namespace Models {
    interface IUpdatable
    {
        #region Methods
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="tick">Tick time</param>
        /// <returns>True or false</returns>
        bool Update(int tick);
        #endregion
    }
}
