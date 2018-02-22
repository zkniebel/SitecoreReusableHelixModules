using Sitecore.Pipelines;

namespace ZacharyKniebel.Foundation.Rules
{
	/// <summary>
    /// Generic rule context used when passing pipeline args into the rules engine
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments.</typeparam>
    public interface IPipelineArgsRuleContext<out TArgs> : IRuleContext<TArgs>
		where TArgs : PipelineArgs
	{
    }
}