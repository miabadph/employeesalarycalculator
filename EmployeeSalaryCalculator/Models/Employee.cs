using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeSalaryCalculator.Common;

namespace EmployeeSalaryCalculator.Models
{
    public abstract class Employee
    {
        #region Properties
        public Guid Id { get; set; }

        public int EmployeeId { get; set; }

        public string Name { get; set; }

        public string TIN { get; set; }

        public DateTime BirthDate { get; set; }

        public EmployeeType EmployeeType { get; set; }
        #endregion

        #region Methods
        public abstract double CalculateSalary();
        #endregion
    }
}
