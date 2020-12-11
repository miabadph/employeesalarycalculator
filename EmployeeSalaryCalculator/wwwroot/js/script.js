var addForm = $("#addForm");
var calculateForm = $("#calculateSalary");
var fName = $("#firstName");
var lName = $("#lastName")
var bDate = $("#birthDatePicker");
var tin = $("#tinNumber");
var employeeType = $("#employeeType");
var dailySalary = $("#dailySalary");
var monthlySalary = $("#monthlySalary");
var employeeIdSelector = $("#employeeIdSelect");
var monthSelector = $("#monthSelector");
var absencesInput = $("#absencesInput");
var computeBtn = $("#computeBtn");
var tbl = $("#salariesTbl");
var salaryEmployeeSelector = $("#employeeSalarySelect");
var viewSalaryBtn = $("#viewSalaryBtn");

const createRegularURL = "/api/SalaryCalculator/AddRegularEmployee";
const createContractualURL = "/api/SalaryCalculator/AddContractualEmployee";
const computeSalaryURL = "/api/SalaryCalculator/CalculateSalary"
const getEmployeeIdsURL = "api/SalaryCalculator/GetAllEmployeeIds";

var selectedId = "";

computeBtn.click(function () {
    var empId = parseInt(employeeIdSelector.val());
    var calcDate = new Date(monthSelector.val());
    var absences = parseFloat(absencesInput.val());
    var workDays = getWeekdaysInMonth(calcDate.getMonth(), calcDate.getFullYear());

    var inputData = {
        EmployeeId: empId,
        Absences: absences,
        WorkDays: workDays,
        CalculationDate: calcDate
    }

    var jsonInput = JSON.stringify(inputData);

    $.ajax({
        url: computeSalaryURL,
        type: "POST",
        data: jsonInput,
        contentType: "application/json;charset=utf-8",
        success: function (res) {
            console.log("Success");
            selectedId = res.employeeId.toString();
            if (!isOptionExists(selectedId)) {
                getAllEmployeeIds();
                return;
            }
            salaryEmployeeSelector.val(selectedId).change();
            employeeIdSelector.val(selectedId).change();
            getAllSalariesById();
            
        },
        error: function (res) {
            console.log("Add Error");
        }
    });

    monthSelector.val('');
    absencesInput.val('');
});

addForm.on("submit", function (e) {
    var name = lName.val() + ", " + fName.val();
    var birthDate = bDate.val();
    var tinId = tin.val();
    var empType = parseInt(employeeType.val());
    
    var url = empType == 0 ? createRegularURL : createContractualURL;

    var inputDataRegular = {
        Name: name,
        TIN: tinId,
        BirthDate: birthDate,
        EmployeeType: empType,
        BasicSalary: parseFloat(monthlySalary.val().substring(4).replace(/,/g, ''))
    };

    var inputDateContractual = {
        Name: name,
        TIN: tinId,
        BirthDate: birthDate,
        EmployeeType: empType,
        DailySalary: parseFloat(dailySalary.val().substring(4).replace(/,/g, ''))
    };

    var jsonInput = JSON.stringify(empType == 0 ? inputDataRegular : inputDateContractual);
    $.ajax({
        url: url,
        type: "POST",
        data: jsonInput,
        contentType: "application/json;charset=utf-8",
        success: function (res) {
            var opt = document.createElement('option');
            opt.value = res;
            opt.innerHTML = res;
            employeeIdSelector.append(opt);
        },
        error: function (res) {
            console.log("Add Error");
        }
    })

    addForm[0].reset();
    monthlySalary.prop('disabled', false);
    dailySalary.prop('disabled', true);
    e.preventDefault();
});

function getAllSalariesById() {
    if (salaryEmployeeSelector.val() == "") {
        return;
    }

    $.ajax({
        url: "/api/SalaryCalculator/GetAllSalaryCalculationById/" + salaryEmployeeSelector.val(),
        type: "GET",
        success: function (data) {
            $("#salariesTbl tr:gt(0)").remove();
            for (var item in data) {
                var et = getEmployeeType(data[item].employeeType);
                var dp = data[item].dailyPay.toFixed(2);
                var tp = data[item].totalPay.toFixed(2);
                var fd = getYearMonth(data[item].date);
                addRow(et, data[item].workDays, dp, data[item].absences, data[item].tax, tp, fd);
            }
        },
        error: function (res) {
            console.log("Error load results");
        }
    });
}

