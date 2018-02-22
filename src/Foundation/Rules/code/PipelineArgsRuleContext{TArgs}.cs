using Sitecore.Diagnostics;

namespace ZacharyKniebel.Foundation.Rules
{
    /// <summary>
    /// The pipeline args rule context.
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments.</typeparam>
    public class PipelineArgsRuleContext<TArgs> : RuleContext<TArgs>, IPipelineArgsRuleContext<TArgs>
        where TArgs : Sitecore.Pipelines.PipelineArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineArgsRuleContext{TArgs}"/> class.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public PipelineArgsRuleContext(TArgs args) : base(args)
        {
            Assert.IsNotNull(args, "args");
            Assert.IsNotNull(args.ProcessorItem, "ProcessorItem is null");
            this.Item = args.ProcessorItem.InnerItem;
        }
    }
}