using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager2.Models
{
    public interface EmployeeRepository
    {
        Employee GetEmployee(int Id);
    }
}
