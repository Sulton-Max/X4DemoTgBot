using System.Reflection;

namespace DemoBot.Web.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetConfiguration<T>(this IConfiguration configuration)
        {
            return configuration
                .GetSection(typeof(T)
                .GetProperty("Position", BindingFlags.Public | BindingFlags.Static)
                .GetValue(null, null) as string).Get<T>();
        }
    }
}
