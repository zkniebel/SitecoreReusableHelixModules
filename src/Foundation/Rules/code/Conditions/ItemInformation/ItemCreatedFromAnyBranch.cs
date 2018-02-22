using Sitecore.Data;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Conditions.ItemInformation
{
    /// <summary>
    /// Rule that checks if the item was created from a branch template
    /// </summary>
    /// <typeparam name="TRuleContext"></typeparam>
    public sealed class ItemCreatedFromAnyBranch<TRuleContext> : BaseWhenCondition<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// The execute rule.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <returns>
        /// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected override bool ExecuteRule(TRuleContext ruleContext)
        {
            var item = ruleContext.Item;
            return item != null && item.BranchId != ID.Null;
        }
    }
}
