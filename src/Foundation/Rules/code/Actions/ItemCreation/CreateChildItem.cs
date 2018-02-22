using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace ZacharyKniebel.Foundation.Rules.Actions.ItemCreation
{
    /// <summary>
    /// Action for creating a child item under the rule context item with the specified name and 
    /// from the specified template
    /// </summary>
    /// <typeparam name="TRuleContext">The type of the RuleContext</typeparam>
    public sealed class CreateChildItem<TRuleContext> : BaseRuleAction<TRuleContext> 
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// The backing field for the Template ID
        /// </summary>
        private ID _templateId;

        /// <summary>
        /// Gets or sets the template ID that the item should be created from
        /// </summary>
        public ID TemplateId
        {
            get
            {
                return _templateId;
            }

            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                _templateId = value;
            }
        }

        /// <summary>
        /// The backing field for the item Name
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets or sets the name of the item to create
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            } 
            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                _name = value;
            }
        }

        /// <summary>
        /// Creates the child item
        /// </summary>
        /// <param name="ruleContext"></param>
        protected override void ApplyRule(TRuleContext ruleContext)
        {
            using(new SecurityDisabler())
            {
                ruleContext.Item.Add(Name, new TemplateID(TemplateId));
            }
        }
    }
}