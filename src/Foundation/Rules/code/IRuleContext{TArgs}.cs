using Sitecore.Data.Items;

namespace ZacharyKniebel.Foundation.Rules
{
    public interface IRuleContext<out TArgs> where TArgs : class
	{
		/// <summary>
		/// Gets the args.
		/// </summary>
		TArgs Args { get; }

		/// <summary>
		/// Gets the processor item.
		/// </summary>
		Item Item { get; }
	}
}