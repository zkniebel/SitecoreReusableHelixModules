using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.SecurityModel;
using Sitecore.Data.Fields;

namespace ZacharyKniebel.Foundation.Rules.Actions.Workflows
{
    public sealed class AssignWorkflow<TRuleContext> : BaseRuleAction<TRuleContext>
        where TRuleContext : RuleContext
    {
        /// <summary>
        /// Gets or sets the ID of the workflow that should be assigned to the item
        /// </summary>
        public ID WorkflowId { get; set; }

        protected override void ApplyRule(TRuleContext ruleContext)
        {
            var item = ruleContext.Item;

            var workflowField = (ReferenceField) item.Fields[Sitecore.FieldIDs.Workflow];
            if (workflowField.TargetID == WorkflowId)
            {
                return;
            }

            using (new SecurityDisabler())
            using (new EditContext(item))
            {
                workflowField.InnerField.Value = 
                    WorkflowId == (ID)null || WorkflowId == ID.Null
                        ? null
                        : WorkflowId.ToString();
            }
        }
    }
}