using System.IO;
using System.Reflection;
using System.Text.Json;

namespace VikingLogixUtility.Models
{
    internal sealed class Settings(string address)
    {
        private static readonly string Filename = Assembly
            .GetExecutingAssembly()
            .Location
            .Replace(".dll", ".json");

        private static Settings Default =>
            new(string.Empty);

        public static Settings Load(string? filename = null)
        {
            try
            {
                var json = File.ReadAllText(filename ?? Filename);

                return JsonSerializer.Deserialize<Settings>(json) ?? Default;
            }
            catch
            {
                // NOP
            }

            return Default;
        }

        public void Write(string? filename = null)
        {
            var json = JsonSerializer.Serialize(this);

            File.WriteAllText(filename ?? Filename, json);
        }

        public string Address => address;
    }
}
