#pragma checksum "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "23da4475275a5b81214bc4f09b3d3854c5f1ea6d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home__TabsView), @"mvc.1.0.view", @"/Views/Home/_TabsView.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/_TabsView.cshtml", typeof(AspNetCore.Views_Home__TabsView))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\Projects\C#\TestApplication\TestApplication\Views\_ViewImports.cshtml"
using TestApplication;

#line default
#line hidden
#line 2 "D:\Projects\C#\TestApplication\TestApplication\Views\_ViewImports.cshtml"
using TestApplication.Models;

#line default
#line hidden
#line 4 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
using TestApplication.Areas.Identity.Managers;

#line default
#line hidden
#line 5 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
using TestApplication.Areas.Identity.Models;

#line default
#line hidden
#line 6 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
using TestApplication.Models.ViewModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"23da4475275a5b81214bc4f09b3d3854c5f1ea6d", @"/Views/Home/_TabsView.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c18df14659d9ffcf730f540f4c694666d270b7cb", @"/Views/_ViewImports.cshtml")]
    public class Views_Home__TabsView : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<TabModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(68, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(209, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(276, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(295, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 12 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
  
    bool isManager = await CustomUserManager.IsInRoleAsync(User, "manager");
    bool isDirector = await CustomUserManager.IsInRoleAsync(User, "director");

#line default
#line hidden
            BeginContext(462, 173, true);
            WriteLiteral("\r\n<div class=\"tab-container\">\r\n    <div style=\"display:flex;\">\r\n        <div id=\"IncomongTabId\" class=\"tab-ctrl selected-tab\" onclick=\"clickTab(this.id,0);\">Входящие</div>\r\n");
            EndContext();
#line 20 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
         if (isDirector || isManager)
        {

#line default
#line hidden
            BeginContext(685, 101, true);
            WriteLiteral("            <div id=\"UsersTabId\" class=\"tab-ctrl\" onclick=\"clickTab(this.id,1);\">Пользователи</div>\r\n");
            EndContext();
#line 23 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
        }

#line default
#line hidden
            BeginContext(797, 8, true);
            WriteLiteral("        ");
            EndContext();
#line 24 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
         if (isManager)
        {

#line default
#line hidden
            BeginContext(833, 106, true);
            WriteLiteral("            <div id=\"AdminTabId\" class=\"tab-ctrl\" onclick=\"clickTab(this.id,2);\">Администрирование</div>\r\n");
            EndContext();
#line 27 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
        }

#line default
#line hidden
            BeginContext(950, 61, true);
            WriteLiteral("    </div>\r\n    <div id=\"TabContentId\" class=\"tab-content\">\r\n");
            EndContext();
#line 31 "D:\Projects\C#\TestApplication\TestApplication\Views\Home\_TabsView.cshtml"
          await Html.RenderPartialAsync("_IncomingTabView", Model.Requests);

#line default
#line hidden
            BeginContext(1223, 18, true);
            WriteLiteral("    </div>\r\n</div>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public CustomUserManager<CustomIdentityUser> CustomUserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TabModel> Html { get; private set; }
    }
}
#pragma warning restore 1591