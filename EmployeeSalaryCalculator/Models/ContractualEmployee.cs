using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeSalaryCalculator.Models
{
    public class ContractualEmployee : Employee
    {
        public double DailySalary { get; set; }

        public override double CalculateSalary()
        {
            return DailySalary;
        }
    }
}