function getAllEmployeeIds() {
    $.ajax({
        url: getEmployeeIdsURL,
        type: "GET",
        success: function (data) {
            salaryEmployeeSelector.empty();
            for (var item in data) {
                var opt = document.createElement('option');
                opt.value = data[item].id;
                opt.innerHTML = data[item].id;
                salaryEmployeeSelector.append(opt);
            }

            salaryEmployeeSelector.val(selectedId).change();
            getAllSalariesById();
            
        },
        error: function () {
            console.log("Error load results");
        }
    });
}

viewSalaryBtn.click(function () {
    getAllSalariesById();
});

var salaryMask = {
    'alias': 'numeric',
    'groupSeparator': ',',
    'autoGroup': true,
    'digits': 2,
    'digitsOptional': false,
    'prefix': 'Php ',
    'placeholder': '0'
};

tin.inputmask({ 'mask': '999-999-999' });
dailySalary.inputmask(salaryMask);
monthlySalary.inputmask(salaryMask);

employeeType.on('change', function (e) {
    switch (employeeType.val()) {
        case "0":
            monthlySalary.prop('disabled', false);
            dailySalary.prop('disabled', true);
            break;
        case "1":
            monthlySalary.prop('disabled', true);
            dailySalary.prop('disabled', false);
            break;
    }
});

monthSelector.on('change', function (e) {
    var date = new Date(monthSelector.val());
    var weekDays = getWeekdaysInMonth(date.getMonth(), date.getFullYear());
    absencesInput.val(0);
    absencesInput.attr({
        "max": weekDays
    });
});

absencesInput.bind('mousewheel', function (e) { });

function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}

function isWeekday(year, month, day) {
    var day = new Date(year, month, day).getDay();
    return day != 0 && day != 6;
}

function getWeekdaysInMonth(month, year) {
    var days = daysInMonth(month, year);
    var weekdays = 0;
    for (var i = 0; i < days; i++) {
        if (isWeekday(year, month, i + 1))
            weekdays++;
    }
    return weekdays;
}

function addRow(employeeType, workDays, dailyPay, absences, tax, totalPay, date) {
    var tr = document.createElement('tr');

    addCell(tr, employeeType);
    addCell(tr, workDays);
    addCell(tr, dailyPay);
    addCell(tr, absences);
    addCell(tr, tax);
    addCell(tr, totalPay);
    addCell(tr, date);

    tbl.append(tr);
}


function addCell(tr, val) {
    var td = document.createElement('td');

    td.innerHTML = val;

    tr.appendChild(td);
} 

function isOptionExists(id) {
    var exists = false;
    $('#employeeSalarySelect option').each(function () {
        if (this.value == id) {
            exists = true;
            return false;
        }
    });

    return exists;
}

function getEmployeeType(input) {
    var returnValue = "";
    switch (input) {
        case 0:
            returnValue = "Regular";
            break;
        case 1:
            returnValue = "Contractual";
            break;
    }

    return returnValue;
}

function getYearMonth(dateStr) {
    var date = new Date(dateStr);
    var month = date.getUTCMonth() + 1;
    var year = date.getUTCFullYear();

    var formattedDate = year + "-" + getMonthString(month);

    return formattedDate;
}

function getMonthString(iMonth) {
    var strMonth = "";

    switch (iMonth) {
        case 1:
            strMonth = "January";
            break;
        case 2:
            strMonth = "February";
            break;
        case 3:
            strMonth = "March";
            break;
        case 4:
            strMonth = "April";
            break;
        case 5:
            strMonth = "May";
            break;
        case 6:
            strMonth = "June";
            break;
        case 7:
            strMonth = "July";
            break;
        case 8:
            strMonth = "August";
            break;
        case 9:
            strMonth = "September";
            break;
        case 10:
            strMonth = "October";
            break;
        case 11:
            strMonth = "November";
            break;
        case 12:
            strMonth = "December";
            break;
    }

    return strMonth;
}