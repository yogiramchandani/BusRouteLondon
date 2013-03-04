using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BusRouteLondon.Web.Controllers;
using Raven.Client;

namespace BusRouteLondon.Web.Infrastructure.Filter
{
    public class RavenSessionManagementAttribute : ActionFilterAttribute
    {
        private readonly IDocumentStore store;

        public RavenSessionManagementAttribute(IDocumentStore store)
        {
            if (store == null) throw new ArgumentNullException("store");
            this.store = store;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controller = actionContext.ControllerContext.Controller as RavenController;
            if (controller == null)
                return;

            // Can be set explicitly in unit testing
            if (controller.RavenSession != null)
                return;

            controller.RavenSession = store.OpenSession();
            controller.RavenSession.Advanced.UseOptimisticConcurrency = true;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as RavenController;
            if (controller == null)
                return;

            using (var session = controller.RavenSession)
            {
                if (session == null)
                    return;

                if (actionExecutedContext.Exception != null)
                    return;

                session.SaveChanges();
            }
        }
    }
}