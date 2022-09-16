using Microsoft.AspNetCore.Mvc.Filters;

namespace Beis.HelpToGrow.Api.Voucher.Config
{
    public class CanGenerateTestVoucherAttribute : ActionFilterAttribute
    {
        private readonly bool _showGenerateTestVoucherEndPoint;

        public CanGenerateTestVoucherAttribute(bool showGenerateTestVoucherEndPoint)
        {
            _showGenerateTestVoucherEndPoint = showGenerateTestVoucherEndPoint;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_showGenerateTestVoucherEndPoint)
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