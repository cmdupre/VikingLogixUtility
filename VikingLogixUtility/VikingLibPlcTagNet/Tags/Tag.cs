using libplctag.NativeImport;
using System.Text;
using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Interfaces;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public sealed class Tag<T> : ITag
    {
        private readonly TagPath path;
        private readonly string fqn;
        private readonly int id;

        private string currentValue;
        private string previousValue;

        private Tag(TagPath path, string fqn, int id, string currentValue, string previousValue)
        {
            this.path = path;
            this.fqn = fqn;
            this.id = id;
            this.currentValue = currentValue;
            this.previousValue = previousValue;
        }

        public void Dispose() => Destroy(Id);

        private static void Destroy(int id) => plctag.plc_tag_destroy(id);

        internal static Tag<T>? Create(TagPath path, string fqn, ILoggable? logger = null)
        {
            var id = plctag.plc_tag_create(path.GetAttributeString(fqn), path.Timeout);

            if (id < 0)
            {
                Helper.LogError(logger, "Error creating tag", [fqn], id);
                return null;
            }

            var value = Read(id, path, fqn, logger);

            if (value is null)
            {
                Destroy(id);
                return null;
            }

            return new Tag<T>(path, fqn, id, $"{value}", $"{value}");
        }

        public TagPath Path => path;

        /// <summary>
        /// Fully Qualified Name
        /// </summary>
        public string FQN => fqn;

        public int Id => id;

        public string Value
        {
            get
            {
                // make it clear to user if type is floating point
                if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
                {
                    if (double.TryParse(currentValue, out var currentValueDouble))
                    {
                        var formattedValue = currentValueDouble.ToString("G");

                        if (!formattedValue.Contains('.'))
                            formattedValue += ".0";

                        return formattedValue;
                    }
                }

                return currentValue;
            }

            private set
            {
                PreviousValue = Value;
                currentValue = value;
            }
        }

        public string PreviousValue
        {
            get => previousValue;

            private set => previousValue = value;
        }

        public bool Changed => !Value.Equals(PreviousValue);

        public void Write(string value, ILoggable? logger = null)
        {
            if (typeof(T) == typeof(bool))
                plctag.plc_tag_set_bit(Id, 0, Int32.Parse(value));

            else if (typeof(T) == typeof(sbyte))
                plctag.plc_tag_set_int8(Id, 0, SByte.Parse(value));

            else if (typeof(T) == typeof(short))
                plctag.plc_tag_set_int16(Id, 0, short.Parse(value));

            else if (typeof(T) == typeof(int))
                plctag.plc_tag_set_int32(Id, 0, Int32.Parse(value));

            else if (typeof(T) == typeof(long))
                plctag.plc_tag_set_int64(Id, 0, Int64.Parse(value));

            else if (typeof(T) == typeof(byte))
                plctag.plc_tag_set_uint8(Id, 0, byte.Parse(value));

            else if (typeof(T) == typeof(ushort))
                plctag.plc_tag_set_uint16(Id, 0, ushort.Parse(value));

            else if (typeof(T) == typeof(uint))
                plctag.plc_tag_set_uint32(Id, 0, ushort.Parse(value));

            else if (typeof(T) == typeof(ulong))
                plctag.plc_tag_set_uint64(Id, 0, ulong.Parse(value));

            else if (typeof(T) == typeof(float))
                plctag.plc_tag_set_float32(Id, 0, Single.Parse(value));

            else if (typeof(T) == typeof(double))
                plctag.plc_tag_set_float64(Id, 0, double.Parse(value));

            else if (typeof(T) == typeof(DateTime))
                plctag.plc_tag_set_int32(Id, 0, Int32.Parse(value));

            else if (typeof(T) == typeof(string))
                plctag.plc_tag_set_string(Id, 0, value);

            else
            {
                Helper.LogError(logger, "Data type not recognized", [typeof(T)]);
                return;
            }

            plctag.plc_tag_write(Id, Path.Timeout);

            Value = value;
        }

        public void Read(ILoggable? logger = null) =>
            Value = Read(Id, Path, FQN, logger);

        public void Toggle(ILoggable? logger = null)
        {
            if (typeof(T) != typeof(bool))
            {
                Helper.LogError(logger, "Not valid for data type", [typeof(T)]);
                return;
            }

            Write(Value == "1" ? "0" : "1", logger);
        }

        private static string Read(int id, TagPath path, string fqn, ILoggable? logger = null)
        {
            var result = plctag.plc_tag_read(id, path.Timeout);

            if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
            {
                Helper.LogError(logger, "Error reading tag", [fqn], result);
                return string.Empty;
            }

            if (typeof(T) == typeof(bool))
                return plctag.plc_tag_get_bit(id, 0).ToString();

            if (typeof(T) == typeof(sbyte))
                return plctag.plc_tag_get_int8(id, 0).ToString();

            if (typeof(T) == typeof(char))
                return Convert.ToChar(plctag.plc_tag_get_int8(id, 0)).ToString();

            if (typeof(T) == typeof(short))
                return plctag.plc_tag_get_int16(id, 0).ToString();

            if (typeof(T) == typeof(int))
                return plctag.plc_tag_get_int32(id, 0).ToString();

            if (typeof(T) == typeof(long))
                return plctag.plc_tag_get_int64(id, 0).ToString();

            if (typeof(T) == typeof(byte))
                return plctag.plc_tag_get_uint8(id, 0).ToString();

            if (typeof(T) == typeof(ushort))
                return plctag.plc_tag_get_uint16(id, 0).ToString();

            if (typeof(T) == typeof(uint))
                return plctag.plc_tag_get_uint32(id, 0).ToString();

            if (typeof(T) == typeof(ulong))
                return plctag.plc_tag_get_uint64(id, 0).ToString();

            if (typeof(T) == typeof(float))
                return plctag.plc_tag_get_float32(id, 0).ToString();

            if (typeof(T) == typeof(double))
                return plctag.plc_tag_get_float64(id, 0).ToString();

            if (typeof(T) == typeof(DateTime))
                return plctag.plc_tag_get_int32(id, 0).ToString();

            if (typeof(T) == typeof(string))
            {
                // found this length by accident (trial/error) while looking at libplctag github issues
                var length = plctag.plc_tag_get_int32(id, 0);

                var sb = new StringBuilder(length);

                for (int i = 0; i < length; i++)
                {
                    var tag = TagFactory.GetTagFor(DataTypes.CHAR, path, $"{fqn}.DATA[{i}]", logger);
                    sb.Append(tag?.Value);
                }

                return sb.ToString();
            }

            Helper.LogError(logger, "Data type not recognized", [typeof(T)]);
            return string.Empty;
        }
    }
}
