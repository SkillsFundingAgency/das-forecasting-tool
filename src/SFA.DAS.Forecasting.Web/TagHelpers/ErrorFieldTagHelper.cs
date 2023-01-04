using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.Forecasting.Web.TagHelpers;

[HtmlTargetElement("div", Attributes = "asp-fieldname")]
public class ErrorFieldTagHelper : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    private ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-fieldname")]
    public string FieldName { get; set; }
    
    [HtmlAttributeName("asp-additionalclass")]
    public string AdditionalClass { get; set; }


    
    public override void Process(TagHelperContext context, TagHelperOutput tagHelperOutput)
    {
        var fieldNames = FieldName.Split(",");
        if (fieldNames.Any(modelStateKey => ViewContext.ModelState.ContainsKey(modelStateKey) &&
                                            ViewContext.ModelState[modelStateKey]!.Errors.Any()))
        {
            tagHelperOutput.AddClass("error", HtmlEncoder.Default);
        }
        
        if (string.IsNullOrEmpty(AdditionalClass))
        {
            tagHelperOutput.AddClass(AdditionalClass, HtmlEncoder.Default);
        }
        tagHelperOutput.AddClass("form-group", HtmlEncoder.Default);
        
        tagHelperOutput.TagMode = TagMode.StartTagAndEndTag;
    }
}