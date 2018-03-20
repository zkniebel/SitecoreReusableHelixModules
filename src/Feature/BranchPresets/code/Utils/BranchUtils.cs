using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;

namespace ZacharyKniebel.Feature.BranchPresets.Utils
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

        #region Methods for Relinking Branch References

        /// <summary>
        /// Utility method for relinking field references for an item within a branch instance
        /// </summary>
        public static void RelinkFieldReferencesForItemInBranchInstance(Item item, Item instanceRoot)
        {
            var itemTemplate = TemplateManager.GetTemplate(item.TemplateID, item.Database);
            var templateFields = itemTemplate.GetFields(true)
                .Where(field => !field.Name.StartsWith("__"));
            
            var branchPath = instanceRoot.Branch.InnerItem.Paths.FullPath;
            var branchPathLength = branchPath.Length;

            using (new SecurityDisabler())
            using (new EditContext(item))
            {
                foreach (var templateField in templateFields)
                {
                    var itemField = item.Fields[templateField.ID];

                    // multilist, treelist, etc.
                    if (FieldTypeManager.GetField(itemField) is MultilistField)
                    {
                        var field = (MultilistField)itemField;
                        RelinkBranchReferencesInMulitlistField(field, instanceRoot, branchPath, branchPathLength);
                    }
                    // drop link, grouped droplink, etc.
                    else if (FieldTypeManager.GetField(itemField) is LookupField)
                    {
                        var field = (LookupField)itemField;
                        RelinkBranchReferenceInLookupField(field, instanceRoot, branchPath, branchPathLength);
                    }
                    // drop tree, etc.
                    else if (FieldTypeManager.GetField(itemField) is ReferenceField)
                    {
                        var field = (ReferenceField)itemField;
                        RelinkBranchReferenceInReferenceField(field, instanceRoot, branchPath, branchPathLength);
                    }
                    // general link, etc.
                    else if (FieldTypeManager.GetField(itemField) is LinkField)
                    {
                        var field = (LinkField)itemField;
                        RelinkBranchReferenceInLinkField(field, instanceRoot, branchPath, branchPathLength);
                    }
                }
            }
        }

        /// <summary>
        /// Relinks the branch reference, if present, in the given field
        /// </summary>
        /// <typeparam name="TField">The type of the field</typeparam>
        /// <param name="field">The field to relink the reference in</param>
        /// <param name="currentTargetPathSelector">Expression to select the current target item's Sitecore path</param>
        /// <param name="updatedValueSelector">Expression to select the updated field value</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="branchPath">Sitecore path of the branch template</param>
        /// <param name="branchPathLength">Length of the branch template's Sitecore path</param>
        public static void RelinkBranchReferenceInCustomField<TField>(
                TField field, 
                Func<TField, string> currentTargetPathSelector, 
                Func<Item, string> updatedValueSelector,
                Item instanceRoot,
                string branchPath, 
                int? branchPathLength = null)
            where TField : CustomField
        {
            var targetItemPath = currentTargetPathSelector(field);
            if (string.IsNullOrEmpty(targetItemPath) || !targetItemPath.StartsWith(branchPath))
            {
                return;
            }

            var relativePath = targetItemPath.Substring(branchPathLength ?? branchPath.Length);
            // replace $name Sitecore tokens in path
            relativePath = relativePath
                .Replace("$name", instanceRoot.Name);

            var updatedTargetPath = instanceRoot.Parent.Paths.FullPath + relativePath;
            var updatedTargetItem = field.InnerField.Database.GetItem(updatedTargetPath);

            if (updatedTargetItem == null)
            {
                Log.Warn($"An item in the branch instance could not be found at the path \"{updatedTargetPath}\"", typeof(BranchUtils));
                return;
            }

            field.InnerField.Value = updatedValueSelector(updatedTargetItem);
        }

        /// <summary>
        /// Relinks the branch reference, if present, in the given field
        /// </summary>
        /// <param name="field">The field to relink the reference in</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="branchPath">Sitecore path of the branch template</param>
        /// <param name="branchPathLength">Length of the branch template's Sitecore path</param>
        public static void RelinkBranchReferenceInLinkField(LinkField field, Item instanceRoot, string branchPath, int? branchPathLength = null)
        {
            RelinkBranchReferenceInCustomField(
                field,
                linkField => linkField.TargetItem?.Paths.FullPath,
                item =>
                {
                    field.TargetID = item.ID;
                    field.Url = item.Paths.ContentPath;

                    return field.Value;
                },
                instanceRoot,
                branchPath,
                branchPathLength);
        }

        /// <summary>
        /// Relinks the branch reference, if present, in the given field
        /// </summary>
        /// <param name="field">The field to relink the reference in</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="branchPath">Sitecore path of the branch template</param>
        /// <param name="branchPathLength">Length of the branch template's Sitecore path</param>
        public static void RelinkBranchReferenceInReferenceField(ReferenceField field, Item instanceRoot, string branchPath, int? branchPathLength = null)
        {
            RelinkBranchReferenceInCustomField(
                field,
                referenceField => referenceField.TargetItem.Paths.FullPath,
                item => item.ID.ToString(),
                instanceRoot,
                branchPath,
                branchPathLength);
        }

        /// <summary>
        /// Relinks the branch reference, if present, in the given field
        /// </summary>
        /// <param name="field">The field to relink the reference in</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="branchPath">Sitecore path of the branch template</param>
        /// <param name="branchPathLength">Length of the branch template's Sitecore path</param>
        public static void RelinkBranchReferenceInLookupField(LookupField field, Item instanceRoot, string branchPath, int? branchPathLength = null)
        {
            RelinkBranchReferenceInCustomField(
                field,
                lookupField => lookupField.TargetItem?.Paths.FullPath,
                item => item.ID.ToString(),
                instanceRoot,
                branchPath,
                branchPathLength);
        }

        /// <summary>
        /// Relinks the branch references, if present, in the given field
        /// </summary>
        /// <param name="field">The field to relink the reference in</param>
        /// <param name="instanceRoot">Root item of the branch instance</param>
        /// <param name="branchPath">Sitecore path of the branch template</param>
        /// <param name="branchPathLength">Length of the branch template's Sitecore path</param>s
        public static void RelinkBranchReferencesInMulitlistField(MultilistField field, Item instanceRoot, string branchPath, int? branchPathLength = null)
        {
            var selectedItems = field.GetItems();
            var branchPathLen = branchPathLength ?? branchPath.Length;

            var needsUpdate = false;
            var updatedSelectedItems = new List<Item>();

            foreach (var selectedItem in selectedItems)
            {
                var selectedItemPath = selectedItem.Paths.FullPath;
                if (selectedItemPath.StartsWith(branchPath))
                {
                    needsUpdate = true;
                    var relativePath = selectedItemPath.Substring(branchPathLen);
                    // replace $name Sitecore tokens in path
                    relativePath = relativePath
                        .Replace("$name", instanceRoot.Name);

                    var targetPath = instanceRoot.Parent.Paths.FullPath + relativePath;
                    var targetItem = field.InnerField.Database.GetItem(targetPath);

                    if (targetItem == null)
                    {
                        Log.Warn($"An item in the branch instance could not be found at the path \"{targetPath}\"", typeof(BranchUtils));
                        continue;
                    }

                    updatedSelectedItems.Add(targetItem);
                }
                else
                {
                    updatedSelectedItems.Add(selectedItem);
                }
            }

            if (needsUpdate)
            {
                field.InnerField.Value = string.Join(
                    (string) "|",
                    (IEnumerable<string>) updatedSelectedItems
                        .Select(i => i.ID.ToString()));
            }
        }

        #endregion Methods for Relinking Branch References
    }
}
