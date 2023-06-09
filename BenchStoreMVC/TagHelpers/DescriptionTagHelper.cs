﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BenchStoreMVC.TagHelpers
{
    [HtmlTargetElement("div", Attributes = ForAttributeName)]
    [HtmlTargetElement("p", Attributes = ForAttributeName)]
    [HtmlTargetElement("span", Attributes = ForAttributeName)]
    public sealed class DescriptionForTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-description-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Attributes.SetAttribute("class", "text-muted");

            var description = For.Metadata.Description;
            if (description != null)
            {
                // Do not update the content if another tag helper
                // targeting this element has already done so.
                if (!output.IsContentModified)
                {
                    var childContent = await output.GetChildContentAsync();
                    if (childContent.IsEmptyOrWhiteSpace)
                    {
                        output.Content.SetHtmlContent(description);
                    }
                    else
                    {
                        output.Content.SetHtmlContent(childContent);
                    }
                }
            }
        }
    }
}

