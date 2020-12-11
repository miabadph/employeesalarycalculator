using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeSalaryCalculator.Models
{
    public class RegularEmployee : Employee
    {
        public double BasicSalary { get; set; }

        public override double CalculateSalary()
        {
            return BasicSalary;
        }
    }
}
