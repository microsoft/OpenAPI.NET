using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public abstract class ChangedListBO<T> : ChangedBO
    {
        public readonly DiffContextBO Context;
        public readonly IList<T> OldValue;
        public readonly IList<T> NewValue;

        public List<T> Increased { get; set; }
        public List<T> Missing { get; set; }
        public List<T> Shared { get; set; }

        protected ChangedListBO(IList<T> oldValue, IList<T> newValue, DiffContextBO context) 
        {
            OldValue = oldValue;
            NewValue = newValue;
            Context = context;
            Shared = new List<T>();
            Increased = new List<T>();
            Missing = new List<T>();
        }

        public override DiffResultBO IsChanged()
        {
            if (Missing.IsNullOrEmpty() && Increased.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            return IsItemsChanged();
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();

            foreach (var listElement in Increased)
            {
                returnList.Add(ChangedInfoBO.ForAdded(elementType, listElement.ToString()));
            }

            foreach (var listElement in Missing)
            {
                returnList.Add(ChangedInfoBO.ForRemoved(elementType, listElement.ToString()));
            }
            return returnList;
        }

        public abstract DiffResultBO IsItemsChanged();

        //public class SimpleChangedList : ChangedListBO<T>
        //{
        //    protected SimpleChangedList(List<T> oldValue, List<T> newValue) : base(oldValue, newValue, null)
        //    {
        //    }

        //    public override DiffResultBO IsItemsChanged()
        //    {
        //        return new DiffResultBO(DiffResultEnum.Unknown);
        //    }
        //}
    }
}
