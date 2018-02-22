using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Rules;
using ZacharyKniebel.Foundation.Rules.Extensions;
using ZacharyKniebel.Foundation.Rules.Configuration;

namespace ZacharyKniebel.Foundation.Rules.Operators.ItemHierarchy
{
    /// <remarks>
    /// Credit to Matt Gramolini for originally developing this class and the ItemHierarchy operator. This
    /// class has since been modified.
    /// </remarks>
    public static class ItemHierarchyOperatorUtils
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <param name="operatorId">The operator's item ID</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static IEnumerable<Item> GetHierarchyItems<TRuleContext>(TRuleContext ruleContext, string operatorId)
            where TRuleContext : RuleContext
        {
            var op = GetOperatorType(operatorId);

            switch (op)
            {
                case ItemHierarchyType.Ancestors:
                    return ruleContext.Item.GetAncestors();
                case ItemHierarchyType.AncestorsAndSelf:
                    return ruleContext.Item.GetAncestorsAndSelf();
                case ItemHierarchyType.Children:
                    return ruleContext.Item.Children;
                case ItemHierarchyType.ChildrenAndSelf:
                    return new[] { ruleContext.Item }.Concat(ruleContext.Item.Children);
                case ItemHierarchyType.Descendants:
                    return ruleContext.Item.GetDescendantsBreadthFirst();
                case ItemHierarchyType.DescendantsAndSelf:
                    return ruleContext.Item.GetDescendantsAndSelfBreadthFirst();
                case ItemHierarchyType.Parent:
                    return new[] { ruleContext.Item.Parent };
                case ItemHierarchyType.ParentAndSelf:
                    return new[] { ruleContext.Item.Parent, ruleContext.Item };
                case ItemHierarchyType.Self:
                    return new[] { ruleContext.Item };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the operator type.
        /// </summary>
        /// <param name="operatorId">The operator's item ID</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static ItemHierarchyType GetOperatorType(string operatorId)
        {
            return ItemHierarchyOperatorSettings.Settings.GetOperator(operatorId);
        }
    }
}
