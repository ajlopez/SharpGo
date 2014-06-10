namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TypeInfo
    {
        private static TypeInfo tibool = new TypeInfo("bool");
        private static TypeInfo tistring = new TypeInfo("string");

        private string name;

        public TypeInfo(string name)
        {
            this.name = name;
        }

        public static TypeInfo Bool { get { return tibool; } }

        public static TypeInfo String { get { return tistring; } }

        public static TypeInfo GetTypeInfo(object value)
        {
            if (value is bool)
                return tibool;

            if (value is string)
                return tistring;

            throw new NotImplementedException();
        }
    }
}
