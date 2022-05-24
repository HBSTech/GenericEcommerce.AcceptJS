using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using XperienceCommunity.GenericEcommerce.AcceptJS.Models;

namespace XperienceCommunity.GenericEcommerce.AcceptJS.Components.Accept
{
    [ViewComponent(Name = "Generic.Ecom.AcceptJS")]

    public class AcceptJSViewComponent : ViewComponent
    {
        public AcceptJSViewComponent(IAcceptJSOptions acceptJSOptions)
        {
            AcceptJSOptions = acceptJSOptions;
        }

        public IAcceptJSOptions AcceptJSOptions { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(AcceptJSOptions.PayentGatewayView());
        }
    }
}
