using System.Reflection;

namespace WPFAcrylics.Helpers
{
    internal static class AssemblyName
    {
        private static string? _name = null;
        public static string GetName => _name ??= Assembly.GetAssembly(typeof(AssemblyName))!.FullName!;
    }
}
