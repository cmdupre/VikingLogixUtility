using libplctag.NativeImport;
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

        internal static Tag<T>? Create(TagPath path, string fqn)
        {
            var id = plctag.plc_tag_create(path.WithFqn(fqn), path.Timeout);

            if (id < 0)
                return null;

            var value = Read(id, path.Timeout);

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
            get => currentValue;

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

        public void Write(string value)
        {
            if (typeof(T) == typeof(bool))
                plctag.plc_tag_set_bit(Id, 0, Int32.Parse(value));

            else if (typeof(T) == typeof(float))
                plctag.plc_tag_set_float32(Id, 0, Single.Parse(value));

            else if (typeof(T) == typeof(sbyte))
                plctag.plc_tag_set_int8(Id, 0, SByte.Parse(value));

            else if (typeof(T) == typeof(short))
                plctag.plc_tag_set_int16(Id, 0, short.Parse(value));

            else if (typeof(T) == typeof(int))
                plctag.plc_tag_set_int32(Id, 0, Int32.Parse(value));

            else if (typeof(T) == typeof(long))
                plctag.plc_tag_set_int64(Id, 0, Int64.Parse(value));

            else if (typeof(T) == typeof(double))
                plctag.plc_tag_set_float64(Id, 0, double.Parse(value));

            else if (typeof(T) == typeof(string))
                plctag.plc_tag_set_string(Id, 0, value);

            else if (typeof(T) == typeof(byte))
                plctag.plc_tag_set_uint8(Id, 0, byte.Parse(value));

            else if (typeof(T) == typeof(ushort))
                plctag.plc_tag_set_uint16(Id, 0, ushort.Parse(value));

            else if (typeof(T) == typeof(ulong))
                plctag.plc_tag_set_uint64(Id, 0, ulong.Parse(value));

            else
                throw new InvalidCastException("Data type not recognized.");

            plctag.plc_tag_write(Id, Path.Timeout);

            Value = value;
        }

        public void Read() =>
            Value = Read(Id, Path.Timeout);

        public void Toggle()
        {
            if (typeof(T) != typeof(bool))
                throw new NotImplementedException("Not valid for this data type.");

            Write(Value == "1" ? "0" : "1");
        }

        private static string Read(int id, int timeout)
        {
            var result = plctag.plc_tag_read(id, timeout);

            if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                throw new InvalidDataException(plctag.plc_tag_decode_error(result));

            if (typeof(T) == typeof(bool))
                return plctag.plc_tag_get_bit(id, 0).ToString();

            if (typeof(T) == typeof(float))
                return plctag.plc_tag_get_float32(id, 0).ToString();

            if (typeof(T) == typeof(sbyte))
                return plctag.plc_tag_get_int8(id, 0).ToString();

            if (typeof(T) == typeof(short))
                return plctag.plc_tag_get_int16(id, 0).ToString();

            if (typeof(T) == typeof(int))
                return plctag.plc_tag_get_int32(id, 0).ToString();

            if (typeof(T) == typeof(long))
                return plctag.plc_tag_get_int64(id, 0).ToString();

            if (typeof(T) == typeof(double))
                return plctag.plc_tag_get_float64(id, 0).ToString();

            if (typeof(T) == typeof(byte))
                return plctag.plc_tag_get_uint8(id, 0).ToString();

            if (typeof(T) == typeof(ushort))
                return plctag.plc_tag_get_uint16(id, 0).ToString();

            if (typeof(T) == typeof(uint))
                return plctag.plc_tag_get_uint32(id, 0).ToString();

            if (typeof(T) == typeof(ulong))
                return plctag.plc_tag_get_uint64(id, 0).ToString();

            // TODO: Controller "STRING" is a UDT wieh LEN and DATA.
            // DATA is an array of SINTs.
            //if (typeof(T) == typeof(string))
            //{
            //    var sb = new StringBuilder(200);

            //    var getStringResult = plctag.plc_tag_get_string(id, 0, sb, sb.Length);

            //    if (getStringResult != (int)STATUS_CODES.PLCTAG_STATUS_OK)
            //        throw new InvalidDataException(plctag.plc_tag_decode_error(getStringResult));

            //    return sb.ToString();
            //}

            throw new InvalidCastException("Data type not recognized.");
        }
    }
}
