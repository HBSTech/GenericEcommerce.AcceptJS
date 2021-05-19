using Generic.AcceptJSPaymentGateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Generic.AcceptJSPaymentGateway.Components.Accept
{
    [ViewComponent(Name = "Generic.Ecom.AcceptJS")]

    public class AcceptJSViewComponent : ViewComponent
    {
        public AcceptJSViewComponent(IHostEnvironment env, IAcceptJSOptions acceptJSOptions)
        {
            Env = env;
            AcceptJSOptions = acceptJSOptions;
        }

        private IHostEnvironment Env { get; }
        public IAcceptJSOptions AcceptJSOptions { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(AcceptJSOptions.PayentGatewayView(), new AcceptJSViewModel(Env.IsDevelopment()));
        }
    }
}
