using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public enum UserActivityPredicates
    {
        InPast = 1,
        InFuture = 0,
        IsHosting = 2,
    }
}