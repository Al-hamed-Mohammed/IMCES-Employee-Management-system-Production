using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager2.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
               new Employee
               {
                   Id = 1,
                   Name = "Harley",
                   Department = Dept.IT,
                   Email = "mary@gmail.com"
               },

                new Employee
                {
                    Id = 2,
                    Name = "Denzel",
                    Department = Dept.Billing,
                    Email = "Denzel@gmail.com"
                }
           );
        }
    }
}
