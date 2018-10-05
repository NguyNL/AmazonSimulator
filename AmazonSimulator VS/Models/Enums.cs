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
    }

    public enum LoadingDeck
    {
        free,
        isLoading,
        isUnloading
    }
}
