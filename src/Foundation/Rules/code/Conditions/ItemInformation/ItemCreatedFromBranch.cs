using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Conditions.ItemInformation
{
    /// <summary>
    /// Rule that checks if the branch template the item was created from is the branch template specified
    /// </summary>
    /// <typeparam name="TRuleContext"></typeparam>
    public sealed class ItemCreatedFromBranch<TRuleContext> : BaseWhenCondition<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// The backing field for the branch Id.
        /// </summary>
        private ID _branchId;

        /// <summary>
        /// Gets or sets the branch id.
        /// </summary>
        public ID BranchId
        {
            get
            {
                return _branchId;
            }

            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                _branchId = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCreatedFromBranch{TRuleContext}"/> class. 
        /// </summary>
        public ItemCreatedFromBranch()
        {
            _branchId = ID.Null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCreatedFromBranch{TRuleContext}"/> class. 
        /// </summary>
        /// <param name="branchId">
        /// The template id.
        /// </param>
        public ItemCreatedFromBranch([NotNull] ID branchId)
        {
            Assert.ArgumentNotNull(branchId, nameof(branchId));
            this._branchId = branchId;
        }

        /// <summary>
        /// The execute rule.
        /// </summary>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <returns>
        /// <c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected override bool ExecuteRule(TRuleContext ruleContext)
        {
            var item = ruleContext.Item;
            return item != null && item.BranchId == _branchId;
        }
    }
}
