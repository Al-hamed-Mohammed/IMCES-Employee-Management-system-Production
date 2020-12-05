using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager2.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;
        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Employee Add(Employee employee)
        {
            context.Employee.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = context.Employee.Find(id);
            if (employee != null)
            {
                context.Employee.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employee;
        }
        public IEnumerable<Employee> SearchEmployee(string txt)
        {
            var emp = from m in context.Employee
                         select m;
            emp = emp.Where(s => s.Name.Contains(txt));

            return emp;



        }
        public IEnumerable<Employee> SearchwithdateAndName(string txt, string from,string to )
        {
            var emp = from m in context.Employee
                      select m;
            emp = emp.Where(s => s.ReceiptDate >= Convert.ToDateTime(from) && s.ReceiptDate <= Convert.ToDateTime(to) && s.Name.Contains(txt));

            return emp;

        }
        public IEnumerable<Employee> SearchReceipt(string from,string to)
        {
            var emp = from m in context.Employee
                      select m;
            emp = emp.Where(s => s.ReceiptDate >= Convert.ToDateTime(from)  && s.ReceiptDate <= Convert.ToDateTime(to));

            return emp;



        }

        public Employee GetEmployee(int Id)
        {
           return context.Employee.Find(Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = context.Employee.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}
