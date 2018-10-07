using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    #region Enums
    /// <summary>
    /// Transport statuses.
    /// </summary>
    public enum Transport
    {
        // Transport vehicle created.
        created,
        // Transport vehicle going to loading deck.
        toLoadingDeck,
        // Transport vehicle at loading deck.
        loadingDeck,
        // Transport vehicle driving away from loading deck.
        fromLoadingDeck,
        // Transport vehicle finished.
        finish
    }

    /// <summary>
    /// Loading deck statuses.
    /// </summary>
    public enum LoadingDeck
    {
        // Loading deck is free.
        free,
        // Loading deck is loading.
        isLoading,
        // Loading deck is unloading.
        isUnloading,
        // Loading deck is finished.
        finish
    }

    /// <summary>
    /// Cranestate statuses.
    /// </summary>
    public enum CraneState
    {
        // Crane is free.
        free,
        // Crane is loading.
        loading,
        // Crane is unloading.
        unloading,
        // Crane finished.
        finish
    }
    #endregion
}