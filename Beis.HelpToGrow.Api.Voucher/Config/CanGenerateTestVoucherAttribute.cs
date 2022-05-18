using Microsoft.AspNetCore.Mvc.Filters;

namespace Beis.HelpToGrow.Api.Voucher.Config
{

    public class CanGenerateTestVoucherAttribute : ActionFilterAttribute
    {
        private bool canShowVouherGenerationEndpoint
        {
            get
            {
                bool value;
                return (bool.TryParse(Environment.GetEnvironmentVariable("SHOW_GENERATE_TEST_VOUCHER_ENDPOINT") ?? "false", out value) && value);         
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!canShowVouherGenerationEndpoint)
            {
                filterContext.Result = new NotFoundResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
