using Humanizer;

namespace Gems
{
    public static class NamingConventionExtensions
    {
        public static string Convert(this NamingConvention namingConvention, string name)
        {
            switch (namingConvention)
            {
                case NamingConvention.None:
                    return name;
                case NamingConvention.CamelCase:
                    return name.Camelize();
                case NamingConvention.SnakeCase:
                    return name.Underscore();
                case NamingConvention.PascalCase:
                    return name.Pascalize();
                case NamingConvention.KebabCase:
                    return name.Dasherize();
                default:
                    throw new NotImplementedException();
            }
        }

        public static string Convert(this string name, NamingConvention namingConvention)
        {
            return Convert(namingConvention, name);
        }
    }
}
