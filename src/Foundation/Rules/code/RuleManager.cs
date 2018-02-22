using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace ZacharyKniebel.Foundation.Rules
{

    /// <summary>
    /// Manages the execution of the rules
    /// </summary>
    public static class RuleManager
	{
        /// <summary>
        /// Runs the rules in the rules folder with the given ID
        /// </summary>
        /// <param name="ruleContext"></param>
        /// <param name="rulesFolderId">The rules folder.</param>
        public static void RunRules<TRuleContext>(TRuleContext ruleContext, ID rulesFolderId)
            where TRuleContext : RuleContext
        {
            try
            {
                Assert.ArgumentNotNull(ruleContext, "ruleContext is null");
                Assert.ArgumentNotNull(rulesFolderId, "rulesFolderId is null");

                Assert.IsNotNull(ruleContext.Item, "ruleContext.Item is null");
                Assert.IsNotNull(ruleContext.Item.Database, "ruleContext.Item.Database is null");
            } 
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, typeof(RuleManager));
                return;
            }
            
            if (!Sitecore.Configuration.Settings.Rules.ItemEventHandlers.RulesSupported(ruleContext.Item.Database))
            {
                return;
            }

            Item rulesFolderItem;
            using (new SecurityDisabler())
            {
                rulesFolderItem = ruleContext.Item.Database.GetItem(rulesFolderId);
                if (rulesFolderItem == null)
                {
                    return;
                }
            }

            RunRules(ruleContext, rulesFolderItem);
        }

        /// <summary>
        /// Runs the rules in the given rules folder
        /// </summary>
        /// <param name="ruleContext"></param>
        /// <param name="rulesFolderId">The rules folder.</param>
        public static void RunRules<TRuleContext>(TRuleContext ruleContext, Item rulesFolderItem)
            where TRuleContext : RuleContext
        {
            try
            {
                Assert.ArgumentNotNull(ruleContext, "ruleContext is null");
                Assert.ArgumentNotNull(rulesFolderItem, "rulesFolderItem is null");

                Assert.IsNotNull(rulesFolderItem.Database, "rulesFolderItem.Database is null");

                if (!Sitecore.Configuration.Settings.Rules.ItemEventHandlers.RulesSupported(rulesFolderItem.Database))
                {
                    return;
                }

                var rules = RuleFactory.GetRules<TRuleContext>(rulesFolderItem, "Rule");
                if (rules == null || rules.Count == 0)
                {
                    return;
                }

                rules.Run(ruleContext);
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception, typeof(RuleManager));
            }
        }
    }
}