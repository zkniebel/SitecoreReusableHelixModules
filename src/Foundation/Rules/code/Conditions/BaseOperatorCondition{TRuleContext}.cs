using System;
using System.Diagnostics;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Conditions
{
    /// <summary>
    /// The base class for the rules engine condition
    /// </summary>
    /// <typeparam name="TRuleContext">
    /// Type providing rule context.
    /// </typeparam>
    [UsedImplicitly]
    public abstract class BaseOperatorCondition<TRuleContext> : Sitecore.Rules.Conditions.OperatorCondition<TRuleContext>
        where TRuleContext : RuleContext
    { 
        /// <summary>
        /// Condition implementation.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        /// True if the item has layout details for the default device,
        /// otherwise False.
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
                Trace.TraceError(exception.ToString());
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