using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager2.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAllEmployee();
        IEnumerable<Employee> SearchwithdateAndName(string txt, string from, string to);
        IEnumerable<Employee> SearchEmployee(string txt);
        IEnumerable<Employee> SearchReceipt(string from, string to);
        Employee Add(Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int id);
    }
}