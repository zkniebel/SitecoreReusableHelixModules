using System.Collections.Generic;
using Sitecore.Data;

using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace ZacharyKniebel.Foundation.Rules.Extensions
{
    /// <summary>
    /// Item extensions for use in rules code
    /// </summary>
    /// <remarks>
    /// This class has been marked internal so as not to conflict with extensions in other projects/modules
    /// when this module is reused. Feel free to mark this class public, if reuse of these extensions 
    /// in other projects is desired
    /// </remarks>
    internal static class ItemExtensions
    {
        /// <summary>
        /// If a child item does not exist, create it.
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="name">The name of the item to find or create.</param>
        /// <param name="templateId">The template to use if creating a new item.</param>
        /// <returns>An item matching the name specified under the parent item.</returns>
        public static Item FindOrCreateChildItem(this Item parent, string name, ID templateId)
        {
            Assert.IsNotNull(parent, "parent cannot be null.");

            Assert.IsNotNullOrEmpty(name, "name cannot be null or empty.");

            Assert.IsNotNull(templateId, "templateId cannot be null.");

            var child = parent.Database.GetItem(parent.Paths.FullPath + "/" + name);

            if (child == null)
            {
                name = ItemUtil.ProposeValidItemName(name);

                child = parent.Add(name, new TemplateID(templateId));
            }

            return child;
        }

        /// <summary>
        /// Gets the ancestors.
        /// </summary>
        /// <remarks>
        /// Performant method so long as developers do not combine with unnecessary eager enumeration
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetAncestors(this Item item)
        {
            var parent = item.Parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Gets the ancestors and self.
        /// </summary>
        /// <remarks>
        /// Performant method so long as developers do not combine with unnecessary eager enumeration
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetAncestorsAndSelf(this Item item)
        {
            yield return item;

            foreach (var ancestor in item.GetAncestors())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        /// Gets the descendants breadth first.
        /// </summary>
        /// <remarks>
        /// Highly performant method so long as developers do not combine with unnecessary 
        /// eager enumeration. Significantly better performance than Sitecore's <see cref="Item.Axes.GetDescendants"/>
        /// method. BFS in O(n log n) time (logarithmic time)
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetDescendantsBreadthFirst(this Item item)
        {
            var children = item.Children;
            var queue = new Queue<Item>(children.Count);

            foreach (Item child in children)
            {
                yield return child;
                queue.Enqueue(child);
            }

            foreach (var child in queue)
            {
                foreach (var descendant in child.GetDescendantsBreadthFirst())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Gets the descendants depth first.
        /// </summary>
        /// <remarks>
        /// Highly performant method so long as developers do not combine with unnecessary 
        /// eager enumeration. Significantly better performance than Sitecore's <see cref="Item.Axes.GetDescendants"/>
        /// method. DFS in O(n log n) time (logarithmic time)
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetDescendantsDepthFirst(this Item item)
        {
            foreach (Item child in item.Children)
            {
                yield return child;

                foreach (var descendant in child.GetDescendantsDepthFirst())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Gets the descendants and self breadth first.
        /// </summary>
        /// <remarks>
        /// Highly performant method so long as developers do not combine with unnecessary 
        /// eager enumeration. Significantly better performance than Sitecore's <see cref="Item.Axes.GetDescendants"/>
        /// method. BFS in O((n log n) + 1) time (logarithmic)
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetDescendantsAndSelfBreadthFirst(this Item item)
        {
            yield return item;

            foreach (var descendant in item.GetDescendantsBreadthFirst())
            {
                yield return descendant;
            }
        }

        /// <summary>
        /// Gets the descendants and self depth first.
        /// </summary>
        /// <remarks>
        /// Highly performant method so long as developers do not combine with unnecessary 
        /// eager enumeration. Significantly better performance than Sitecore's <see cref="Item.Axes.GetDescendants"/>
        /// method. DFS in O((n log n) + 1) time (logarithmic)
        /// </remarks>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerable<Item> GetDescendantsAndSelfDepthFirst(this Item item)
        {
            yield return item;

            foreach (var descendant in item.GetDescendantsDepthFirst())
            {
                yield return descendant;
            }
        }
    }
}