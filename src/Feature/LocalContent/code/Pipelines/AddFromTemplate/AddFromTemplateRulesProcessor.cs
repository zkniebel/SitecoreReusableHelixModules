using System;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ItemProvider.AddFromTemplate;
using ZacharyKniebel.Foundation.Rules;

namespace ZacharyKniebel.Feature.LocalContent.Pipelines.AddFromTemplate
{
	public class AddFromTemplateRulesProcessor : AddFromTemplateProcessor
	{
		/// <summary>
		///     Gets or sets the rule folder id.
		/// </summary>
		public string RuleFolderId { get; set; }
        
		/// <summary>
		///     Processes the specified upload arguments.
		/// </summary>
		/// <param name="args">
		///     The arguments.
		/// </param>
		public override void Process([NotNull] AddFromTemplateArgs args)
		{
			// this is managed in configuration (runIfAborted=true would have to be set to override the value)
			if (args.Aborted)
			{
				return;
			}

			Assert.IsNotNull(args.FallbackProvider, "FallbackProvider is null");

			try
			{
				var item = args.FallbackProvider.AddFromTemplate(args.ItemName, args.TemplateId, args.Destination, args.NewId);
				if (item == null)
				{
					return;
				}

				args.ProcessorItem = args.Result = item;
			}
			catch (Exception ex)
			{
				Log.Error("AddFromTemplateRulesProcessor failed. Removing partially created item if it exists.", ex, this);

				var item = args.Destination.Database.GetItem(args.NewId);
				item?.Delete();

				throw;
			}

			if (AddFromTemplateRulesDisabler.IsActive)
			{
				return;
			}

			ID id;
			if (string.IsNullOrWhiteSpace(RuleFolderId)
			    || !Settings.Rules.ItemEventHandlers.RulesSupported(args.Destination.Database)
			    || !ID.TryParse(RuleFolderId, out id))
			{
				return;
			}

			var ruleContext = new PipelineArgsRuleContext<AddFromTemplateArgs>(args);

			RuleManager.RunRules(ruleContext, id);
		}
	}
}