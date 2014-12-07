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
        private static TypeInfo tistring = new TypeInfo("string");
        private static TypeInfo tiint32 = new TypeInfo("int32");
        private static TypeInfo tireal64 = new TypeInfo("real64");

        private string name;

        public TypeInfo(string name)
        {
            this.name = name;
        }

        public static TypeInfo Bool { get { return tibool; } }

        public static TypeInfo String { get { return tistring; } }

        public static TypeInfo Int32 { get { return tiint32; } }

        public static TypeInfo Real64 { get { return tireal64; } }

        public static TypeInfo Nil { get { return tinil; } }

        public static TypeInfo GetTypeInfo(object value)
        {
            if (value == null)
                return tinil;

            if (value is bool)
                return tibool;

            if (value is string)
                return tistring;

            if (value is int)
                return tiint32;

            if (value is double)
                return tireal64;

            throw new NotImplementedException();
        }
    }
}
