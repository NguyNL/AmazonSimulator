using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
   public enum Transport
    {
        created,
        toLoadingDeck,
        loadingDeck,
        fromLoadingDeck,
        done
    }

    public enum LoadingDeck
    {
        free,
        isLoading,
        isUnloading
    }

    public enum CraneState
    {
        free,
        loading,
        unloading,
        done
    }
}
