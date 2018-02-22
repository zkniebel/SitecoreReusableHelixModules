using Sitecore.Rules;
using ZacharyKniebel.Feature.LocalContent;

namespace ZacharyKniebel.Sitecore.Rules.Actions
{
    /// <summary>
    /// Action used for local datasource relinking
    /// </summary>
    /// <remarks>
    /// It is recommended that this action be used when relinking the datasources for 
    /// the branch root item, only.
    /// </remarks>
    /// <typeparam name="TRuleContext"></typeparam>
    public sealed class RelinkItemBranchDatasources<TRuleContext> : Foundation.Rules.Actions.BaseRuleAction<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// The apply rule.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        protected override void ApplyRule(TRuleContext ruleContext)
        {
            BranchUtils.RelinkDatasourcesInBranchInstance(ruleContext.Item);
        }
    }
}
