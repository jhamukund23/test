using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dss.Domain.Models.MRM
{
    public class AddDocument
    {
        public DateTimeOffset StartsOn { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }

    }
}
