using Sitecore.Diagnostics;
using Sitecore.Pipelines.ItemProvider.AddFromTemplate;
using Sitecore.Rules;

namespace ZacharyKniebel.Foundation.Rules.Registrations.Pipelines.AddFromTemplate
{
    public class AddFromTemplateRuleContext : RuleContext, IPipelineArgsRuleContext<AddFromTemplateArgs>
    {
        /// <summary>
        /// Gets the args.
        /// </summary>
        public AddFromTemplateArgs Args { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFromTemplateRuleContext"/> class.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public AddFromTemplateRuleContext(AddFromTemplateArgs args)
        {
            Assert.IsNotNull(args, "args");
            Args = args;
            Item = args.ProcessorItem.InnerItem;
        }
    }
}
