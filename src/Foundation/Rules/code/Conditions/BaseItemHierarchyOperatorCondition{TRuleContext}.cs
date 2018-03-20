using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using ZacharyKniebel.Foundation.Rules.Operators.ItemHierarchy;

namespace ZacharyKniebel.Foundation.Rules.Conditions
{
    /// <summary>
	/// Performs a predicate function on a group of items and saves the first item that meets the condition into
	/// the ruleContext.Parameters using the supplied ParameterKey
	/// </summary>
	/// <typeparam name="TRuleContext">The type of the rule context.</typeparam>
	/// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{TRuleContext}" />
	public abstract class BaseItemHierarchyOperatorCondition<TRuleContext> : BaseWhenCondition<TRuleContext> where TRuleContext : RuleContext
	{
		/// <summary>
		/// Gets or sets the operator identifier.
		/// </summary>
		/// <value>
		/// The operator identifier.
		/// </value>
		public string OperatorId { get; set; }

		/// <summary>
		/// Gets or sets the parameter key.
		/// </summary>
		/// <value>
		/// The parameter key.
		/// </value>
		public string ParameterKey { get; set; }

		/// <summary>
		/// The execute rule.
		/// </summary>
		/// <param name="ruleContext">The rule context.</param>
		/// <returns>
		/// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
		/// </returns>
		protected override bool ExecuteRule(TRuleContext ruleContext)
		{
            Assert.IsNotNullOrEmpty(OperatorId, "OperatorId is null or empty");

			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var item in ItemHierarchyOperatorUtils.GetHierarchyItems(ruleContext, OperatorId))
			{
			    if (!Condition(item))
			    {
			        continue;
			    }

			    if (!string.IsNullOrWhiteSpace(ParameterKey))
			    {
			        ruleContext.Parameters[ParameterKey] = item;
			    }
					
			    return true;
			}

			return false;
		}

		/// <summary>
		/// Conditions the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		protected abstract bool Condition(Item item);
	}
}
