using libplctag.NativeImport;
using System.Text;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public sealed class Tag<T> : ITag
    {
        private readonly TagPath path;
        private readonly string name;
        private readonly int id;
        
        private object currentValue;
        private object previousValue;

        private Tag(TagPath path, string name, int id, object currentValue, object previousValue)
        {
            this.path = path;
            this.name = name;
            this.id = id;
            this.currentValue = currentValue;
            this.previousValue = previousValue;
        }

        public void Dispose() => Destroy(Id);

        private static void Destroy(int id) => plctag.plc_tag_destroy(id);

        public static Tag<T>? Create(TagPath path, string name)
        {
            var id = plctag.plc_tag_create(path.WithName(name), path.Timeout);

            if (id < 0)
            {
                Console.Error.WriteLine(plctag.plc_tag_decode_error(id));
                return null;
            }

            var value = Read(id, path.Timeout);

            if (value is null)
            {
                Destroy(id);
                Console.Error.WriteLine("Invalid tag value returned from read.");
                return null;
            }

            // Satisfy compiler null check.
            var notnullValue = value;

            return new Tag<T>(path, name, id, notnullValue, notnullValue);
        }

        public TagPath Path => path;

        public string Name => name;

        public int Id => id;

        public object Value
        {
            get => currentValue;

            private set
            {
                PreviousValue = Value;
                currentValue = value;
            }
        }

        public object PreviousValue
        {
            get => previousValue;

            private set => previousValue = value;
        }

        public bool Changed => !Value.Equals(PreviousValue);

        public void Write(object value)
        {
            // TODO: check return values?

            if (typeof(T) == typeof(bool))
                plctag.plc_tag_set_bit(Id, 0, (int)value);

            else if (typeof(T) == typeof(float))
                plctag.plc_tag_set_float32(Id, 0, Convert.ToSingle(value));

            else if (typeof(T) == typeof(sbyte))
                plctag.plc_tag_set_int8(Id, 0, (sbyte)value);

            else if (typeof(T) == typeof(short))
                plctag.plc_tag_set_int16(Id, 0, (short)value);

            else if (typeof(T) == typeof(int))
                plctag.plc_tag_set_int32(Id, 0, (int)value);

            else if (typeof(T) == typeof(long))
                plctag.plc_tag_set_int64(Id, 0, (long)value);

            else if (typeof(T) == typeof(double))
                plctag.plc_tag_set_float64(Id, 0, (double)value);

            else if (typeof(T) == typeof(string))
                plctag.plc_tag_set_string(Id, 0, (string)value);

            else if (typeof(T) == typeof(byte))
                plctag.plc_tag_set_uint8(Id, 0, (byte)value);

            else if (typeof(T) == typeof(ushort))
                plctag.plc_tag_set_uint16(Id, 0, (ushort)value);

            else if (typeof(T) == typeof(ulong))
                plctag.plc_tag_set_uint64(Id, 0, (ulong)value);

            else
                throw new InvalidCastException("Data type not recognized.");

            plctag.plc_tag_write(Id, Path.Timeout);

            Read();
        }

        public void Read() => 
            Value = Read(Id, Path.Timeout);

        public void Toggle()
        {
            if (typeof(T) != typeof(bool))
                throw new NotImplementedException("Not valid for this data type.");

            Write((int)Value == 1 ? 0 : 1);
        }

        private static object Read(int id, int timeout)
        {
            var result = plctag.plc_tag_read(id, timeout);

            if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                Console.Error.WriteLine(plctag.plc_tag_decode_error(result));

            if (typeof(T) == typeof(bool))
                return plctag.plc_tag_get_bit(id, 0);

            if (typeof(T) == typeof(float))
                return plctag.plc_tag_get_float32(id, 0);

            if (typeof(T) == typeof(sbyte))
                return plctag.plc_tag_get_int8(id, 0);

            if (typeof(T) == typeof(short))
                return plctag.plc_tag_get_int16(id, 0);

            if (typeof(T) == typeof(int))
                return plctag.plc_tag_get_int32(id, 0);

            if (typeof(T) == typeof(long))
                return plctag.plc_tag_get_int64(id, 0);

            if (typeof(T) == typeof(double))
                return plctag.plc_tag_get_float64(id, 0);

            if (typeof(T) == typeof(byte))
                return plctag.plc_tag_get_uint8(id, 0);

            if (typeof(T) == typeof(ushort))
                return plctag.plc_tag_get_uint16(id, 0);

            if (typeof(T) == typeof(uint))
                return plctag.plc_tag_get_uint32(id, 0);

            if (typeof(T) == typeof(ulong))
                return plctag.plc_tag_get_uint64(id, 0);

            if (typeof(T) == typeof(string))
            {
                var sb = new StringBuilder(200);

                // TODO: need length + 1?
                var getStringResult = plctag.plc_tag_get_string(id, 0, sb, sb.Length);

                // TODO: exception?
                if (getStringResult != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                    Console.Error.WriteLine($"Error reading string value: {plctag.plc_tag_decode_error(getStringResult)}.");

                return sb.ToString();
            }

            throw new InvalidCastException("Data type not recognized.");
        }
    }
}
