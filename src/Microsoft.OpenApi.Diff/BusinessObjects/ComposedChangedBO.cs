using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public abstract class ComposedChangedBO : ChangedBO
    {
        protected ComposedChangedBO()
        {
        }

        public abstract List<(string Identifier, ChangedBO Change)> GetChangedElements();

        public override List<ChangedInfosBO> GetAllChangeInfoFlat(string identifier, List<string> parentPath = null)
        {
            var coreChangeInfo = GetCoreChangeInfo(identifier, parentPath);
            var changedElements = GetChangedElements();
            var returnList = changedElements
                .SelectMany(x => x.Change.GetAllChangeInfoFlat(x.Identifier, coreChangeInfo.Path))
                .Where(x => !x.ChangeType.IsUnchanged())
                .OrderBy(x => x.Path.Count)
                .ToList();

            returnList.Add(coreChangeInfo);

            return returnList;
        }

        public override DiffResultBO IsChanged()
        {
            var elementsResultMax = GetChangedElements()
                .Where(x => x.Change != null)
                .Select(x => (int)x.Change.IsChanged().DiffResult)
                .DefaultIfEmpty(0)
                .Max();

            var elementsResult = new DiffResultBO((DiffResultEnum)elementsResultMax);
            
            return IsCoreChanged().DiffResult > elementsResult.DiffResult ? IsCoreChanged() : elementsResult;
        }

        protected List<ChangedInfoBO> GetCoreChangeInfosOfComposed<T>(List<T> increased, List<T> missing, Func<T, string> identifierSelector)
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();

            foreach (var listElement in increased)
            {
                returnList.Add(ChangedInfoBO.ForAdded(elementType, identifierSelector(listElement)));
            }

            foreach (var listElement in missing)
            {
                returnList.Add(ChangedInfoBO.ForRemoved(elementType, identifierSelector(listElement)));
            }
            return returnList;
        }
    }
}
