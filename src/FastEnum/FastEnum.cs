using System;
using System.Collections.Generic;
using System.Linq;



namespace FastEnum
{
    public static class FastEnum<T>
        where T : Enum
    {
        public static Type Type { get; }
        public static Type UnderlyingType { get; }
        public static T[] Values { get; }
        public static string[] Names { get; }
        public static Member<T>[] Members { get; }
        internal static Dictionary<T, Member<T>> MemberByValue { get; }
        private static Dictionary<string, Member<T>> MemberByName { get; }


        static FastEnum()
        {
            Type = typeof(T);
            UnderlyingType = Enum.GetUnderlyingType(Type);
            Values = Enum.GetValues(Type) as T[];
            Names = Enum.GetNames(Type);
            Members = Values.Select(x => new Member<T>(x)).ToArray();
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
