using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Pipelines.GetRenderedRuleElements
{
    public class GetElementFolders : Sitecore.Pipelines.Rules.Taxonomy.GetElementFolders
    {
        private IEnumerable<Item> GetElementFoldersDepthFirst(Item parentItem, string tagId)
        {
            foreach (Item child in parentItem.Children)
            {
                if (child.TemplateID == RuleIds.ElementFolderTemplateID)
                {
                    var shouldInclude = child.Children
                        .Any(grandchild
                            =>
                                grandchild.TemplateID == RuleIds.TagDefinitionsFolderTemplateID
                                && grandchild.Children
                                    .Any(greatgrandchild
                                        =>
                                            greatgrandchild.TemplateID == RuleIds.TagDefinitionTemplateID
                                            && greatgrandchild["Tags"].Contains(tagId)));
                    if (shouldInclude)
                    {
                        yield return child;
                    }
                }
                else if (child.TemplateID == Sitecore.TemplateIDs.Folder)
                {
                    foreach (var descendant in GetElementFoldersDepthFirst(child, tagId))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        protected override IEnumerable<Item> GetConditionFolders(Item tag)
        {
            var elementsRootFolder = Sitecore.Client.ContentDatabase.GetItem(ID.Parse(RuleIds.ElementsFolderID));
            if (elementsRootFolder == null)
            {
                return Enumerable.Empty<Item>();
            }

            return elementsRootFolder == null
                ? Enumerable.Empty<Item>()
                : GetElementFoldersDepthFirst(elementsRootFolder, tag.ID.ToString());
        }
    }
}