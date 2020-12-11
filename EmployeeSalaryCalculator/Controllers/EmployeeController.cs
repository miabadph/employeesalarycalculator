using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeSalaryCalculator.Data;
using EmployeeSalaryCalculator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSalaryCalculator.Controllers
{
    [Route("api/SalaryCalculator")]
    [ApiController]
    public class EmployeeController : Controller
    {

        private readonly EmployeeSalaryCalculatorContext dbContext;

        public EmployeeController(EmployeeSalaryCalculatorContext context)
        {
            dbContext = context;
        }

        [HttpPost("AddRegularEmployee")]
        public async Task<IActionResult> CreateRegualr([Bind("BasicSalary,Id,EmployeeId,Name,BirthDate,TIN,EmployeeType")] RegularEmployee regularEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            regularEmployee.Id = Guid.NewGuid();
            regularEmployee.EmployeeId = generateEmployeeNumber();
            dbContext.Add(regularEmployee);
            await dbContext.SaveChangesAsync();

            return Ok(regularEmployee.EmployeeId);

        }

        [HttpPost("AddContractualEmployee")]
        public async Task<IActionResult> CreateContractual([Bind("DailySalary,Id,EmployeeId,Name,BirthDate,TIN,EmployeeType")] ContractualEmployee contractualEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            contractualEmployee.Id = Guid.NewGuid();
            contractualEmployee.EmployeeId = generateEmployeeNumber();
            dbContext.Add(contractualEmployee);
            await dbContext.SaveChangesAsync();

            return Ok(contractualEmployee.EmployeeId);
        }

        //To return JSON value
        [HttpPost("CalculateSalary")]
        public async Task<IActionResult> CalculateSalary(SalaryCalculationInfo input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var isRegular = dbContext.RegularEmployee.Where(r => r.EmployeeId == input.EmployeeId).Any();
            var isContractual = dbContext.ContractualEmployee.Where(r => r.EmployeeId == input.EmployeeId).Any();
            var employeeSalary = new EmployeeSalary();

            if (isRegular)
            {
                var regularEmployee = dbContext.RegularEmployee.FirstOrDefault(r => r.EmployeeId == input.EmployeeId);
                employeeSalary.Absences = input.Absences;
                employeeSalary.EmployeeId = input.EmployeeId;
                employeeSalary.EmployeeType = regularEmployee.EmployeeType;
                employeeSalary.Tax = (regularEmployee.BasicSalary * 0.12d);
                employeeSalary.Date = input.CalculationDate;
                employeeSalary.WorkDays = input.WorkDays;
                employeeSalary.DailyPay = regularEmployee.BasicSalary / input.WorkDays;
                employeeSalary.TotalPay = regularEmployee.BasicSalary - (employeeSalary.DailyPay * input.Absences) - employeeSalary.Tax;
            }

            if (isContractual)
            {
                var contractualEmployee = dbContext.ContractualEmployee.FirstOrDefault(r => r.EmployeeId == input.EmployeeId);
                employeeSalary.Absences = input.Absences;
                employeeSalary.EmployeeId = input.EmployeeId;
                employeeSalary.EmployeeType = contractualEmployee.EmployeeType;
                employeeSalary.Tax = 0.00d;
                employeeSalary.Date = input.CalculationDate;
                employeeSalary.WorkDays = input.WorkDays;
                employeeSalary.DailyPay = contractualEmployee.DailySalary;
                employeeSalary.TotalPay = contractualEmployee.DailySalary * (input.WorkDays - input.Absences);
            }

            employeeSalary.Id = Guid.NewGuid();
            dbContext.Add(employeeSalary);
            await dbContext.SaveChangesAsync();

            return Ok(employeeSalary);
        }

        [HttpGet("GetAllEmployeeIds")]
        public async Task<IActionResult> GetAllEmployeeIDsAsync()
        {
            var query = (from a in dbContext.RegularEmployee
                         select new { Id = a.EmployeeId, EmpType = a.EmployeeType })
                        .Concat(from b in dbContext.ContractualEmployee
                                select new { Id = b.EmployeeId, EmpType = b.EmployeeType });

            return Ok(await query.ToListAsync());
        }

        [HttpGet("GetAllSalaryCalculationById/{EmpId}")]
        public async Task<IActionResult> GetAllEmployeeSalaries(int EmpId)
        {
            return Ok(await dbContext.EmployeeSalary.Where(s => s.EmployeeId == EmpId).ToListAsync());
        }

        private string formatNumber(int number, int digits)
        {
            string s = "{0:";
            for (int i = 0; i < digits; i++)
            {
                s += "0";
            }
            s += "}";

            string value = string.Format(s, number);

            return value;
        }

        private int getEmployeeNumber()
        {
            var returnValue = 1;

            var rEmp = dbContext.RegularEmployee.LastOrDefault<RegularEmployee>();
            var cEmp = dbContext.ContractualEmployee.LastOrDefault<ContractualEmployee>();

            if (rEmp != null && cEmp == null)
            {
                return (int.Parse(rEmp.EmployeeId.ToString().Substring(4)) + 1);
            }

            if (rEmp == null && cEmp != null)
            {
                return (int.Parse(cEmp.EmployeeId.ToString().Substring(4)) + 1);
            }

            if (rEmp != null && cEmp != null)
            {
                var val1 = (int.Parse(rEmp.EmployeeId.ToString().Substring(4)) + 1);
                var val2 = (int.Parse(cEmp.EmployeeId.ToString().Substring(4)) + 1);

                return Math.Max(val1, val2);
            }

            return returnValue;
        }

        private int generateEmployeeNumber()
        {
            var yrStr = DateTime.Now.Year.ToString();
            var tempNumber = getEmployeeNumber();
            var formattedStr = yrStr + formatNumber(tempNumber, 5);

            return int.Parse(formattedStr);
        }
    }
}
