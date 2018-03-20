using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using ZacharyKniebel.Foundation.Rules.Conditions;
using ZacharyKniebel.Foundation.Rules.Operators.ItemHierarchy;

namespace ZacharyKniebel.Foundation.Rules.Actions
{
    /// <summary>
	/// Base class for rule action to be executed on the selected item hierarchy
	/// </summary>
	/// <typeparam name="TRuleContext">The type of the rule context.</typeparam>
	/// <seealso cref="BaseRuleAction{TRuleContext}" />
	public abstract class BaseItemHierarchyOperatorAction<TRuleContext> : Sitecore.Rules.Actions.RuleAction<TRuleContext>
        where TRuleContext : RuleContext
    {
		/// <summary>
		/// Gets or sets the operator identifier.
		/// </summary>
		/// <value>
		/// The operator identifier.
		/// </value>
		public string OperatorId { get; set; }

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
	        Assert.IsNotNullOrEmpty(OperatorId, "OperatorId is null or empty");

            if (ruleContext.IsAborted)
	        {
	            return;
	        }

	        try
	        {
	            var msg = "RuleAction " + GetType().Name + " started for " + ruleContext.Item.Name;

	            Trace.TraceInformation(msg);

	            var hierarchyItems = ItemHierarchyOperatorUtils.GetHierarchyItems(ruleContext, OperatorId);
	            ApplyRule(hierarchyItems, ruleContext);
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
        /// Applies the action on each of the given items
        /// </summary>
        /// <param name="items">The items to apply the action on</param>
        /// <param name="ruleContext">The rule context</param>
        protected virtual void ApplyRule(IEnumerable<Item> items, TRuleContext ruleContext)
        {
            foreach (var item in items)
            {
                ApplyRule(item, ruleContext);
            }
        }

	    /// <summary>
	    /// Applies the action to the given item
	    /// </summary>
	    /// <param name="item">
	    /// The item to execute the action on
	    /// </param>
	    /// <param name="ruleContext">
	    /// The rule context
	    /// </param>
	    protected abstract void ApplyRule(Item item, TRuleContext ruleContext);
    }
}
