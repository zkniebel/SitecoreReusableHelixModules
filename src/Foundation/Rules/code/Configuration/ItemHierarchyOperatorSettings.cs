using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Data;
using Sitecore.Xml;
using ZacharyKniebel.Foundation.Rules.Operators.ItemHierarchy;

namespace ZacharyKniebel.Foundation.Rules.Configuration
{
    public class ItemHierarchyOperatorSettings
    {
        private static ItemHierarchyOperatorSettings _settings;

        public static ItemHierarchyOperatorSettings Settings => 
            _settings ?? (_settings = Sitecore.Configuration.Factory.CreateObject(ConfigurationNodePath, true) as ItemHierarchyOperatorSettings);

        public static string ConfigurationNodePath = "ruleSettings/operators/itemHierarchyOperators";

        protected Dictionary<ID, ItemHierarchyType> Operators = new Dictionary<ID, ItemHierarchyType>();

        public Guid OperatorRoot { get; set; }

        /// <summary>
        /// Adds a new operator to <see cref="Operators"/>
        /// </summary>
        /// <param name="node">Configuration node</param>
        public void AddOperator(XmlNode node)
        {
            #region Parse and Validate itemHierachyType

            var name = XmlUtil.GetValue(node);
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Error adding ItemHierarchyOperator: operator's value is required");
            }

            ItemHierarchyType itemHierarchyType;
            if (!Enum.TryParse(name, out itemHierarchyType))
            {
                throw new Exception("Error adding ItemHierarchyOperator: operator's value is not a known ItemHierarchyType");
            }
            if (Operators.ContainsValue(itemHierarchyType))
            {
                throw new Exception("Error adding ItemHierarchyOperator: operator's value must be unique");
            }

            #endregion

            #region Parse and Validate value

            var rawId = XmlUtil.GetAttribute("itemId", node);
            Guid parsed;
            if (!Guid.TryParse(rawId, out parsed))
            {
                throw new Exception("Error adding ItemHierarchyOperator: operator's itemId must be a valid guid");
            }

            var id = new ID(rawId);
            if (Operators.ContainsKey(id))
            {
                throw new Exception("Error adding ItemHierarchyOperator: operator's itemId must be a unique Sitecore ID or Guid");
            }

            #endregion

            Operators.Add(id, itemHierarchyType);
        }

        /// <summary>
        /// Gets the <seealso cref="ItemHierarchyType"/> configured with the given Sitecore ID
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public ItemHierarchyType GetOperator(ID operatorId)
        {
            ItemHierarchyType itemHierarchyType;
            if (!Operators.TryGetValue(operatorId, out itemHierarchyType))
            {
                throw new Exception($"Error getting operator: configuration not found for operator with ID '{operatorId}'");
            }

            return itemHierarchyType;
        }

        /// <summary>
        /// Gets the <seealso cref="ItemHierarchyType"/> configured with the given Sitecore ID
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public ItemHierarchyType GetOperator(string operatorId)
        {
            Guid parsed;
            if (!Guid.TryParse(operatorId, out parsed))
            {
                throw new Exception($"Error getting operator: the given operator ID, '{operatorId}' is not in a valid Guid format");
            }

            return GetOperator(new ID(operatorId));
        }

        /// <summary>
        /// Gets a new instance of the <see cref="ItemHierarchyOperatorSettings"/> class
        /// </summary>
        /// <returns></returns>
        public static ItemHierarchyOperatorSettings GetSettings()
        {
            return Sitecore.Configuration.Factory.CreateObject(ConfigurationNodePath, true) as ItemHierarchyOperatorSettings;
        }
    }
}
