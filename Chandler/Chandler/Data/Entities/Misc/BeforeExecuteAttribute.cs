using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Before Execute Attribute
    /// </summary>
    public class BeforeExecuteAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        /// <summary>
        /// Overridden OnActionExecutionAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await DbActionHelper.WaitLockSemaphoreAsync().ConfigureAwait(false);
            await base.OnActionExecutionAsync(context, next);
        }

        /// <summary>
        /// Overridden OnActionExecuted
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            DbActionHelper.ReleaseSemaphore();
            base.OnActionExecuted(context);
        }
    }
}
