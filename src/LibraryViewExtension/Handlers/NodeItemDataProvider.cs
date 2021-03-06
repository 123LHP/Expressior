﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using CefSharp;
using Dynamo.Search;
using Dynamo.Search.SearchElements;
using Newtonsoft.Json;

namespace Dynamo.LibraryUI.Handlers
{
    class NodeData
    {
        public string creationName { get; set; }
        public string module { get; set; }
    }

    class LoadedTypeItem
    {
        public string fullyQualifiedName { get; set; }
        public string iconUrl { get; set; }
        public string contextData { get; set; }
        public string parameters { get; set; }
        public string itemType { get; set; }
        public string description { get; set; }
        public string keywords { get; set; }
    }

    class LoadedTypeData<T> where T : LoadedTypeItem
    {
        public List<T> loadedTypes { get; set; }
    }

    /// <summary>
    /// 为所有已经加载的节点提供json资源
    /// </summary>
    class NodeItemDataProvider : ResourceProviderBase
    {
        protected NodeSearchModel model;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items"></param>
        public NodeItemDataProvider(NodeSearchModel model) : base(false)
        {
            this.model = model;
        }

        public override Stream GetResource(IRequest request, out string extension)
        {
            extension = "json";
            return GetNodeItemDataStream(model.SearchEntries);
        }

        protected Stream GetNodeItemDataStream(IEnumerable<NodeSearchElement> searchEntries)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            var serializer = new JsonSerializer();

            var data = CreateObjectForSerialization(searchEntries);
            serializer.Serialize(sw, data);

            sw.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 构造LoadedTypeData对象用于序列化
        /// </summary>
        /// <param name="searchEntries"></param>
        /// <returns></returns>
        protected virtual object CreateObjectForSerialization(IEnumerable<NodeSearchElement> searchEntries)
        {
            var data = new LoadedTypeData<LoadedTypeItem>();
            data.loadedTypes = searchEntries
                //.Where(e => !e.ElementType.HasFlag(ElementTypes.Packaged))
                .Select(e => CreateLoadedTypeItem<LoadedTypeItem>(e)).ToList();
            return data;
        }

        /// <summary>
        /// 获取正在搜索的元素的全名(用于模糊搜索)
        /// </summary>
        public static string GetFullyQualifiedName(NodeSearchElement element)
        {
            //如果正在搜索的是包名(),则在前面加上pkg://前缀
            if (element.ElementType.HasFlag(ElementTypes.Packaged))
            {
                //Use FullCategory and name as read from _customization.xml file
                return string.Format("{0}{1}.{2}", "pkg://", element.FullCategoryName, element.Name);
            }
            //如果是一个自定义节点的名字
            else if (element.ElementType.HasFlag(ElementTypes.CustomNode))
            {
                //Use FullCategory and name as read from _customization.xml file
                return string.Format("{0}{1}", "dyf://", element.FullName);
            }
            return element.FullName;
        }

        /// <summary>
        /// Creates LoadedTypeItem from given node search element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal T CreateLoadedTypeItem<T>(NodeSearchElement element) where T: LoadedTypeItem, new()
        {
            var item = new T()
            {
                fullyQualifiedName = GetFullyQualifiedName(element),
                contextData = element.CreationName,
                iconUrl = new IconUrl(element.IconName, element.Assembly).Url,

                parameters = element.Parameters,
                itemType = element.Group.ToString().ToLower(),
                description = element.Description,
                keywords = element.SearchKeywords.Any()
                        ? element.SearchKeywords.Where(s => !string.IsNullOrEmpty(s)).Aggregate((x, y) => string.Format("{0}, {1}", x, y))
                        : string.Empty
            };

            //If this element is not a custom node then we are done. The icon url for custom node is different
            if (!element.ElementType.HasFlag(ElementTypes.CustomNode)) return item;

            var customNode = element as CustomNodeSearchElement;
            if (customNode != null)
            {
                item.iconUrl = new IconUrl(customNode.IconName, customNode.Path, true).Url;
            }

            return item;
        }
    }
}
