using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeSalaryCalculator.Models;

namespace EmployeeSalaryCalculator.Data
{
    public class EmployeeSalaryCalculatorContext : DbContext
    {
        public EmployeeSalaryCalculatorContext(DbContextOptions<EmployeeSalaryCalculatorContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeSalaryCalculator.Models.RegularEmployee> RegularEmployee { get; set; }

        public DbSet<EmployeeSalaryCalculator.Models.ContractualEmployee> ContractualEmployee { get; set; }

        public DbSet<EmployeeSalaryCalculator.Models.EmployeeSalary> EmployeeSalary { get; set; }
    }
}
