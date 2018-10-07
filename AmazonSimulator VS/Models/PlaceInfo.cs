using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class PlaceInfo
    {
        #region Properties
        /// <summary>
        /// Get and set if a place is taken by rack.
        /// </summary>
        public bool HasMeshOnIt { get; set; }
        /// <summary>
        /// Get and set all coordinates of empty rack spots.
        /// </summary>
        public string Coord { get; set; }
        /// <summary>
        /// Get and set unique id.
        /// </summary>
        public Guid guid { get; set; }
        #endregion
    }
}