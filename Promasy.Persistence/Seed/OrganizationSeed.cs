using System;
using System.Collections.Generic;
using System.Linq;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Persistence.Context;
using Promasy.Security;

namespace Promasy.Persistence.Seed;

internal static class OrganizationSeed
{
    public static void EnsureDefaultsCreated(PromasyContext context, DefaultOrganizationSeedSettings settings)
    {
        if (!settings.EnableSeedDefaults)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.Organization))
        {
            throw new Exception("Default Organization name cannot be empty");
        }

        var organization = context.Organizations.FirstOrDefault(o => o.Name == settings.Organization)
                           ?? CreateOrganization(context, settings.Organization);
        
        if (string.IsNullOrWhiteSpace(settings.Department))
        {
            throw new Exception("Default Department name cannot be empty");
        }

        var department =
            context.Departments.FirstOrDefault(
                d => d.Name == settings.Department && d.OrganizationId == organization.Id)
            ?? CreateDepartment(context, settings.Department, organization.Id);
        
        if (string.IsNullOrWhiteSpace(settings.SubDepartment))
        {
            throw new Exception("Default SubDepartment name cannot be empty");
        }

        var subDepartment =
            context.SubDepartments.FirstOrDefault(s =>
                s.Name == settings.SubDepartment && s.DepartmentId == department.Id)
            ?? CreateSubDepartment(context, settings.SubDepartment, department.Id);
        
        if (string.IsNullOrWhiteSpace(settings.AdministratorUsername))
        {
            throw new Exception("Default Administrator username cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(settings.AdministratorPassword))
        {
            throw new Exception("Default Administrator password cannot be empty");
        }

        var employee = context.Employees.FirstOrDefault(e =>
                           e.UserName == settings.AdministratorUsername && e.SubDepartmentId == subDepartment.Id)
                       ?? CreateAdministrator(context, settings.AdministratorUsername, settings.AdministratorPassword,
                           subDepartment.Id);

        organization.CreatorId = employee.Id;
        department.CreatorId = employee.Id;
        subDepartment.CreatorId = employee.Id;
        subDepartment.OrganizationId = organization.Id;
        employee.CreatorId = employee.Id;
        employee.OrganizationId = organization.Id;
        context.SaveChanges();
    }

    private static Organization CreateOrganization(PromasyContext context, string organizationName)
    {
        var organization = new Organization
        {
            Name = organizationName,
            Email = string.Empty,
            Edrpou = string.Empty,
            PhoneNumber = string.Empty,
            FaxNumber = string.Empty,
            Address = new Address
            {
                Country = string.Empty,
                PostalCode = string.Empty,
                Region = string.Empty,
                City = string.Empty,
                CityType = CityType.City,
                Street = string.Empty,
                StreetType = StreetType.Street,
                BuildingNumber = string.Empty,
            }
        };
        context.Organizations.Add(organization);
        context.SaveChanges();

        return organization;
    }

    private static Department CreateDepartment(PromasyContext context, string departmentName, int organizationId)
    {
        var department = new Department
        {
            Name = departmentName,
            OrganizationId = organizationId
        };
        context.Departments.Add(department);
        context.SaveChanges();

        return department;
    }

    private static SubDepartment CreateSubDepartment(PromasyContext context, string subDepartmentName, int departmentId)
    {
        var subDepartment = new SubDepartment
        {
            Name = subDepartmentName,
            DepartmentId = departmentId,
        };

        context.SubDepartments.Add(subDepartment);
        context.SaveChanges();

        return subDepartment;
    }

    private static Employee CreateAdministrator(PromasyContext context, string username, string password, int subDepartmentId)
    {
        var role = context.Roles.First(r => r.Name == RoleName.Administrator);
        var employee = new Employee
        {
            FirstName = string.Empty,
            MiddleName = string.Empty,
            LastName = string.Empty,
            UserName = username.ToLower(),
            Email = string.Empty,
            PrimaryPhone = string.Empty,
            Password = PasswordHelper.Hash(password),
            SubDepartmentId = subDepartmentId,
            Roles = new List<Role> { role }
        };

        context.Employees.Add(employee);
        context.SaveChanges();

        return employee;
    }
}