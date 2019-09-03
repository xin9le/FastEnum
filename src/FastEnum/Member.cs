using System;
using System.Reflection;



namespace FastEnum
{
    public sealed class Member<T>
        where T : Enum
    {
        public T Value { get; }
        public string Name { get; }
        public FieldInfo FieldInfo { get; }
        internal Member(T value)
        {
            var type = typeof(T);
            this.Value = value;
            this.Name = Enum.GetName(type, value);
            this.FieldInfo = type.GetField(this.Name);
        }
    }
}
