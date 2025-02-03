using System.Text;
using VikingLibPlcTagNet.Interfaces;

namespace VikingLogixUtility.Models
{
    internal sealed class Logger : ILoggable
    {
        private readonly StringBuilder text = new();

        public string Text => text.ToString();

        public event EventHandler<EventArgs>? TextChanged;

        public void Log(string message)
        {
            text.AppendLine(message);
            TextChanged?.Invoke(this, new EventArgs());
        }

        public void Clear() => text.Clear();
    }
}
