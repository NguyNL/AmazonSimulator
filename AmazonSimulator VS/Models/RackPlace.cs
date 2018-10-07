using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RackPlace
    {
        #region Properties
        /// <summary>
        /// Get and set if a place is taken by rack.
        /// </summary>
        public bool HasRackOnIt { get; set; }
        /// <summary>
        /// Get and set all coordinates of empty rack spots.
        /// </summary>
        public string Coord { get; set; }
        #endregion
    }
}
