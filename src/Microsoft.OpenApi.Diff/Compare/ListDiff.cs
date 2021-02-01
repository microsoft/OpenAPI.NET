using System.Linq;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Extensions;

namespace Microsoft.OpenApi.Diff.Compare
{
    public static class ListDiff
    {
        public static T1 Diff<T1>(T1 instance)
            where T1 : ChangedListBO<string>
        {
            if (instance.OldValue.IsNullOrEmpty() && instance.NewValue.IsNullOrEmpty())
            {
                return instance;
            }
            if (instance.OldValue.IsNullOrEmpty())
            {
                instance.Increased = instance.NewValue.ToList();
                return instance;
            }
            if (instance.NewValue.IsNullOrEmpty())
            {
                instance.Missing = instance.OldValue.ToList();
                return instance;
            }
            instance.Increased.AddRange(instance.NewValue);
            foreach (var leftItem in instance.OldValue)
            {
                if (instance.NewValue.Contains(leftItem))
                {
                    instance.Increased.Remove(leftItem);
                    instance.Shared.Add(leftItem);
                }
                else
                {
                    instance.Missing.Add(leftItem);
                }
            }
            return instance;
        }
    }
}
