using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace HelloCore.DatabaseAccess {
   public class HelloCoreDbContext : DbContext {
       public HelloCoreDbContext(DbContextOptions<HelloCoreDbContext> options) : base(options) {
       }
    }
}
