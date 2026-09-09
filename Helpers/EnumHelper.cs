using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Models;

namespace Helpers
{
    public static class EnumHelper
    {
        public static List<LookupItem> ToLookup<T>()
            where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x =>
                {
                    var member = typeof(T)
                        .GetMember(x.ToString())
                        .First();

                    var display = member.GetCustomAttribute<DisplayAttribute>();

                    return new LookupItem
                    {
                        Id = Convert.ToInt32(x),
                        Code = x.ToString(),
                        Name = display?.Name ?? x.ToString()
                    };
                })
                .ToList();
        }
    }
}