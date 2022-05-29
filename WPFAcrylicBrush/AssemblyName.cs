using System.Reflection;

namespace WPFAcrylics
{
    internal static class AssemblyName
    {
        private static string? _name = null;
        public static string GetName => _name ??= Assembly.GetAssembly(typeof(AssemblyName))!.FullName!;
    }
}
