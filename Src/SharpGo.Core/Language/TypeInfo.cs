namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TypeInfo
    {
        private static TypeInfo tinil = new TypeInfo("nil");
        private static TypeInfo tibool = new TypeInfo("bool");
        private static TypeInfo tistring = new TypeInfo("string", typeof(string));
        private static TypeInfo tibyte = new TypeInfo("byte");
        private static TypeInfo tiint = new TypeInfo("int");
        private static TypeInfo tiint16 = new TypeInfo("int16");
        private static TypeInfo tiint32 = new TypeInfo("int32");
        private static TypeInfo tiint64 = new TypeInfo("int64");
        private static TypeInfo tiuint = new TypeInfo("uint");
        private static TypeInfo tifloat32 = new TypeInfo("float32");
        private static TypeInfo tifloat64 = new TypeInfo("float64");

        private string name;
        private Type nativetype;

        public TypeInfo(string name)
        {
            this.name = name;
        }

        public TypeInfo(string name, Type nativetype)
        {
            this.name = name;
            this.nativetype = nativetype;
        }

        public static TypeInfo Bool { get { return tibool; } }

        public static TypeInfo String { get { return tistring; } }

        public static TypeInfo Byte { get { return tibyte; } }

        public static TypeInfo Int { get { return tiint32; } }

        public static TypeInfo Int16 { get { return tiint16; } }

        public static TypeInfo Int32 { get { return tiint32; } }

        public static TypeInfo Int64 { get { return tiint64; } }

        public static TypeInfo UInt { get { return tiuint; } }

        public static TypeInfo Float32 { get { return tifloat32; } }

        public static TypeInfo Float64 { get { return tifloat64; } }

        public static TypeInfo Nil { get { return tinil; } }

        public string Name { get { return this.name; } }

        public Type NativeType { get { return this.nativetype; } }

        public static TypeInfo GetTypeInfo(object value)
        {
            if (value == null)
                return tinil;

            if (value is bool)
                return tibool;

            if (value is string)
                return tistring;

            if (value is byte)
                return tibyte;

            if (value is short)
                return tiint16;

            if (value is int)
                return tiint32;

            if (value is long)
                return tiint64;

            if (value is float)
                return tifloat32;

            if (value is double)
                return tifloat64;

            throw new NotImplementedException();
        }
    }
}
