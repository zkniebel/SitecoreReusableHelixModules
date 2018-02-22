using System;
using System.Diagnostics;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Actions
{
    /// <summary>
    /// The rule action.
    /// </summary>
    /// <typeparam name="TRuleContext">The type of the rule context.</typeparam>
    [UsedImplicitly]
    public abstract class BaseRuleAction<TRuleContext> : Sitecore.Rules.Actions.RuleAction<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// Action implementation.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        public sealed override void Apply([NotNull] TRuleContext ruleContext)
        {
            Assert.IsNotNull(ruleContext, "ruleContext");
            Assert.IsNotNull(ruleContext.Item, "ruleContext.Item");
            if (ruleContext.IsAborted)
            {
                return;
            }

            try
            {
                var msg = "RuleAction " + GetType().Name + " started for " + ruleContext.Item.Name;

                Trace.TraceInformation(msg);

                ApplyRule(ruleContext);
            }
            catch (Exception exception)
            {
                var msg = "RuleAction " + GetType().Name + " failed.";
                Log.Error(msg, exception, this);
                Trace.TraceError(msg);
            }

            var message = "RuleAction " + GetType().Name + " ended for " + ruleContext.Item.Name;

            Trace.TraceInformation(message);
        }
        
        /// <summary>
        /// The apply rule.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        protected abstract void ApplyRule([NotNull] TRuleContext ruleContext);
    }
}