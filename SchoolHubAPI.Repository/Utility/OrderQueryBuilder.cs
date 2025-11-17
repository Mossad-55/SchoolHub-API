using System.Reflection;
using System.Text;

namespace SchoolHubAPI.Repository.Utility;

public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParams = orderByQueryString.Trim().Split(',');

        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var queryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var propertyName = param.Split(" ")[0];
            var objectPropery = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (objectPropery is null)
                continue;

            var direction = param.EndsWith(" desc") ? "descending" : "ascending";

            queryBuilder.Append($"{objectPropery.Name.ToString()} {direction}, ");
        }

        var orderQuery = queryBuilder.ToString().TrimEnd(',', ' ');

        return orderQuery;
    }
}
