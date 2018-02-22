using Sitecore.Common;

namespace ZacharyKniebel.Foundation.SitecoreExtensions.Common
{
	/// <summary>
	///     Base class for a custom disposable disabler
	/// </summary>
	/// <remarks>
	///     This class is meant to be implemented by specific disabler types. An example of a disabler would be
	///     a disabler for a particular pipeline that is checked in that pipeline's processors to decide whether
	///     or not to actually execute the processor. Sitecore includes several OOTB disablers, like the
	///     EventDisabler
	/// </remarks>
	/// <typeparam name="TSwitchType"></typeparam>
	public abstract class Disabler<TSwitchType> : Switcher<DisablerState, TSwitchType>
	{
		// ReSharper disable once PublicConstructorInAbstractClass
		public Disabler() : base(DisablerState.Enabled)
		{
		}

		public static bool IsActive => CurrentValue == DisablerState.Enabled;
	}
}