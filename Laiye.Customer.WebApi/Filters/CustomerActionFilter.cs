using Laiye.Customer.WebApi.Model.Dto;
using Laiye.Customer.WebApi.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Laiye.Customer.WebApi.Filters
{
    public class CustomerActionFilter: IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {


           
            //if (!checkresult) throw new NotImplementedException();
            String authTokenHeader = context.HttpContext.Request.Headers["authToken"];
               
            Dictionary<string, string> requestItem = new Dictionary<string, string>();
            //Console.WriteLine(context.HttpContext.Request.HttpContext);
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
            var parameterName = parameter.Name;//获取Action方法中参数的名字
            var parameterType = parameter.ParameterType;//获取Action方法中参数的类型
            Console.WriteLine(parameterName);
            if (parameterType == typeof(RequestInfo))
            {
                // 如果有，那么就获取LoginLogoutRequest类型参数的值
                var RequestZsj = context.ActionArguments[parameterName] as RequestInfo;
                System.Reflection.PropertyInfo[] cfgItemProperties = RequestZsj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (System.Reflection.PropertyInfo item in cfgItemProperties)
                {
                    string name = item.Name;
                    object value = item.GetValue(RequestZsj, null);
                    if (value != null && (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        requestItem.Add(name, RequestZsj.ToString());
                    }
                }
                Console.WriteLine(requestItem);
                   // bool checkresult = SecurityUtils.verificateRequest(requestItem, authTokenHeader);

                }
        }
        //string strData = sr.ReadToEndAsync().Result;

        //throw new NotImplementedException();

    }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }
    }
}
