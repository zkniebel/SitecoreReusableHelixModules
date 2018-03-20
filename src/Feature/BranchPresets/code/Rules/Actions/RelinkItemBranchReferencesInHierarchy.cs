using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules;
using ZacharyKniebel.Feature.BranchPresets.Utils;

namespace ZacharyKniebel.Feature.BranchPresets.Rules.Actions
{
    /// <summary>
    /// Action used for item references relinking when creating a new instance of a branch template
    /// </summary>
    /// <remarks>
    /// Note that this action is meant to relink reference fields that point to items within the branch from
    /// which the new instance was created. This action makes such references instead point at the new instances
    /// of the referenced items.
    /// instead.
    /// </remarks>
    /// <typeparam name="TRuleContext"></typeparam>
    public sealed class RelinkItemBranchReferencesInHierarchy<TRuleContext> : Foundation.Rules.Actions.BaseItemHierarchyOperatorAction<TRuleContext>
        where TRuleContext : RuleContext
    {
        protected override void ApplyRule(IEnumerable<Item> items, TRuleContext ruleContext)
        {
            using (new BulkUpdateContext())
            {
                base.ApplyRule(items, ruleContext);
            }        
        }

        protected override void ApplyRule(Item item, TRuleContext ruleContext)
        {
            BranchUtils.RelinkFieldReferencesForItemInBranchInstance(item, ruleContext.Item);
        }
    }
}
