using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeSalaryCalculator.Common;

namespace EmployeeSalaryCalculator.Models
{
    public class EmployeeSalary
    {
        public Guid Id { get; set; }

        public int EmployeeId { get; set; }

        public double DailyPay { get; set; }

        public double TotalPay { get; set; }

        public double Absences { get; set; }

        public double Tax { get; set; }

        public double WorkDays { get; set; }

        public EmployeeType EmployeeType { get; set; }

        public DateTime Date { get; set; }
    }

    public class SalaryCalculationInfo
    {
        public int EmployeeId { get; set; }

        public double Absences { get; set; }

        public double WorkDays { get; set; }

        public DateTime CalculationDate { get; set; }
    }
}
