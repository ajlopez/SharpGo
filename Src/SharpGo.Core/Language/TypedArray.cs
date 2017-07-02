namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TypedArray<T>
    {
        private T[] values;

        public TypedArray(int size)
        {
            this.values = new T[size];
        }

        public int Length { get { return this.values.Length; } }

        public T this[int index]
        {
            get
            {
                return this.values[index];
            }

            set
            {
                this.values[index] = value;
            }
        }
    }
}
