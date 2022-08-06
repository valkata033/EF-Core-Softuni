using SoftUni.Models;
using SoftUni.Data;
using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Collections.Generic;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();

            //Problem 3
            //Console.WriteLine(GetEmployeesFullInformation(softUniContext));

            //Problem 4
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(softUniContext));

            //Problem 5
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(softUniContext));

            //Problem 6
            //Console.WriteLine(AddNewAddressToEmployee(softUniContext));

            //Problem 7
            //Console.WriteLine(GetEmployeesInPeriod(softUniContext));

            //Problem 8
            //Console.WriteLine(GetAddressesByTown(softUniContext));

            //Problem 9
            //Console.WriteLine(GetEmployee147(softUniContext));

            //Problem 10
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(softUniContext));

            //Problem 11
            //Console.WriteLine(GetLatestProjects(softUniContext));

            //Problem 12
            //Console.WriteLine(IncreaseSalaries(softUniContext));

            //Problem 13
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(softUniContext));

            //Problem 14
            //Console.WriteLine(DeleteProjectById(softUniContext));

            //Problem 15
            //Console.WriteLine(RemoveTown(softUniContext));
        }

        //Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee[] employees = context.Employees.ToArray();

            foreach (Employee e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);

            Employee nakov = context
                .Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            nakov.Address = newAddress;
            context.SaveChanges();

            var employees = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(e => e.Project.StartDate.Year >= 2001 &&
                                                         e.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                            EndDate = ep.Project.EndDate.HasValue 
                            ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") 
                            : "not finished"
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var p in e.AllProjects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adresses = context
               .Addresses
               .Include(x => x.Town)
               .Include(x => x.Employees)
               .OrderByDescending(e => e.Employees.Count)
               .ThenBy(t => t.Town.Name)
               .ThenBy(a => a.AddressText)
               .Select(e => new
               {
                   e.AddressText,
                   Town = e.Town.Name,
                   Employees = e.Employees
               })
               .Take(10)
               .ToArray();

            foreach (var a in adresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.Town} - {a.Employees.Count} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 9
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            int employeeId = 147;

            var employee = context
                .Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .FirstOrDefault(e => e.EmployeeId == employeeId);

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var e in employee.EmployeesProjects.OrderBy(x => x.Project.Name))
            {
                sb.AppendLine(e.Project.Name);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Department[] departments = context
                .Departments
                .Include(x => x.Employees)
                .ThenInclude(x => x.Manager)
                .Where(x => x.Employees.Count() > 5)
                .OrderBy(x => x.Employees.Count())
                .ThenBy(x => x.Name)
                .ToArray();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}");

                foreach (var e in d.Employees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName))
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Project[] projects = context
                .Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x => x.Name)
                .ToArray();

            foreach (var p in projects)
            {
                sb
                    .AppendLine(p.Name)
                    .AppendLine(p.Description)
                    .AppendLine($"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departmentsArray = new List<string>()
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var employeesToPromote = context
                .Employees
                .Include(x => x.Department)
                .Where(x => departmentsArray.Contains(x.Department.Name))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var e in employeesToPromote)
            {
                e.Salary *= 1.12M;

                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var names = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var e in names)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Project projectToDelete = context.Projects.Find(2);

            EmployeeProject[] referredEmployees = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == projectToDelete.ProjectId)
                .ToArray();

            context.EmployeesProjects.RemoveRange(referredEmployees);
            context.Projects.Remove(projectToDelete);
            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Select(p => p.Name)
                .Take(10)
                .ToArray();

            foreach (var pName in projectNames)
            {
                sb.AppendLine(pName);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context
                .Towns
                .FirstOrDefault(x => x.Name == "Seattle");

            var addresses = context
                .Addresses
                .Where(x => x.TownId == town.TownId);

            var employees = context
                .Employees
                .Where(x => addresses.Any(y => y.AddressId == x.AddressId))
                .ToArray();

            foreach (var e in employees)
            {
                e.AddressId = null;
            }

            context.SaveChanges();

            var deletedTownsCount = addresses.Count();
            context.Addresses.RemoveRange(addresses);
            context.SaveChanges();

            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{deletedTownsCount} addresses in Seattle were deleted";
        }
    }
}
