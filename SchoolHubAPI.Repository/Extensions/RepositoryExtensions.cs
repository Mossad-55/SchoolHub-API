using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Utility;
using System.Linq.Dynamic.Core;

namespace SchoolHubAPI.Repository.Extensions;

public static class RepositoryExtensions
{
    // Admin search & sort
    public static IQueryable<Admin> Search(this IQueryable<Admin> admins, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return admins;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return admins.Where(a =>
           (a.User!.Name != null && a.User.Name.ToLower().Contains(lowerCaseTerm)) ||
           (a.User.Email != null && a.User.Email.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Admin> Sort(this IQueryable<Admin> admins, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return admins.OrderBy(a => a.User!.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Admin>(orderByQueryString);
        if(string.IsNullOrWhiteSpace(orderQuery))
            return admins.OrderBy(a => a.User!.Name);

        return admins.OrderBy(orderQuery);
    }

    // Teacher search & sort
    public static IQueryable<Teacher> Search(this IQueryable<Teacher> teachers, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return teachers;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return teachers.Where(t =>
            (t.User!.Name != null && t.User.Name.ToLower().Contains(lowerCaseTerm)) ||
            (t.User.Email != null && t.User.Email.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Teacher> Sort(this IQueryable<Teacher> teachers, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return teachers.OrderBy(t => t.User!.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Teacher>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return teachers.OrderBy(t => t.User!.Name);

        return teachers.OrderBy(orderQuery);
    }

    // Student search & sort
    public static IQueryable<Student> Search(this IQueryable<Student> students, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return students;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return students.Where(s =>
            (s.User!.Name != null && s.User.Name.ToLower().Contains(lowerCaseTerm)) ||
            (s.User.Email != null && s.User.Email.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Student> Sort(this IQueryable<Student> students, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return students.OrderBy(s => s.User!.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Student>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return students.OrderBy(s => s.User!.Name);

        return students.OrderBy(orderQuery);
    }

    // Department search & sort
    public static IQueryable<Department> Search(this IQueryable<Department> departments, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return departments;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return departments.Where(d => d.Name != null && d.Name.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Department> Sort(this IQueryable<Department> departments, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return departments.OrderBy(d => d.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Department>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return departments.OrderBy(d => d.Name);

        return departments.OrderBy(orderQuery);
    }

    // Course search & sort
    public static IQueryable<Course> Search(this IQueryable<Course> courses, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return courses;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return courses.Where(c =>
            (c.Name != null && c.Name.ToLower().Contains(lowerCaseTerm)) ||
            (c.Code != null && c.Code.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Course> Sort(this IQueryable<Course> courses, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return courses.OrderBy(c => c.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Course>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return courses.OrderBy(c => c.Name);

        return courses.OrderBy(orderQuery);
    }

    // Batch search & sort
    public static IQueryable<Batch> Search(this IQueryable<Batch> batches, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return batches;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return batches.Where(b =>
            b.Name != null && b.Name.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Batch> Sort(this IQueryable<Batch> batches, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return batches.OrderBy(b => b.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Batch>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return batches.OrderBy(b => b.Name);

        return batches.OrderBy(orderQuery);
    }
}
