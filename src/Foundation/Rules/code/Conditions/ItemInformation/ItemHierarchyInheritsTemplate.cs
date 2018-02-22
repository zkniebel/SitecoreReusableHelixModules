using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Conditions.ItemInformation
{

    /// <summary>
    /// Using selected Item Hierarchy, determine if any of the items inherit from the selected template id
    /// </summary>
    /// <typeparam name="TRuleContext">The type of the rule context.</typeparam>
    /// <seealso cref="Conditions.Base.ItemHierarchyOperatorCondition{TRuleContext}" />
	public class ItemHierarchyInheritsTemplate<TRuleContext> 
        : BaseItemHierarchyOperatorCondition<TRuleContext> where TRuleContext : RuleContext
	{
		/// <summary>
		/// Gets or sets the template identifier.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		public ID TemplateId { get; set; }

		/// <summary>
		/// Conditions the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		protected override bool Condition(Item item)
		{
			var result = item.TemplateID == TemplateId ||
			             (TemplateManager.GetTemplate(item)?.InheritsFrom(TemplateId) ?? false);

			return result;
		}
	}
}
