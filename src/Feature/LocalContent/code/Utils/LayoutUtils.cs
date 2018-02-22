using System;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.SecurityModel;

namespace ZacharyKniebel.Feature.LocalContent
{
    /// <summary>
    /// Layout Utilities class
    /// </summary>
    /// <remarks>
    /// Adapted from original code, written by Kam Figy: 
    /// https://github.com/kamsar/BranchPresets/blob/master/BranchPresets/AddFromBranchPreset.cs
    /// </remarks>
    public static class LayoutUtils
    {
        /// <summary>
        ///     Invokes Action on all Renderings on item
        /// </summary>
        public static void ApplyActionToAllRenderings(Item item, Action<RenderingDefinition> action)
        {
            ApplyActionToAllSharedRenderings(item, action);
            ApplyActionToAllFinalRenderings(item, action);
        }

        /// <summary>
        ///     Invokes Action on all Shared Renderings on item
        /// </summary>
        public static void ApplyActionToAllSharedRenderings(Item item, Action<RenderingDefinition> action)
        {
            ApplyActionToLayoutField(item, FieldIDs.LayoutField, action);
        }

        /// <summary>
        ///     Invokes Action on all Final Renderings on item
        /// </summary>
        public static void ApplyActionToAllFinalRenderings(Item item, Action<RenderingDefinition> action)
        {
            ApplyActionToLayoutField(item, FieldIDs.FinalLayoutField, action);
        }

        /// <summary>
        ///     Invokes Action on all Final Renderings on item
        /// </summary>
        private static void ApplyActionToLayoutField(Item item, ID fieldId, Action<RenderingDefinition> action)
        {
            var currentLayoutXml = LayoutField.GetFieldValue(item.Fields[fieldId]);
            if (string.IsNullOrEmpty(currentLayoutXml))
            {
                return;
            }

            var newXml = ApplyActionToLayoutXml(currentLayoutXml, action);
            if (newXml == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                using (new EditContext(item))
                {
                    // NOTE: when dealing with layouts its important to get and set the field value with LayoutField.Get/SetFieldValue()
                    // if you fail to do this you will not process layout deltas correctly and may instead override all fields (breaking full inheritance), 
                    // or attempt to get the layout definition for a delta value, which will result in your wiping the layout details when they get saved.
                    LayoutField.SetFieldValue(item.Fields[fieldId], newXml);
                }
            }
        }

        private static string ApplyActionToLayoutXml(string xml, Action<RenderingDefinition> action)
        {
            var layout = LayoutDefinition.Parse(xml);

            // normalize the output in case of any minor XML differences (spaces, etc)
            xml = layout.ToXml(); 

            // loop over devices in the rendering
            for (var deviceIndex = layout.Devices.Count - 1; deviceIndex >= 0; deviceIndex--)
            {
                var device = layout.Devices[deviceIndex] as DeviceDefinition;
                if (device == null)
                {
                    continue;
                }

                // loop over renderings within the device
                for (var renderingIndex = device.Renderings.Count - 1; renderingIndex >= 0; renderingIndex--)
                {
                    var rendering = device.Renderings[renderingIndex] as RenderingDefinition;
                    if (rendering == null)
                    {
                        continue;
                    }

                    // run the action on the rendering
                    action(rendering);
                }
            }

            var layoutXml = layout.ToXml();

            // save a modified layout value if necessary
            return layoutXml != xml ? layoutXml : null;
        }
    }
}
