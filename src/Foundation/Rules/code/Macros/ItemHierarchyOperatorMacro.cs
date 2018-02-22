using System.Linq;
using System.Xml.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules.RuleMacros;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using ZacharyKniebel.Foundation.Rules.Configuration;

namespace ZacharyKniebel.Foundation.Rules.Macros
{
    /// <remarks>
    /// Credit to Matt Gramolini for originally developing the ItemHierarchy operator and macro. They
    /// have since been modified.
    /// </remarks>
    public class ItemHierarchyOperatorMacro : IRuleMacro
	{
		public void Execute(XElement element, string name, UrlString parameters, string value)
		{
			Assert.ArgumentNotNull(element, "element");
			Assert.ArgumentNotNull(name, "name");
			Assert.ArgumentNotNull(parameters, "parameters");
			Assert.ArgumentNotNull(value, "value");

			var selectItemOptions = new SelectItemOptions();

			Item item = null;
			if (!string.IsNullOrEmpty(value))
			{
				item = Client.ContentDatabase.GetItem(value);
			}

			var path = XElement.Parse(element.ToString()).FirstAttribute.Value;
			if (!string.IsNullOrEmpty(path))
			{
				var filterItem = Client.ContentDatabase.GetItem(path);
				if (filterItem != null)
				{
					selectItemOptions.FilterItem = filterItem;
				}
			}

            var rootItemId = new ID(ItemHierarchyOperatorSettings.Settings.OperatorRoot);

            selectItemOptions.Root = Client.ContentDatabase.GetItem(rootItemId);

			if (item == null)
			{
				if (selectItemOptions.Root != null)
				{
					item = selectItemOptions.Root.Children.FirstOrDefault();
				}
			}
            
			selectItemOptions.SelectedItem = item;
			selectItemOptions.Title = "Select Item Hierarchy Operator";
			selectItemOptions.Text = "Select the item hierarchal type to use in this rule.";
			selectItemOptions.Icon = "applications/32x32/media_stop.png";
			selectItemOptions.ShowRoot = false;

			SheerResponse.ShowModalDialog(selectItemOptions.ToUrlString().ToString(), "1200px", "700px", string.Empty, true);
		}
	}
}
