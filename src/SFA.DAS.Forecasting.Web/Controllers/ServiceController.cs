﻿using System;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Extensions;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [RoutePrefix("Service")]
    public class ServiceController : Controller
    {
	    public readonly IOwinWrapper OwinWrapper;

		public ServiceController(IOwinWrapper owinWrapper)
		{
			OwinWrapper = owinWrapper;
		}

		[Route("signout")]
        public ActionResult SignOut()
        {
			return OwinWrapper.SignOutUser(Url.ExternalUrlAction("service", "signout", true));
		}

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            throw new NotImplementedException();
        }
    }
}