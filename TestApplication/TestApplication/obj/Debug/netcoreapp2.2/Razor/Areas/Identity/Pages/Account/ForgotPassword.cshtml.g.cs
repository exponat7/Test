#pragma checksum "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\Account\ForgotPassword.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a70e72d2eebd02dc32de8e865e4ce39497537a23"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(TestApplication.Areas.Identity.Pages.Account.Areas_Identity_Pages_Account_ForgotPassword), @"mvc.1.0.razor-page", @"/Areas/Identity/Pages/Account/ForgotPassword.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.RazorPageAttribute(@"/Areas/Identity/Pages/Account/ForgotPassword.cshtml", typeof(TestApplication.Areas.Identity.Pages.Account.Areas_Identity_Pages_Account_ForgotPassword), null)]
namespace TestApplication.Areas.Identity.Pages.Account
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#line 2 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\_ViewImports.cshtml"
using TestApplication.Areas.Identity;

#line default
#line hidden
#line 3 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\_ViewImports.cshtml"
using TestApplication.Areas.Identity.Models;

#line default
#line hidden
#line 1 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\Account\_ViewImports.cshtml"
using TestApplication.Areas.Identity.Pages.Account;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a70e72d2eebd02dc32de8e865e4ce39497537a23", @"/Areas/Identity/Pages/Account/ForgotPassword.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6e1eec3e4a3bb7879b71847b1b1dfb7050e40cfb", @"/Areas/Identity/Pages/_ViewImports.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"528c142d487bf978d495a536a7814ace702ba13e", @"/Areas/Identity/Pages/Account/_ViewImports.cshtml")]
    public class Areas_Identity_Pages_Account_ForgotPassword : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 3 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\Account\ForgotPassword.cshtml"
  
    ViewData["Title"] = "Забыли пароль?";

#line default
#line hidden
            BeginContext(85, 6, true);
            WriteLiteral("\r\n<h1>");
            EndContext();
            BeginContext(92, 17, false);
#line 7 "D:\Projects\C#\TestApplication\TestApplication\Areas\Identity\Pages\Account\ForgotPassword.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(109, 39, true);
            WriteLiteral("</h1>\r\n<h4>Так привет!!!</h4>\r\n<hr />\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ForgotPasswordModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ForgotPasswordModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ForgotPasswordModel>)PageContext?.ViewData;
        public ForgotPasswordModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
