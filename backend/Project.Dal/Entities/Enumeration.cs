using System.Collections.ObjectModel;
using System.Reflection;

namespace Project.Dal.Entities
{
    public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
        where TEnum : Enumeration<TEnum>
    {
        public int Id { get; protected init; }
        public string Name { get; protected init; } = string.Empty;

        private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumeration();

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static TEnum? FromId(int id) => Enumerations.TryGetValue(id, out TEnum? enumeration) ? enumeration : default;
        public static TEnum? FromName(string name) => Enumerations.Values.SingleOrDefault(x => x.Name == name);
        public bool Equals(Enumeration<TEnum>? other)
        {
            if (other is null) return false;

            return GetType().Equals(other.GetType()) && Name == other.Name;
        }
        public override bool Equals(object? obj) => obj is Enumeration<TEnum> other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;
        private static Dictionary<int, TEnum> CreateEnumeration()
        {
            var enumerationType = typeof(TEnum);

            var fieldsForType = enumerationType
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy)
                .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
                .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);

            return fieldsForType.ToDictionary(x => x.Id);
        }
        public static IReadOnlyCollection<TEnum> GetValues() => new ReadOnlyCollection<TEnum>(Enumerations.Values.ToList());

    }
}