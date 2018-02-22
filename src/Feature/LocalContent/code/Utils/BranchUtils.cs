using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;

namespace ZacharyKniebel.Feature.LocalContent
{
	public class BranchUtils
    {
        #region Methods for Relinking Branch Datasources

        /// <summary>
        /// Utility method for relinking branch datasources
        /// </summary>
        public static void RelinkDatasourcesInBranchInstance(Item item, bool descendants = false)
        {
            RelinkDatasourcesForItemInBranchInstance(item, item);

            if (!descendants)
            {
                return;
            }

            foreach (var descendant in item.Axes.GetDescendants())
            {
                RelinkDatasourcesForItemInBranchInstance(descendant, item);
            }
        }

        /// <summary>
        /// Utility method for relinking datasources for an item within a branch instance
        /// </summary>
        /// <remarks>
        /// Adapted from original code, written by Kam Figy: 
        /// https://github.com/kamsar/BranchPresets/blob/master/BranchPresets/AddFromBranchPreset.cs
        /// </remarks>
        public static void RelinkDatasourcesForItemInBranchInstance(Item item, Item instanceRoot)
        {
            Action<RenderingDefinition> relinkRenderingDatasource =
                rendering =>
                    RelinkRenderingDatasourceForItemInBranch(item, instanceRoot, rendering);

            LayoutUtils.ApplyActionToAllRenderings(item, relinkRenderingDatasource);
        }

        /// <summary>
        /// Utility method for relinking the datasource for the supplied rendering on an item in the branch instance
        /// </summary>
        /// <remarks>
        /// Adapted from original code, written by Kam Figy: 
        /// https://github.com/kamsar/BranchPresets/blob/master/BranchPresets/AddFromBranchPreset.cs
        /// </remarks>
        /// <param name="item">Item that contains the rendering</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="rendering">Rendering to be relinked</param>
        public static void RelinkRenderingDatasourceForItemInBranch(Item item, Item instanceRoot, RenderingDefinition rendering)
        {
            var branchBasePath = item.Branch.InnerItem.Paths.FullPath;

            if (string.IsNullOrWhiteSpace(rendering.Datasource))
            {
                return;
            }

            var database = item.Database;

            // note: queries and multiple item datasources are not supported
            var renderingTargetItem = database.GetItem(rendering.Datasource);

            Assert.IsNotNull(
                renderingTargetItem,
                "Error while expanding branch template rendering datasources: data source {0} was not resolvable."
                    .FormatWith(rendering.Datasource));

            // if there was no valid target item OR the target item is not a child of the branch template we skip out
            if (renderingTargetItem == null ||
                !renderingTargetItem.Paths.FullPath.StartsWith(branchBasePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // get the path relative to the branch item
            var relativeRenderingPath =
                renderingTargetItem.Paths.FullPath.Substring(branchBasePath.Length);
            
            // replace $name Sitecore tokens in path
            relativeRenderingPath = relativeRenderingPath
                .Replace("$name", instanceRoot.Name);

            var newTargetPath = instanceRoot.Parent.Paths.FullPath + relativeRenderingPath;
            var newTargetItem = database.GetItem(newTargetPath);

            // if the target item was a valid under branch item, but the same relative path does not exist under the branch instance
            // we set the datasource to something invalid to avoid any potential unintentional edits of a shared data source item
            rendering.Datasource = newTargetItem?.ID.ToString() ?? "INVALID_BRANCH_SUBITEM_ID";
        }

        #endregion Methods for Relinking Branch Datasources
    }
}
