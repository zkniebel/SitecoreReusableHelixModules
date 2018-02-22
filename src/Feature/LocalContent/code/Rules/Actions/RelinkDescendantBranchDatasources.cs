using Sitecore.Rules;

namespace ZacharyKniebel.Feature.LocalContent.Rules.Actions
{
    /// <summary>
    /// Action used for local datasource relinking
    /// </summary>
    /// <remarks>
    /// Note that this action is particularly useful if you want to relink the datasources for subitems
    /// within the branch (i.e. adding a page and child pages from a single branch and relinking all of 
    /// their datasources).
    /// 
    /// When called from pipelines (e.g. the <addFromTemplate> pipeline), as opposed to eventing, there 
    /// is a significant gain in performance, over calling <seealso cref="RelinkItemBranchDatasources"/>
    /// from an event for each added item. 
    /// 
    /// For performance reasons, if you know that you only need to relink the datasources for the root
    /// item in the branch, it is recommended that you use <seealso cref="RelinkItemBranchDatasources"/>
    /// instead.
    /// </remarks>
    /// <typeparam name="TRuleContext"></typeparam>
    public sealed class RelinkDescendantsAndSelfBranchDatasources<TRuleContext> : Foundation.Rules.Actions.BaseRuleAction<TRuleContext>
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
            BranchUtils.RelinkDatasourcesInBranchInstance(ruleContext.Item, true);
        }
    }
}
