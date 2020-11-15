using Sitecore.Abstractions;
using Sitecore.Connector.CMP.Pipelines.ImportEntity;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System.Linq;
using Sitecore.Data;
using System;
using Sitecore.Connector.CMP;

namespace Sitecore.CMP.Connector.Extensions.LinkField.Pipelines
{

    public class SaveLinkFieldValues : ImportEntityProcessor
    {
        public SaveLinkFieldValues(BaseLog logger): base(logger)
        {
            logger.Info("Inside SaveLinkFieldValues constructor", this.GetType().Name);
        }

        public override void Process(ImportEntityPipelineArgs args, BaseLog logger)
        {
            Assert.IsNotNull(args.Item, "The item is null.");
            Assert.IsNotNull(args.Language, "The language is null.");

            using (new SecurityDisabler())
            {
                using (new LanguageSwitcher(args.Language))
                {
                    try
                    {
                        args.Item.Editing.BeginEdit();                        
                        foreach (var item in from i in args.EntityMappingItem.Children
                                              where i.TemplateID == Constants.LinkFieldMappingTypeId
                                              select i)
                        {
                            var cmpLinkLabelFieldName = item[Constants.LinkFieldMappingCmpLinkLabelFieldNameFieldId];
                            var cmpLinkFieldName = item[Constants.LinkFieldMappingCmpLinkFieldNameFieldId];
                            var sitecoreFieldName = item[Constants.LinkFieldMappingSitecoreFieldNameFieldId];

                            try
                            {
                                var linkLabel = args.Entity.GetPropertyValue<string>(cmpLinkLabelFieldName);
                                var link = args.Entity.GetPropertyValue<string>(cmpLinkFieldName);

                                if (!string.IsNullOrEmpty(link))
                                {
                                    LinkType linkType;
                                    if (link.ToLowerInvariant().StartsWith("http") || link.ToLowerInvariant().StartsWith("www"))
                                        linkType = LinkType.External;
                                    else
                                        linkType = LinkType.Internal;

                                    args.Item[sitecoreFieldName] = GetLinkElement(link, linkLabel, linkType);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.Error(Helper.GetLogMessageText(
                                    $"An error occurred during converting '{(object) cmpLinkLabelFieldName}' field to '{(object) sitecoreFieldName}' field. Image Field mapping ID: '{(object) item.ID}'."), ex, (object)this);
                            }
                        }
                    }
                    finally
                    {
                        args.Item.Editing.EndEdit();
                    }
                }
            }
        }

        private static string GetLinkElement(string link, string linkLabel, LinkType linkType)
        {
            if (linkType == LinkType.External)
                return $"<link text=\"{linkLabel}\" linktype=\"external\" url=\"{link}\" anchor=\"\" target=\"\" />";
            else //Internal link
            {
                //If the link is an Sitecore ID (Guid), then return the link with that ID
                if (ID.TryParse(link, out ID linkID))
                {
                    return $"<link text=\"{linkLabel}\" anchor=\"\" linktype=\"internal\" class=\"\" title=\"\" target=\"\" querystring=\"\" id=\"{linkID}\" />";
                }

                //Else the link is an sitecore item path, try to get the item and its ID
                var master = Sitecore.Configuration.Factory.GetDatabase("master");
                var linkItem = master.GetItem(link);

                return linkItem == null ? string.Empty : $"<link text=\"{linkLabel}\" anchor=\"\" linktype=\"internal\" class=\"\" title=\"\" target=\"\" querystring=\"\" id=\"{linkItem.ID}\" />";
            }
        }

        private enum LinkType
        {
            Internal,
            External
        }

    }
}