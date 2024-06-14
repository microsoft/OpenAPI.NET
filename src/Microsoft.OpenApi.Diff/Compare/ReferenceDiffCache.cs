using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;

namespace Microsoft.OpenApi.Diff.Compare
{
    public abstract class ReferenceDiffCache<TC, TD>
    where TD : class
    {
        public Dictionary<CacheKey, TD> RefDiffMap { get; set; }

        protected ReferenceDiffCache()
        {
            RefDiffMap = new Dictionary<CacheKey, TD>();
        }

        protected string GetRefKey(string leftRef, string rightRef)
        {
            return leftRef + ":" + rightRef;
        }

        protected abstract TD ComputeDiff(
            HashSet<string> refSet, TC left, TC right, DiffContextBO context);

        public TD CachedDiff(
            HashSet<string> refSet,
            TC left,
            TC right,
            string leftRef,
            string rightRef,
            DiffContextBO context)
        {
            var areBothRefParameters = leftRef != null && rightRef != null;
            if (areBothRefParameters)
            {
                var key = new CacheKey(leftRef, rightRef, context);
                if (RefDiffMap.TryGetValue(key, out var changedFromRef))
                    return changedFromRef;
               
                var refKey = GetRefKey(leftRef, rightRef);
                if (refSet.Contains(refKey))
                    return null;
                
                refSet.Add(refKey);
                var changed = ComputeDiff(refSet, left, right, context);
                RefDiffMap.Add(key, changed);
                refSet.Remove(refKey);
                return changed;
            }
            
            return ComputeDiff(refSet, left, right, context);
        }
    }
}
