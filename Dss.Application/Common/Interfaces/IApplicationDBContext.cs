using Dss.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dss.Application.Common.Interfaces
{
    public interface IApplicationDBContext
    {
        public DbSet<Patient> patients { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
