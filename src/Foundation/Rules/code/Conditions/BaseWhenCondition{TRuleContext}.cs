using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Conditions
{
    /// <summary>
    /// The base class for the rule engine
    /// </summary>
    /// <typeparam name="TRuleContext">
    /// Instance of Sitecore.Rules.Conditions.RuleContext.
    /// </typeparam>
    [UsedImplicitly]
    public abstract class BaseWhenCondition<TRuleContext> : Sitecore.Rules.Conditions.WhenCondition<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <returns>
        /// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected sealed override bool Execute([NotNull] TRuleContext ruleContext)
        {
            Assert.IsNotNull(ruleContext, "ruleContext");
            Assert.IsNotNull(ruleContext.Item, "ruleContext.Item");
            if (ruleContext.IsAborted)
            {
                return false;
            }

            try
            {
                Log.Debug("RuleAction " + GetType().Name + " started for " + ruleContext.Item.Name, this);

                var result = ExecuteRule(ruleContext);
                return result;
            }
            catch (Exception exception)
            {
                Log.Error("RuleAction " + GetType().Name + " failed.", exception, this);
            }

            Log.Debug("RuleAction " + GetType().Name + " ended for " + ruleContext.Item.Name, this);

            return false;
        }

        /// <summary>
        /// The execute rule.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        /// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected abstract bool ExecuteRule([NotNull] TRuleContext ruleContext);
    }
}