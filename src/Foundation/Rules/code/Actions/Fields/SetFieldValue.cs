using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace ZacharyKniebel.Foundation.Rules.Actions.Fields
{
    public sealed class SetFieldValue<TRuleContext> : BaseRuleAction<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// Gets or sets the name of the field to update.
        /// </summary>
        /// <value>
        /// The name of the field to be updated.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the value to be set as the field value.
        /// </summary>
        /// <value>
        /// The value the field should be updated to.
        /// </value>
        public string Value { get; set; }

        protected override void ApplyRule(TRuleContext ruleContext)
        {
            var item = ruleContext.Item;

            var currentValue = item[FieldName];
            if (currentValue == Value)
            {
                return;
            }

            using (new SecurityDisabler())
            using (new EditContext(item))
            {
                item[FieldName] = Value;
            }
        }
    }
}