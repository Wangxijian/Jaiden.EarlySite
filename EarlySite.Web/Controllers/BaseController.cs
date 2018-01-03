﻿namespace EarlySite.Web.Controllers
{
    using EarlySite.SModel;
    using System;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        public BaseController()
        {
            ViewBag.Title = "EarlySite";
        }
        
        /// <summary>
        /// 当前登录账户
        /// </summary>
        protected Account CurrentAccount
        {
            get
            {
                return new Account();
            }
        }

        /// <summary>
        /// 上次提交日期
        /// </summary>
        protected DateTime LastSubmitDate
        {
            get;
            set;
        }


        /// <summary>
        /// 重写调用方法前操作
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //验证登录信息或其他  
            base.OnActionExecuting(filterContext);
        }


        protected override void HandleUnknownAction(string actionName)
        {
            //Response.Redirect("/", true);
            this.View("Error").ExecuteResult(this.ControllerContext);
        }
        

        /// <summary>
        /// 重写异常
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            Result<Exception> exceptionmodel = new Result<Exception>()
            {
                Data = exception,
                Message = exception.Message,
                Status = false
            };
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = Json(exceptionmodel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                filterContext.Result = View("Error", exceptionmodel);
            }

            filterContext.ExceptionHandled = true;
            //base.OnException(filterContext);
        }
        
    }
}