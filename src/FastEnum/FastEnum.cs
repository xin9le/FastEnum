using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace FastEnum
{
    public static class FastEnum<T>
        where T : Enum
    {
        public sealed class Member
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


        public static Type Type { get; }
        public static Type UnderlyingType { get; }
        public static T[] Values { get; }
        public static string[] Names { get; }
        public static Member[] Members { get; }
        internal static Dictionary<T, Member> MemberByValue { get; }
        internal static Dictionary<string, Member> MemberByName { get; }


        static FastEnum()
        {
            Type = typeof(T);
            UnderlyingType = Enum.GetUnderlyingType(Type);
            Values = Enum.GetValues(Type) as T[];
            Names = Enum.GetNames(Type);
            Members = Values.Select(x => new Member(x)).ToArray();
            MemberByValue = Members.ToDictionary(x => x.Value);
            MemberByName = Members.ToDictionary(x => x.Name);
        }


        public static bool IsDefined(T value)
            => MemberByValue.ContainsKey(value);


        public static T Parse(string name)
            => MemberByName.TryGetValue(name, out var member)
            ? member.Value
            : throw new ArgumentException(nameof(name));


        public static bool TryParse(string name, out T value)
        {
            if (MemberByName.TryGetValue(name, out var member))
            {
                value = member.Value;
                return true;
            }
            value = default;
            return false;
        }
    }
}
