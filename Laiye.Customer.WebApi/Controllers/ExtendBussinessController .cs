using DotNetCore.CAP;
using Laiye.Customer.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Laiye.Customer.WebApi.Model.Dto;
using static Laiye.EntUC.Service.Tenant.Tenant;
using static Laiye.EntUC.Service.Configuration.License;
using Laiye.Customer.WebApi.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using static Laiye.EntUC.Service.Configuration.Ticket;
using static Laiye.EntUC.Service.Configuration.SystemConfiguration;
using Laiye.EntUC.Service.Tenant;
using Laiye.EntUC.Service.Configuration;
using Google.Protobuf.WellKnownTypes;
using System.Linq;
using Laiye.Customer.WebApi.Client;
using System.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Laiye.EntUC.Core.Event;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Text.Encodings.Web;
using static Laiye.EntUC.Service.Tenant.Employee;
using static Laiye.EntUC.Service.Base.SubscribleFile;
using Laiye.EntUC.Service.Base;
using System.Security.Cryptography;
using Laiye.EntUC.Core.Common;
using Laiye.EntUC.Service.User;
using static Laiye.EntCmd.Service.Core.Flow;
using Laiye.EntCmd.Service.Core;
using static Laiye.EntCmd.Service.Core.CmdAttachment;
using static Laiye.EntCmd.Service.Core.CreateAttachmentRequest;
using Laiye.EntUC.Core.Event;

namespace Laiye.Customer.WebApi.Controllers
{
    [ApiController]

    [Route("Extend/Bussiness/Subscribe")]

    public class ExtendBussinessController : ControllerBase
    {
        private const string LAST_RECORD_DAY = "2099-12-31";
        private static string ACCESS_KEY = "";
        private ILogger<ExtendBussinessController> Logger { get; }
        private IConfiguration Configuration { get; }

        //private Ticket.TicketClient TicketClient { get; }
        private User.UserClient UserClient { get; }
        //private IServiceProvider ServiceProvider { get; }
        private SystemConfigurationClient SystemConfigurationClient { get; }
        //private HttpClient HttpClient { get; set; }


        public ExtendBussinessController(ILogger<ExtendBussinessController> logger, 
            IConfiguration configuration,
            //Ticket.TicketClient ticketClient,
            User.UserClient userClient,
            SystemConfigurationClient systemConfigurationClient)
        {
            Logger = logger;
            Configuration = configuration;
            //TicketClient = ticketClient;
            UserClient = userClient;
            SystemConfigurationClient = systemConfigurationClient;
            //SubscribleFileClient = subscribleFileClient;
            //HttpClient = new HttpClient();
        }

        private string[] GetProjectName(string projectid) {
            try
            {
                var client = new CommanderClient(new CommanderClientOption
                {
                    
                    HeadersKey = Configuration["ProjectName_HeadersKey"],//读取头文件
                    CommanderUrl = Configuration["getProjectName_Url"]
                    // CommanderUrl = "https://openapi.cmft.com/gateway/paas_console/1.0/open-api"
                }, 1);
                Logger.LogInformation($"GetProjectName <20.1>:GetProjectNameAsync");
                
                var res = client.getProjectNameClient.GetProjectNameAsync(projectid);
                Logger.LogInformation($"GetProjectName <20.2>:res {res}");
                return res;
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetProjectName <20.3>:error:"+ ex);
                return new string[2] { "error", "error" }; 
            }
        }

        [HttpGet("test")]
        public BaseResponse<string> Test([FromQuery] SubscribeArgs request)
        {
            Console.WriteLine("companyid:"+request.companyid);
            return new BaseResponse<string>("aaaa");
        }



        private bool DoCheckHeadsinfo1(string accesskey,string headerscode, RequestCommand request)
        {
            Dictionary<string, string> requestItem = new Dictionary<string, string>();
            Logger.LogInformation($"doCheckHeadsinfo <<30.1>>");
            {
                // 如果有，那么就获取LoginLogoutRequest类型参数的值
                var RequestZsj = request;
                System.Reflection.PropertyInfo[] cfgItemProperties = request.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (System.Reflection.PropertyInfo item in cfgItemProperties)
                {
                    string name = item.Name;
                    object value = item.GetValue(request, null)??string.Empty;
                   // if (!string.IsNullOrWhiteSpace(value.ToString())) { Console.WriteLine(value.ToString()); }
                    
                    if (value != null && (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                       // if (name == "licenseCount" && value.ToString().Equals("0")) { continue; }
                        requestItem.Add(name, value.ToString());
                        //Console.WriteLine(value.ToString());
                    } 
                    else
                    {
                        requestItem.Add(name, "");
                    }
                }
                //
                 requestItem.OrderBy(item => item.Key).ToArray();

                string str = SecurityUtils.getRequestParamsForSign(requestItem);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> getRequestParamsForSignstr {str} ");
                string signature= SecurityUtils.getSignature(accesskey.ToUpper(), str);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> getSignature {signature}" );
                bool ret= SecurityUtils.verifySignatrue(signature, headerscode);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> verifySignatrue headerscode:{headerscode}, result:{ret}" );
                //Logger.LogInformation($"doCheckHeadsinfo <<30.2>> verifySignatrue result {ret}" );
                return ret;
            }
        }

        private bool DoCheckHeadsinfo2(string accesskey, string headerscode, RequestInfo request)
        {
            Dictionary<string, string> requestItem = new Dictionary<string, string>();
            Logger.LogInformation($"doCheckHeadsinfo <<30.1>>");
            {
                // 如果有，那么就获取LoginLogoutRequest类型参数的值
                var RequestZsj = request;
                System.Reflection.PropertyInfo[] cfgItemProperties = request.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (System.Reflection.PropertyInfo item in cfgItemProperties)
                {
                    
                    string name = item.Name;
                    object value = item.GetValue(request, null)??string.Empty;
                    // if (!string.IsNullOrWhiteSpace(value.ToString())) { Console.WriteLine(value.ToString()); }
                    
                    if (value != null && (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        //string s1 =HttpUtility.UrlEncode(value.ToString(), Encoding.UTF8);
                        requestItem.Add(name, value.ToString());
                    }
                    else
                    {
                        requestItem.Add(name, "");
                    }
                }
                //
                requestItem.OrderBy(item => item.Key).ToArray();

               
                string str = SecurityUtils.getRequestParamsForSign(requestItem);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> getRequestParamsForSignstrt {str} ");
                string signature = SecurityUtils.getSignature(accesskey.ToUpper(), str);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> getSignature {signature}");
                bool ret = SecurityUtils.verifySignatrue(signature, headerscode);
                Logger.LogInformation($"doCheckHeadsinfo <<30.2>> verifySignatrue headerscode:{headerscode}, result:{ret}");
                //Logger.LogInformation($"doCheckHeadsinfo <<30.2>> verifySignatrue result {ret}");
                return ret;
            }
        }


        private BaseResponse<ResponeSuccess2> GetReturnSignature(string accesskey, BaseResponse<ResponeSuccess2> data,string retstr, string loginurl)
        {
            //Logger.LogInformation($"getReturnSignature <<100>> begin");
            data.data= new ResponeSuccess2 { loginUrl = loginurl,saasUrl = retstr };
            var res=JsonConvert.SerializeObject(data);
            
            


            //res = "{\"code\":\"200\",\"data\":{\"LoginUrl\":\"http://192.168.12.171\",\"saasUrl\":\"http://192.168.12.171/api/identity/invite/commanderAdministrator?ticket=986653ed0e2646b982fe9eb3fb12d05b\"},\"message\":\"SUCCESS\"}";
            //string signinfo = SecurityUtils.getSignature("CMHK-RPA-FTUIBOT-HK-DI", res);

            Logger.LogInformation($"$getReturnSignature <<100>>  {res}");
            var dic = JsonConvert.DeserializeObject<SortedDictionary<string, object>>(res);
            SortedDictionary<string, object> keyValues = new SortedDictionary<string, object>(dic);
             keyValues.OrderBy(m => m.Key);
            
            //keyValues.OrderBy(data=> data.Key);

           //升序
           //keyValues.OrderByDescending(m => m.Key);//降序
           var res1 = JsonConvert.SerializeObject(keyValues);
            Logger.LogInformation($"$getReturnSignature <<100>> sort:{res1}, ak:{accesskey.ToUpper()}");
            //Logger.LogInformation($"$getReturnSignature <<100>> ak:{accesskey.ToUpper()}");
            string signatureinfo = SecurityUtils.getSignature(accesskey.ToUpper(), res1);
            Logger.LogInformation($"getReturnSignature <<100>> signinfo {signatureinfo}" );
            this.HttpContext.Response.Headers["signature"] = signatureinfo;
            //Logger.LogInformation($"getReturnSignature <<100>> Signature {signatureinfo}");
            return data;
      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="conn"></param>
        /// <param name="tenantClient"></param>
        /// <param name="ticketClient"></param>
        /// <param name="systemConfigurationClient"></param>
        /// <returns></returns>
        [HttpGet("GetCommander")]
        //public async Task<string> GetCommander(
        public async Task<BaseResponse<ResponeSuccess2>> GetCommander(
        
           [FromQuery] RequestCommand requestCommand,
          // [FromHeader] ZsyHeaders zsyheaders,
           [FromServices] IFreeSql conn,
           [FromServices] TenantClient tenantClient,
           [FromServices] TicketClient ticketClient,
           [FromServices] SystemConfigurationClient systemConfigurationClient)

        {
            RequestInfo request = new RequestInfo{
            //spec = requestCommand.spec,
            specNames = requestCommand.specNames,
            specValues = requestCommand.specValues,
            chargeType = requestCommand.chargeType,
            price = requestCommand.price,
            orderNo= requestCommand.orderNo,
            productNo = requestCommand.productNo,
            versionNo = requestCommand.versionNo,
            channel = requestCommand.channel,
            tenant = requestCommand.tenant,
            projectId = requestCommand.projectId,
            projectType = requestCommand.projectType,
            userName = requestCommand.userName,
            userType = requestCommand.userType,
            requestTime = requestCommand.requestTime,
            dueDate = requestCommand.dueDate,//string.IsNullOrEmpty(requestCommand.dueDate) ? Timestamp.FromDateTime(DateTime.MinValue).ToString() : requestCommand.requestTime;
            };
            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            Logger.LogInformation($"GetCommander <<1.16>>:GetWebConfigurationInfoAsync:webConfigurationResponse {webConfigurationResponse}");
            string loginurl = EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl);

            if (!Request.Headers.TryGetValue("authToken", out var headerValue))
            {
                // error
                Logger.LogError($"GetCommander <<1.1>>:The authToken field is required");
                return new BaseResponse<ResponeSuccess2>(code: "400", message: "参数authToken是必须的");//The authToken field is required


            }
            string headerscode = headerValue;

            // string headerscode = zsyheaders.authToken;
            //Logger.LogInformation($"GetCommander <<1.1>>:get headerscode {headerscode}");
            string[] projectInfo = GetProjectName(request.projectId);
            //Logger.LogInformation($"GetCommander <<1.2>>:get projectInfo {projectInfo}" );

            if (projectInfo[1] == "error")
            {
              return  new BaseResponse<ResponeSuccess2>(code: "000501", message: "未找到项目信息"); }//BAD_SQL_ERROR
            //string accesskey = request.tenant + '-' + projectInfo[0];
            ACCESS_KEY = request.tenant + '-' + projectInfo[0];
            Logger.LogInformation($"GetCommander <<1.3>>:get ACCESS_KEY:{ACCESS_KEY}");
             //Logger.LogInformation($"GetCommander <<1.4>>:doCheckHeadsinfo {ACCESS_KEY} ");
              bool checkvalue = DoCheckHeadsinfo1(ACCESS_KEY, headerscode, requestCommand);
             if (checkvalue == false)
             {

                return  GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "验签错误"),"", "");//VALIDATE_ERROR
                  //return new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR");
              }

            //判断租户是否存在
            string tenantid = projectInfo[2];//    request.tenant;
            //Logger.LogInformation($"GetCommander <<1.5>> get tenant,{tenantid} ");
            string projectid = request.projectId;
            Logger.LogInformation($"GetCommander <<1.6>> get projectId,{projectid}");

            //string projectname = projectcode[1];
            //租户名称=租户名称+'/'项目名称
            string projectname = projectInfo[3] + '/' + projectInfo[1];
            //Logger.LogInformation($"GetCommander <<1.7>> get projectName,{projectname} ");
            string saasurl = "";
            //Logger.LogInformation("GetCommander <<1.8>>:QueryTenant begin");

            long ret = await QueryTenant(conn, tenantid, projectid, tenantClient);//查询租户

            if (ret == 0)//如果不存在 则创建租户
            {
                //Logger.LogInformation("GetCommander <<1.9>>:doCreateTenantInfo begin");
                request.tenant = tenantid;
                var res = DoCreateTenantInfo(request, projectname, conn, tenantClient, ticketClient, systemConfigurationClient);
                if (res.Result.code == "200")
                {
                    saasurl = res.Result.data.saasUrl??String.Empty;
                   return  GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl = loginurl }),  saasurl, loginurl);
                   // return new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl });
                }
                else
                {
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"),"","");
                    //return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR");

                }

            }
            else if (ret == -1)
            {
                Logger.LogInformation($"GetCommander <<1.15>>:CreateTenantInfo tenant is disabled:{ret}");
                //throw new LaiyeException(400, "租户已被禁用");
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000403", message: "NOT_RIGHT"), "", "");
            }
            else
            {
                //如果租户存在,则判断之前是否已经订阅Commander，如果已经订阅，则返回Commander界面，如果没有则生成邀请码
                Logger.LogInformation("GetCommander <<1.10>>:CreateTenantInfo is Exist");
                //bool isExisted = await QuerySubscriptionIsExisted(conn, ret, request, tenantClient);
                //Logger.LogInformation("GetCommander <<1.15>>:QuerySubscriptionIsExisted:租户之前是否已经订阅:", isExisted);

                //if (isExisted == true)
                //{
                //    return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl = loginurl }), loginurl, loginurl); ;
                //}

                //Logger.LogInformation("GetCommander <<1.11>>:GetAdministratorInvitationUrl2");
                string returl = await GetAdministratorInvitationUrl2(ret, request, conn, tenantClient, ticketClient, systemConfigurationClient);
                //Logger.LogInformation($"GetCommander <<1.12>>:GetAdministratorInvitationUrl2,{ returl}");
                
                int status = 0;
                status = returl == "error" ? -1 : 1;
                if (status == 1)
                {
                    Logger.LogInformation($"GetCommander <<1.13>>:return returl{ returl}");

                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl = loginurl }), returl, loginurl);

                    //return new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = returl });
                }
                else
                {
                    Logger.LogInformation($"GetCommander <<1.14>>:return status {status}");
                    return  GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"),"","");
                    //return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR");
                }

            }


        }



        [HttpGet("GetLicense")]//
        public async Task<BaseResponse<ResponeSuccess2>> GetLicense(
           [FromQuery] RequestInfo request,
           //[FromHeader] ZsyHeaders zsyheaders,
           [FromServices] IFreeSql conn,
           [FromServices] TenantClient tenantClient,
           [FromServices] TicketClient ticketClient,
           [FromServices] SystemConfigurationClient systemConfigurationClient,
           [FromServices] LicenseClient licenseclient)
        {
            if (!Request.Headers.TryGetValue("authToken", out var headerValue))
            {
                // error
                Logger.LogError($"Getlicense <<2.0>>:The authToken field is required");
                return new BaseResponse<ResponeSuccess2>(code: "400", message: "The authToken field is required");
            }


            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            Logger.LogInformation($"GetLicense <<2.27>>:GetWebConfigurationInfoAsync:webConfigurationResponse {webConfigurationResponse}");
            string loginurl = EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl);

            string headerscode = headerValue;
            //Logger.LogInformation($"Getlicense <<2.1>>:getheaderscode {headerscode}");
            string[] projectcode = GetProjectName(request.projectId);
            //Logger.LogInformation($"Getlicense <<2.2>>:projectcode {projectcode}");
            if (projectcode[0] == "error") { return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"); }
            //string accesskey = request.tenant + '-' + projectcode[0];
            ACCESS_KEY = request.tenant + '-' + projectcode[0];
            Logger.LogInformation($"Getlicense <<2.3>>:accesskey,{ACCESS_KEY}");

            bool checkvalue = DoCheckHeadsinfo2(ACCESS_KEY, headerscode, request);
            checkvalue = true;
            Logger.LogInformation($"Getlicense <<2.4>>:CheckHeadsinfo {checkvalue}");
            if (checkvalue == false)
            {
                GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR"), "", "");
                return new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR");

            }


            //string tenantid = request.tenant;

            string tenantid = projectcode[2];
            //Logger.LogInformation($"Getlicense <<2.5>>:tenant {tenantid}");
            string projectid = request.projectId;
            Logger.LogInformation($"Getlicense <<2.6>>:projectId {projectid}");
            //licenseType和count又要从specValue值取 specValue  类型|数量
            string[] array = request.specValues.ToString().Split("-");
            string licenseType_s = array[0].Substring(1);
            string licenseCount_s = array[1];


            //licenseType 1 - 试用 2 - 年度订阅 3 - 终身买断
            int licenseType = int.Parse(licenseType_s);

            int licenseCount = int.Parse(licenseCount_s);

            //orderType的值得 从productNo取
            string orderType = request.productNo;//.orderType;
            Logger.LogInformation($"Getlicense <<2.9>>:licenseType:{licenseType}, licenseCount:{licenseCount}, orderType:{orderType}");

            string saasurl = "";
            long companyId = 0;//系统生成的租户ID

            int status = 0;//分配Licence,1：订阅成功 -1：失败

            //租户名称=租户名称+'/'项目名称
            string projectname = projectcode[3] + '/' + projectcode[1];
            //Logger.LogInformation($"GetLicense <<2.10>>:QueryTenant begin");
            companyId = await QueryTenant(conn, tenantid, projectid, tenantClient);
            

            //Logger.LogInformation($"GetLicense <<2.17>>:QueryLicenceToItemId begin:");
            var itemId = await QueryLicenceToItemId(licenseclient, orderType, licenseType, licenseCount);
            Logger.LogInformation($"GetLicense <<2.18>>:QueryLicenceToItemId {itemId}");
            DateTime ls_endTime = DateTime.UtcNow;
            //Logger.LogInformation($"GetLicense <<2.19>>:ls_endTime {ls_endTime}");
            if (!itemId.Equals("0"))
            {
                if (companyId == 0)
                {
                    Logger.LogInformation($"GetLicense <<2.11>>:Tenant is not exist");
                    request.tenant = tenantid;
                    var res = DoCreateTenantInfo(request, projectname, conn, tenantClient, ticketClient, systemConfigurationClient);
                    //判断返回消息码，如果创建失败直接返回，没失败直接走下一步
                    if (res.Result.code == "200")
                    {
                        saasurl = res.Result.data.saasUrl;
                        companyId = res.Result.data.companyId;
                        Logger.LogInformation($"GetLicense <<2.15>>:doCreateTenantInfo:companyId {companyId},saasurl:{saasurl}");
                    }
                    else
                    {
                        Logger.LogInformation($"GetLicense <<2.16>>: doCreateTenantInfo Error ");
                        return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"), "", "");
                        
                    }
                }
                if (companyId == -1)
                {
                    Logger.LogInformation($"GetLicense <<1.15>>:QueryTenant tenant is disabled:{companyId}");
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000403", message: "NOT_RIGHT"), "", "");
                }

                Logger.LogInformation($"GetLicense <<2.21>>:AssignLisence:{itemId},companyId{companyId},licenseCount:{licenseCount}");
                var ret2 = await AssignLisence(licenseclient, companyId, itemId, licenseCount);
                Logger.LogInformation($"GetLicense <<2.22>>:AssignLisence:{ret2}");

                switch (licenseType)
                {
                    case 1:
                        {
                            //ls_endTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(2).AddDays(-1);
                            ls_endTime = DateTime.UtcNow.Date.AddMonths(2).AddDays(-1);
                            //Logger.LogInformation($"GetLicense <<2.23>>:ls_endTime:{ls_endTime}");
                            break;
                        }
                    case 2:
                        {
                            //ls_endTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).AddYears(1);
                            ls_endTime = DateTime.UtcNow.Date.AddYears(1).AddMonths(1).AddDays(-1);
                            //Logger.LogInformation($"GetLicense <<2.24>>:ls_endTime:{ls_endTime}");
                            break;
                        }
                    case 3:
                        {
                            ls_endTime = DateTime.Parse(LAST_RECORD_DAY);
                            //Logger.LogInformation($"GetLicense <<2.25>>:ls_endTime:{ls_endTime}");
                            break;
                        }
                }

                if (ret2 == true)
                {
                    status = 1;
                    //Logger.LogInformation($"GetLicense <<2.26>>:ret:{ret2}");
                    //requset里参数，写订阅表
                    //Logger.LogInformation("GetLicense <<2.27>>:doWriteSubscription begin");
                    DoWriteSubscription(status, ls_endTime, request, conn, new SubscribeArgs { companyid = companyId }, 0);
                    //Logger.LogInformation("GetLicense <<2.23>>:doWriteSubscription end");

                }

            }
            else
            {
                //分配licesen
                //Logger.LogInformation("GetLicense <<2.24>>:QueryLicenceToItemId  not use");
                status = -1;
                //ls_endTime = DateTime.UtcNow;
                //写订阅表失败--lisence数量不够
                //Logger.LogInformation("GetLicense <<2.25>>:doWriteSubscription begin");
                DoWriteSubscription(status, ls_endTime, request, conn, new SubscribeArgs { companyid = companyId }, 0);
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000404", message: "NOT_FOUND"), "", "");
            };//返回liscens不够信息


            // return new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl });
            if (string.IsNullOrEmpty(saasurl))
            {
                string returl = await GetAdministratorInvitationUrl2(companyId, request, conn, tenantClient, ticketClient, systemConfigurationClient);
                
                saasurl = returl;
            }
            return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl = loginurl }), saasurl, loginurl);
        }

        //public async Task<BaseResponse<ResponeSuccess2>> GetLicense(
        //   [FromQuery] RequestInfo request,
        //   //[FromHeader] ZsyHeaders zsyheaders,
        //   [FromServices] IFreeSql conn,
        //   [FromServices] TenantClient tenantClient,
        //   [FromServices] TicketClient ticketClient,
        //   [FromServices] SystemConfigurationClient systemConfigurationClient,
        //   [FromServices] LicenseClient licenseclient)
        //{
        //    if (!Request.Headers.TryGetValue("authToken", out var headerValue))
        //    {
        //        // error
        //        Logger.LogInformation($"Getlicense <<2.0>>:The authToken field is required");
        //        return new BaseResponse<ResponeSuccess2>(code: "400", message: "The authToken field is required");
        //    }


        //    var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
        //    Logger.LogInformation($"GetLicense <<2.27>>:GetWebConfigurationInfoAsync:webConfigurationResponse {webConfigurationResponse}");
        //    string loginurl = EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl);

        //    string headerscode = headerValue;
        //    Logger.LogInformation($"Getlicense <<2.1>>:getheaderscode {headerscode}");
        //    string[] projectcode = await GetProjectName(request.projectId);
        //    Logger.LogInformation($"Getlicense <<2.2>>:projectcode {projectcode}");
        //    if (projectcode[0] == "error") { return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"); }
        //    //string accesskey = request.tenant + '-' + projectcode[0];
        //    ACCESS_KEY = request.tenant + '-' + projectcode[0];
        //    Logger.LogInformation($"Getlicense <<2.3>>:accesskey,{ACCESS_KEY}");

        //    bool checkvalue = await doCheckHeadsinfo2(ACCESS_KEY, headerscode, request);
        //    checkvalue = true;
        //    Logger.LogInformation($"Getlicense <<2.4>>:CheckHeadsinfo {checkvalue}");
        //    if (checkvalue == false)
        //    {
        //        getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR"), "", "");
        //        return new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR");

        //    }


        //    //string tenantid = request.tenant;

        //    string tenantid = projectcode[2];
        //    Logger.LogInformation($"Getlicense <<2.5>>:tenant {tenantid}");
        //    string projectid = request.projectId;
        //    Logger.LogInformation($"Getlicense <<2.6>>:projectId {projectid}");
        //    //licenseType和count又要从specValue值取 specValue  类型|数量
        //    string[] array = request.specValues.ToString().Split("-");
        //    string   licenseType_s = array[0].SubString(1);
        //    string  licenseCount_s = array[1];


        //    //转换赋值给之前变量 licenseType
        //    int licenseType = int.Parse(licenseType_s);

        //      //1 - 试用 2 - 年度订阅 3 - 终身买断

        //    Logger.LogInformation($"Getlicense <<2.7>>:licenseType {licenseType}");
        //    //转换赋值给之前变量 licenseCount
        //    int licenseCount = int.Parse(licenseCount_s);

        //    Logger.LogInformation($"Getlicense <<2.8>>:licenseCount {licenseCount}");
        //    //orderType的值得 从productNo取
        //    string orderType = request.productNo;//.orderType;
        //    Logger.LogInformation($"Getlicense <<2.9>>:orderType {orderType}");

        //    string saasurl = "";
        //    long companyId = 0;//系统生成的租户ID

        //    int status = 0;//分配Licence,1：订阅成功 -1：失败

        //    //租户名称=租户名称+'/'项目名称
        //    string projectname = projectcode[3]+'/'+ projectcode[1];
        //    Logger.LogInformation($"GetLicense <<2.10>>:QueryTenant begin");
        //    companyId = await QueryTenant(conn, tenantid, projectid, tenantClient);
        //    if (companyId == 0)
        //    {
        //        Logger.LogInformation($"GetLicense <<2.11>>:Tenant is not exist");
        //        //Logger.LogInformation("GetLicense <<8.3>>:doCreateTenantInfo begin");
        //        Logger.LogInformation($"GetLicense <<2.12>>:doCreateTenantInfo");
        //        request.tenant = tenantid;
        //        var res = doCreateTenantInfo(request, projectname, conn, tenantClient, ticketClient, systemConfigurationClient);
        //        //判断返回消息码，如果创建失败直接返回，没失败直接走下一步
        //        if (res.Result.code == "200")
        //        {
        //            Logger.LogInformation($"GetLicense <<2.13>>:doCreateTenantInfo:{res}");
        //            saasurl = res.Result.data.saasUrl;
        //            Logger.LogInformation($"GetLicense <<2.14>>:doCreateTenantInfo:saasurl{saasurl}");
        //            companyId = res.Result.data.companyId;
        //            Logger.LogInformation($"GetLicense <<2.15>>:doCreateTenantInfo:companyId {companyId}");
        //        }
        //        else
        //        {
        //            Logger.LogInformation($"GetLicense <<2.16>>: doCreateTenantInfo Error ");
        //            return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"), "", "");
        //            //return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR"); }

        //        }
        //    }
        //    if (companyId == -1)
        //    {
        //        Logger.LogInformation($"GetLicense <<1.15>>:QueryTenant tenant is disabled:{companyId}");
        //        //throw new LaiyeException(400, "租户已被禁用");
        //        return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000403", message: "NOT_RIGHT"), "", "");
        //    }
        //    Logger.LogInformation($"GetLicense <<2.17>>:QueryLicenceToItemId begin:");
        //        var itemId = await QueryLicenceToItemId(licenseclient, orderType, licenseType, licenseCount);
        //        Logger.LogInformation($"GetLicense <<2.18>>:QueryLicenceToItemId {itemId}");
        //        DateTime ls_endTime = DateTime.UtcNow;
        //        Logger.LogInformation($"GetLicense <<2.19>>:ls_endTime { ls_endTime}");
        //        if (!itemId.Equals("0"))
        //        {

        //            Logger.LogInformation($"GetLicense <<2.20>>:ItemId :{itemId}");
        //            Logger.LogInformation($"GetLicense <<2.21>>:AssignLisence:{itemId}");
        //            var ret2 = await AssignLisence(licenseclient, companyId, itemId, licenseCount);
        //            Logger.LogInformation($"GetLicense <<2.22>>:AssignLisence:{ret2}");

        //            switch (licenseType)
        //            {
        //                case 1:
        //                    {
        //                        //ls_endTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(2).AddDays(-1);
        //                        ls_endTime = DateTime.UtcNow.Date.AddMonths(2).AddDays(-1);
        //                        Logger.LogInformation($"GetLicense <<2.23>>:ls_endTime:{ls_endTime}");
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        //ls_endTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).AddYears(1);
        //                        ls_endTime = DateTime.UtcNow.Date.AddYears(1).AddMonths(1).AddDays(-1);
        //                        Logger.LogInformation($"GetLicense <<2.24>>:ls_endTime:{ls_endTime}");
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        ls_endTime = DateTime.Parse(LAST_RECORD_DAY);
        //                        Logger.LogInformation($"GetLicense <<2.25>>:ls_endTime:{ls_endTime}");
        //                        break;
        //                    }
        //            }

        //            if (ret2 == true)
        //            {
        //                status = 1;
        //                //Logger.LogInformation("GetLicense <<8.6>>:AssignLisence retvalue",ret2);
        //                Logger.LogInformation($"GetLicense <<2.26>>:ret:{ret2}");
        //                //requset里参数，写订阅表
        //                Logger.LogInformation("GetLicense <<2.27>>:doWriteSubscription begin");
        //                //doWriteSubscription(status, ls_endTime, request, conn);
        //                doWriteSubscription(status, ls_endTime, request, conn, new SubscribeArgs { companyid = companyId }, 0);
        //                Logger.LogInformation("GetLicense <<2.23>>:doWriteSubscription end");

        //            }

        //        }
        //        else
        //        {
        //            //分配licesen
        //            Logger.LogInformation("GetLicense <<2.24>>:QueryLicenceToItemId  not use");
        //            status = -1;
        //            //ls_endTime = DateTime.UtcNow;
        //            //写订阅表失败--lisence数量不够
        //            Logger.LogInformation("GetLicense <<2.25>>:doWriteSubscription begin");
        //            doWriteSubscription(status, ls_endTime, request, conn, new SubscribeArgs { companyid = companyId }, 0);
        //            Logger.LogInformation("GetLicense <<2.26>>:doWriteSubscription begin");
        //            return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000404", message: "NOT_FOUND"), "", "");
        //            //return new BaseResponse<ResponeSuccess2>(code: "000404", message: "NOT_FOUND");
        //        };//返回liscens不够信息


        //    // return new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl });
        //    if (string.IsNullOrEmpty(saasurl))
        //    {
        //        saasurl = loginurl;
        //    }
        //    return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl=loginurl }), saasurl,loginurl);
        //}


        [HttpGet("GetFlowSubscription")]
        public async Task<BaseResponse<ResponeSuccess2>> GetFlowSubscription(

            [FromQuery] RequestCommand requestCommand,
            [FromServices] SubscribleFileClient subscribleFileClient,
            [FromServices] SystemConfigurationClient systemConfigurationClient)

        {
            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            //Logger.LogInformation($"GetFlowSubscription <<1.16>>:GetWebConfigurationInfoAsync:webConfigurationResponse {webConfigurationResponse}");
            string loginurl = EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl);
            
            if (!Request.Headers.TryGetValue("authToken", out var headerValue))
            {
                // error
                Logger.LogError($"GetFlowSubscription <<1.1>>:The authToken field is required");
                return new BaseResponse<ResponeSuccess2>(code: "400", message: "The authToken field is required");
            }
            string headerscode = headerValue;

            string[] projectInfo = GetProjectName(requestCommand.projectId??String.Empty);
            //Logger.LogInformation($"GetFlowSubscription <<1.2>>:get projectInfo {projectInfo}");

            if (projectInfo[1] == "error")
            {
                return new BaseResponse<ResponeSuccess2>(code: "000501", message: "BAD_SQL_ERROR");
            }
            //string accesskey = requestCommand.tenant + '-' + projectInfo[0];
            ACCESS_KEY = requestCommand.tenant + '-' + projectInfo[0];
            Logger.LogInformation($"GetFlowSubscription <<1.3>>:get accesskey {ACCESS_KEY}");
            bool checkvalue = DoCheckHeadsinfo1(ACCESS_KEY, headerscode, requestCommand);
            checkvalue = true;
            if (checkvalue == false)
            {
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR"), "", "");
            }
 
            Logger.LogInformation($"GetFlowSubscription <<1.10>>:requestCommand.productNo:{requestCommand.productNo}");
            //string ret_httpUrl = await QueryFlowSubscription(conn, requestCommand);
            if (string.IsNullOrEmpty(requestCommand.productNo))
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "PARAM_ERROR"), "", "");
            
            var result = await subscribleFileClient.GetFileByProductNoAsync(new GetFileByProductNoRequest
            {
                ProductNo = requestCommand.productNo,
            });

            if (result.File != null) //if (!string.IsNullOrEmpty(ret_httpUrl))
            {
                result.File.HttpUrl = BuildTempUrl(result.File.HttpUrl, result.File.FileName, "multipart/form-data");
                result.File.HttpUrl = loginurl + result.File.HttpUrl;
                Logger.LogInformation($"GetFlowSubscription <<1.11>>:result.File.HttpUrl:{result.File.HttpUrl}");
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = result.File.HttpUrl, loginUrl = loginurl }), result.File.HttpUrl, loginurl); ;
            }
            else
            {
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000404", message: "NOT_FOUND"), "", "");
            }

        }

        private string BuildTempUrl(string key, string fileName, string responseType)
        {
            var nonce = Guid.NewGuid().ToString("N");
            var timeStamp = (long)DateTimeOffset.Now.ToUnixTimeSeconds();
            var sign = ComputeSign(key, responseType, nonce, timeStamp.ToString());
            //var url = $"/api/tenant/subscriblefile/content?key={HttpUtility.UrlEncode(key)}&responseType={HttpUtility.UrlEncode(responseType)}&nonce={HttpUtility.UrlEncode(nonce)}&timestamp={timeStamp}&signature={sign}";
            var url = $"/api/tenant/subscriblefile/content?key={key}&filename={fileName}&responseType={responseType}&nonce={nonce}&timestamp={timeStamp}&signature={sign}";//key={key}
            return url;
        }

        private string ComputeSign(params string[] args)
        {
            var Key = Encoding.UTF8.GetBytes("KvssrH7FZZaEGGBYwJlmIt3Cg3ugrWmMBoHV");//"KvssrH7FZZaEGGBYwJlmIt3Cg3ugrWmMBoHV"
            var strToSign = string.Join(',', args);
            Logger.LogInformation($"ComputeSign strToSign:{strToSign}");
            using (var hmac = new HMACSHA256(Key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(strToSign));
                Logger.LogInformation($"ComputeSign strToSign:{hash}");
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        

        //createType: 对应OutCapacityType中Type 1-worker 500 2-creator 600 
        //LicenceType:  1-试用 9-正式，要判断过期时间expireDate
        //返回itemId，itemId为0表示没有可以分配的license
        //分配时把ItemId和数量传入参数
        private async Task<bool> QueryLisence(LicenseClient licenseclient, int createType, int LisenceType,int LisenceCount){
            //调用 查询lisence的方法
            var resp = await licenseclient.GetAllLicensesAsync(new EntUC.Service.Configuration.GetAllLicensesRequest { });
            
            var list = resp.Capacities;
            
            foreach (var item in list)
            {
                var _LisenceType = item.LicenseType; 
                var _createType = item.Type;
                var _LisenceCount = item.Quantity;
                //item.StartDate.ToDateTime().Month;//处理时区
                //DateTime.Now.Date.Year
                if((int)_createType== createType && (int)_LisenceType == LisenceType && (int)_LisenceCount >= LisenceCount)
                {
                    return true;
                }

            }
            //判断gr返回的值，根据worker或creator和command
            return false;
        }
        //license类型 1-试用 2-1年 3-买断
        //前端传来的orderType:购买license类型1:command 2:worker 3:creator
        //createType: 对应OutCapacityType中Type 1-worker 500 2-creator 600 
        //LicenceType:  1-试用 9-正式，要判断过期时间expireDate
        // 授权类型
        //enum OutLicenseType
        //{
        //    // 未知
        //    Unkown = 0;
        //    // 试用
        //    Trial = 1;
        //    // 正式已签约
        //    Signed = 9;
        //}
        //返回itemId，itemId为0表示没有可以分配的license
        //分配时把ItemId和数量传入参数
        private async Task<string> QueryLicenceToItemId(LicenseClient licenseclient, string orderType, int LicenseType, int LicenseCount)
        {
            //调用 查询lisence的方法
            // Logger.LogInformation(" QueryLicenceToItemId<<70.1>>:GetAllLicensesAsync");
            //Logger.LogInformation($" QueryLicenceToItemId<<70.1>>:LicenseType:{LicenseType}");
            var resp = await licenseclient.GetAllLicensesAsync(new EntUC.Service.Configuration.GetAllLicensesRequest { });
            //Logger.LogInformation($" QueryLicenceToItemId<<70.1>>:GetAllLicensesAsync res:{resp}");
            var list = resp.Capacities;
            Logger.LogInformation($" QueryLicenceToItemId<<70.2>>:GetAllLicensesAsync Capacities:{ resp.Capacities}");

            string str_createType = "";
            int _LicenseCount = 0;
            int QueryOrderType = 1;

            DateTime startDate;
            DateTime expireDate;
            DateTime now = DateTime.UtcNow;
            if (orderType.Equals("FtuibotWorkerFormal")|| orderType.Equals("FtuibotWorkerTrial")) { QueryOrderType = 2; }
            if (orderType.Equals("FtuibotCreator")|| orderType.Equals("FtuibotCreatorTest")) { QueryOrderType = 3; }
            //added by wenz 1010 人机交互浮动授权
            if (orderType.Equals("FtuibotWorkerAttendedFormal") || orderType.Equals("FtuibotWorkerAttendedTrial")) { QueryOrderType = 4; }

            Logger.LogInformation($" QueryLicenceToItemId<<70.3>>:orderType:{ orderType}");

            foreach (var item in list)
            {
                int _createType = 0;
                str_createType = item.Type.ToString();
                if (str_createType.Trim().Equals("WorkerUnattendRuntime"))

                {
                    _createType = 2;
                }
                if (str_createType.Trim().Equals("CreatorFloating"))
                {
                    _createType = 3;
                }
                //added by wenz 1010 人机交互浮动授权
                if (str_createType.Trim().Equals("WorkerAttendedFloating"))

                {
                    _createType = 4;
                }

                
                _LicenseCount = item.Available;//.Quantity;
                Logger.LogInformation($"QueryLicenceToItemId<<70.6>>:str_createType:{str_createType}, _createType:{_createType}, Available:{_LicenseCount}");
                

                startDate = item.StartDate.ToDateTime().Date;
                //Logger.LogInformation($" QueryLicenceToItemId<<70.7>>:startData:{startDate}" );
                expireDate = item.ExpireDate.ToDateTime().Date;
                Logger.LogInformation($"QueryLicenceToItemId<<70.8>>:startData:{startDate}, expireDate:{ expireDate}");

                var _today = now.Date;
                //Logger.LogInformation($"QueryLicenceToItemId<<70.9>>:today :{_today}");
                var _nextToday = _today.AddMonths(1);             //下月本日
                //Logger.LogInformation($" QueryLicenceToItemId<<70.10>>:_nextToday :{ _nextToday}");
                var _lastday = _nextToday.AddMonths(1);      //下下个月本日
                //Logger.LogInformation($"QueryLicenceToItemId<<70.11>>:_lastday :{ _lastday}");
                var _nextTodayOfNextYear = _today.AddYears(1);    //明年今日
                //Logger.LogInformation($"QueryLicenceToItemId<<70.12>>:_nextTodayOfNextYear :{_nextTodayOfNextYear}");
                var _nextLastdayOfNextYear = _nextTodayOfNextYear.AddMonths(1);  //明年下月今日
                //Logger.LogInformation($"QueryLicenceToItemId<<70.13>>:_nextLastdayOfNextYear:{_nextLastdayOfNextYear}");

                if (expireDate > startDate)
                {   //生效时间早于等于今天      失效时间 大于等于下月今天，小于下下月今天；
                    if (_createType == QueryOrderType && _LicenseCount >= LicenseCount)
                    {
                        //Logger.LogInformation(" QueryLicenceToItemId<<70.14>>:createType == QueryOrderType && _LicenseCount >= LicenseCount):");
                        Logger.LogInformation($" QueryLicenceToItemId<<70.15>>:LicenseType:{LicenseType}");
                        if (LicenseType == 1)  //试用
                        {
                            //Logger.LogInformation($" QueryLicenceToItemId<<70.15>>:LicenseType:{ LicenseType }");
                            if (startDate <= _today && expireDate >= _nextToday && expireDate < _lastday)

                            {
                                //Logger.LogInformation(" QueryLicenceToItemId<<70.15.1>>:getItemId:", item.ItemId);
                                return item.ItemId;
                            }
                        }
                        if (LicenseType == 2)  //年度订阅
                        {
                            //Logger.LogInformation($"QueryLicenceToItemId<<70.16>>:LicenseType:{LicenseType}" );
                            //生效时间早于等于今天      失效时间 大于等于明年今天，小于明年今天再过一个月；
                            if (startDate <= _today && expireDate >= _nextTodayOfNextYear && expireDate < _nextLastdayOfNextYear)
                            {

                                //Logger.LogInformation($"QueryLicenceToItemId<<70.16.1>>:getItemId:{item.ItemId}");
                                return item.ItemId;
                            }
                        }
                        if (LicenseType == 3)  //终身
                        {
                            //Logger.LogInformation($"QueryLicenceToItemId<<70.17>>:LicenseType:{LicenseType}" );
                            if (startDate <= _today && expireDate == DateTime.Parse(LAST_RECORD_DAY))
                            {
                                //Logger.LogInformation($"QueryLicenceToItemId<<70.17.1>>:getItemId:{item.ItemId}" );
                                return item.ItemId;
                            }
                        }
                    }
                }

            }
            //判断gr返回的值，根据worker或crator和lisence
            Logger.LogInformation(" QueryLicenceToItemId<<70.18>>:getItemId:0" );
            return "0";
        }


        //private async Task<bool> AssignLisence(LicenseClient licenseclient, long companyid, string itemId, int LisenceCount)
        //{
        //    Logger.LogInformation(" AssignLisence<<80.1>>");
        //    var request = new EntUC.Service.Configuration.AssignLicenseToTenantRequest()
        //    {
        //        CompanyId = companyid
        //    };

        //    request.Items.Add(new EntUC.Service.Configuration.AssignLicenseItem
        //    {
        //        ItemId = itemId,
        //        Quantity = LisenceCount
        //    });

        //    var resp = await licenseclient.AssignLicenseToTenantAsync(request, null);
        //    Logger.LogInformation($" AssignLisence<<80.2>>:AssignLicenseToTenantAsync：{resp}");
        //    return resp.Success;
        //}





        private async Task<bool> AssignLisence(LicenseClient licenseclient, long companyid, string itemId,  int LisenceCount)
        {
            
            Logger.LogInformation(" AssignLisence<<80.1>>");

            var request = new EntUC.Service.Configuration.AssignLicenseToTenantRequest()
            {
                CompanyId = companyid
            };

            //var existed = await licenseclient.GetLicenseInfoByTenantAsync(new GetLicenseInfoByTenantRequest { CompanyId = companyid });
            //var existedQuntity = existed.Capacities.FirstOrDefault(p => p.ItemId == itemId)?.Quantity ?? 0;
            ////Logger.LogInformation($" AssignLisence<<80.1.1>>：{existed.ToString()}");

            //request.Items.Add(new EntUC.Service.Configuration.AssignLicenseItem
            //{
            //    ItemId = itemId,
            //    Quantity = LisenceCount + existedQuntity
            //});

            //var resp1 = await licenseclient.AssignLicenseToTenantAsync(request);

            bool ret = false;
            var existed = await licenseclient.GetLicenseInfoByTenantAsync(new GetLicenseInfoByTenantRequest { CompanyId = companyid });
            // Logger.LogInformation($" AssignLisence<<80.1.1>>：{existed.ToString()}");
            foreach (var item in existed.Capacities.ToList())
            {
                //Logger.LogInformation($" AssignLisence<<80.1.1>>：{item.ItemId }===={itemId}");
                if (item.ItemId == itemId)
                {
                    //Logger.LogInformation($" AssignLisence<<80.1.2>>：{item.ItemId }===={itemId}");
                    item.Quantity += LisenceCount;
                    Logger.LogInformation($" AssignLisence<<80.1.3>>：{ item.Quantity}");
                    ret = true;
                }
                
               
                if (item.Quantity > 0)
                {
                    request.Items.Add(new AssignLicenseItem
                    {
                        ItemId = item.ItemId,
                        Quantity = item.Quantity
                    });
                }
            }
            if (ret == false)
            {
                request.Items.Add(new AssignLicenseItem
                {
                    ItemId = itemId,
                    Quantity = LisenceCount
                }) ;
            }
            //Logger.LogInformation($" AssignLisence<<80.1.2>>：{existed.ToString()}");
            Logger.LogInformation($" AssignLisence<<80.1.2>>：{ request.Items.ToString()}");

            //var existedQuntity = existed.Capacities.FirstOrDefault(p => p.ItemId == itemId)?.Quantity ?? 0;
            //Logger.LogInformation($" AssignLisence<<80.1.1>>：{existed.ToString()}");

            //request.Items.Add(new EntUC.Service.Configuration.AssignLicenseItem
            //{
            //    ItemId = itemId,
            //    Quantity = LisenceCount + existedQuntity
            //});

            var resp = await licenseclient.AssignLicenseToTenantAsync(request);
            Logger.LogInformation($" AssignLisence<<80.2>>:AssignLicenseToTenantAsync：{resp}");
            return resp.Success;
        }

        /// <summary>
        /// 创建租户信息并写关系表和订阅表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectname"></param>
        /// <param name="conn"></param>
        /// <param name="tenantClient"></param>
        /// <param name="ticketClient"></param>
        /// <param name="systemConfigurationClient"></param>
        /// <returns></returns>
        private async Task<BaseResponse<ResponeSuccess>> DoCreateTenantInfo(
            RequestInfo request, 
            string projectname,
           IFreeSql conn,
           TenantClient tenantClient,
           TicketClient ticketClient,
           SystemConfigurationClient systemConfigurationClient)
        {
            //Logger.LogInformation($" <<50>>:doCreateTenantInfo begin");
            int status = 0;
            long tenantid = await DoCreateTenant(projectname, tenantClient);
            //成功生成租户ID，生成邀请码，写关系表和订阅表，成功生成邀请码，订阅信息表status为1，邀请码生成失败则订阅信息表status为-1
            //生成租户不成功，也写订阅表，status为-1
            if (tenantid > 0)
            {
                //Logger.LogInformation($" <<50.1>>:doCreateTenantInfo suceess");
                //string retUrl = await GetAdministratorInvitationUrl(tenantid, tenantClient, ticketClient, systemConfigurationClient);
                string retUrl = await GetAdministratorInvitationUrl2(tenantid, request, conn, tenantClient, ticketClient, systemConfigurationClient);
                Logger.LogInformation($" <<50.2>>: GetAdministratorInvitationUrl end {retUrl}");
                status = retUrl == "error" ? -1 : 1;
                //Logger.LogInformation(" <<50.3>>: doWriteRelationAndSubscribe begin");
                await DoWriteRelationAndSubscribe(status, request, conn, new SubscribeArgs
                {
                    companyid = tenantid,
                    tenantid_zsy = request.tenant,
                    name = projectname,
                    projectid = request.projectId
                }, 1);
                //Logger.LogInformation(" <<50.4>>: doWriteRelationAndSubscribe end");

                //写订阅信息和关联信息成功
                //if (ret == true)
                //{
                    //返回下载的地址
                    //Logger.LogInformation(" <<50.5>>: doWriteRelationAndSubscribe sucesses");
                    if (status == 1)
                    {
                        //Logger.LogInformation($" <<50.6>>: return retUrl",retUrl);
                        return new BaseResponse<ResponeSuccess>(new ResponeSuccess { 
                            saasUrl = retUrl,
                            companyId = tenantid
                        });
                    }
                    else 
                    { 
                        return new BaseResponse<ResponeSuccess>(code: "000501", message: "BAD_SQL_ERROR"); 
                    } //返回错误的消息码}
                //}
                //else 
                //{ 
                //    return new BaseResponse<ResponeSuccess>(code: "000501", message: "BAD_SQL_ERROR"); 
                //}//返回错误消息
                
                //return new BaseResponse<ResponeSuccess>(new ResponeSuccess { saasUrl = retUrl })=ret=tru
                
            } 
            else 
            {
                //如果租户创建失败
                Logger.LogInformation(" <<4.6>>:doCreateTenantInfo Error");
                status = -1;
                //request.tenant = "-1";

                await DoWriteSubscription(status, DateTime.UtcNow, request, conn,new SubscribeArgs { companyid = -1 }, 1);
                //Logger.LogInformation($" <<4.7>>:doWriteSubscription status {status}");
                //conn.Insert(new SubscriptionModel { }).ExecuteAffrows();
                return new BaseResponse<ResponeSuccess>(code: "000501", message: "BAD_SQL_ERROR");
            }//返回创建租户失败错误码

                         

        }

        public class SubscribeArgs
        {
            public long companyid { get; set; }
            public string? tenantid_zsy { get; set; }
            public string ? name { get; set; }
            public string ? projectid { get; set; }


        }

        /// <summary>
        /// 写关系表和订阅信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="request"></param>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task DoWriteRelationAndSubscribe(int status, RequestInfo request, IFreeSql conn , SubscribeArgs args, int isCreateTenant )
        {
            int licenseType = 0;
            
            if (!string.IsNullOrEmpty(request.specValues)) {
                    
                string[] array = request.specValues.ToString().Split("-");
                string licenseType_s = array[0].Substring(1);
                //string licenseCount_s = array[1];
                if (licenseType_s.Equals("1")) { licenseType = 1; }
                if (licenseType_s.Equals("2")) { licenseType = 2; }
                if (licenseType_s.Equals("3")) { licenseType = 3; }
            }
            conn.Transaction(() =>
            {
                DateTime now = DateTime.UtcNow;
                //写关系表
                conn.Insert(new RelationshipModel
                {
                    CompanyId = args.companyid,//关系表的CompanyId由系统自动生成
                    TenantId_zsy = args.tenantid_zsy??String.Empty,
                    Name = args.name??String.Empty,
                    Projectid = args.projectid??String.Empty,
                    Projectname = "1",
                    CreateTime = now
                }).ExecuteAffrows();//args里的参数
                //写订阅信息--状态
                //licenseType  1-试用 2-1年 3-买断
                DateTime ls_endTime = licenseType == 1 ? now.AddMonths(1) :
                    licenseType == 2 ? now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).AddYears(1)
                    : DateTime.Parse(LAST_RECORD_DAY);//now.AddYears(1000);

                int status = 1;
                    
                //写订阅表
                DoWriteSubscription(status, ls_endTime, request, conn,  args, isCreateTenant);

            });
           
            return System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// 写订阅表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ls_endTime"></param>
        /// <param name="request"></param>
        /// <param name="conn"></param>
        /// <exception cref="Exception"></exception>
        private System.Threading.Tasks.Task DoWriteSubscription(int status, DateTime ls_endTime, RequestInfo request, IFreeSql conn, SubscribeArgs args, int isCreateTenant)
        {
            //Logger.LogInformation($"doWriteSubscription(<<90>>:begin");
            int QueryOrderType = 1;
            int licenseCount = 1;
            string tmp_productNo = "";
            
            if (!string.IsNullOrEmpty(request.specValues))
            {
                string[] array= request.specValues.ToString().Split("-");
                string licenseCount_s = array[1];//取licesnsecount数量
                licenseCount = int.Parse(licenseCount_s);
            }

            if (!string.IsNullOrEmpty(request.productNo))
            {
                //if (request.productNo.Equals("FtuibotCommander")) { QueryOrderType = 1; }
                if (request.productNo.Equals("FtuibotWorkerFormal")|| request.productNo.Equals("FtuibotWorkerTrial")) { QueryOrderType = 2; }
                if (request.productNo.Equals("FtuibotCreator")|| request.productNo.Equals("FtuibotCreatorTest") ) { QueryOrderType = 3; }
                //added by wenz 1010
                if (request.productNo.Equals("FtuibotWorkerAttendedFormal") || request.productNo.Equals("FtuibotWorkerAttendedTrial")) { QueryOrderType = 4; }
            }

            if (isCreateTenant == 1)
            {
                tmp_productNo = "FtuibotCommander";
                //tmp_productNo = request.productNo;
                QueryOrderType = 1;
            }
            else
            {
                tmp_productNo = request.productNo;
            }

            DateTime requestTime_zsy = DateTime.MinValue;
            if (!string.IsNullOrEmpty(request.requestTime))
            {
                DateTimeOffset d = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(request.requestTime)/1000);
                requestTime_zsy = DateTime.Parse(d.ToString());
            }


            conn.Insert(new SubscriptionModel
            {
                //OrderNo = request.orderNo,
                OrderNo_zsy = request.orderNo,
                Async = request.async,
                //Spec = request.spec,
                SpecNames = request.specNames,
                SpecValues = request.specValues,
                ChargeType = request.chargeType,
                Ordertype = QueryOrderType,
                Count = licenseCount,
                ProductNo_zsy = tmp_productNo,
                VersionNo = request.versionNo,
                Channel = request.channel,
                Companyid = args.companyid,
                ProjectId_zsy = request.projectId,
                ProjectType_zsy = request.projectType,
                UserName_zsy = request.userName,
                RequestTime_zsy = requestTime_zsy,
                CreateTime = DateTime.UtcNow,
                Ls_begintime = DateTime.UtcNow,
                Ls_endtime = ls_endTime,
                Status = status,
                Memo = ACCESS_KEY,

            }).ExecuteAffrows();//requestinfo里的参数  status的状态
            
            if (QueryOrderType == 1)
            {
                Logger.LogInformation($"doWriteSubscription(<<90.1>>:status:{status},orderType:{QueryOrderType},companyid:{args.companyid},projectId:{request.projectId},userName:{request.userName}");
            }
            //Logger.LogInformation($"doWriteSubscription(<<90>>:end");
            return System.Threading.Tasks.Task.CompletedTask;
        }




        //[HttpGet(Name = "GetWeatherForecast")]
        //public async Task<IEnumerable<WeatherForecast>> Get(
        //    [FromBody]TestModel request,
        //    [FromServices]IFreeSql conn,
        //    [FromServices]TenantClient tenantClient)
        //{

            //    conn.Transaction(() =>
            //    {
            //        conn.Insert(new TestModel { }).ExecuteAffrows();
            //        conn.Insert(new TestModel { }).ExecuteAffrows();
            //        conn.Insert(new TestModel { }).ExecuteAffrows();
            //        conn.Insert(new TestModel { }).ExecuteAffrows();
            //    });

            //    var resp = await tenantClient.CreateTenantAsync(new EntUC.Service.Tenant.CreateTenantRequest
            //    {
            //        AdministratorEmployeeId = 1,
            //    });
            //    // 邀请的UserId写死为0
            //    // 邀请的UserName写死为空
            //    // resp.Id

            //    conn.Insert(new TestModel { }).ExecuteAffrows();

            //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //    {
            //        Date = DateTime.Now.AddDays(index),
            //        TemperatureC = Random.Shared.Next(-20, 55),
            //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //    })
            //    .ToArray();
            //}


            /// <summary>
            /// 根据招商云的租户ID查询关系表中系统生成的租户ID
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="TenantId"></param>
            /// <param name="Projectid"></param>
            /// <returns></returns>
        private async Task<long> QueryTenant(IFreeSql conn,string TenantId,string Projectid, TenantClient tenantClient)
        {
            long companyId = 0;
            //Logger.LogInformation($"QueryTennat <<40.1>>:QueryTenant begin:{companyId}");
            var RelationshipModelList= await conn.Select<RelationshipModel>().Where(a => a.TenantId_zsy == TenantId && a.Projectid == Projectid ).ToListAsync();//&& !a.Projectname.Equals("0")
            if (RelationshipModelList.Count > 0)
            {
                companyId = RelationshipModelList[0].CompanyId;
            }
            Logger.LogInformation($"QueryTennat <<40.2>>:QueryTenant companyId:{companyId}" );

            if (companyId > 0)
            {
                var tenantResponse = await tenantClient.GetTenantByIdAsync(new GetTenantByIdRequest
                {
                    Id = companyId,
                });
                Logger.LogInformation($"QueryTennat <<40.3>>:QueryTenant 租户是否启用:{tenantResponse.Tenant.IsEnabled}");
                string str_isEnabled = tenantResponse.Tenant.IsEnabled==true ? "1":"0";
                try
                {
                    conn.Transaction(() =>
                    {
                        UpdateRelation(companyId, Projectid, str_isEnabled, conn);
                        UpdateSubscription(companyId, Projectid, str_isEnabled, conn);
                    });
                }
                catch(Exception ex)
                {
                    Logger.LogError($"QueryTennat <<40.4>>:QueryTenant 更新关系表订阅表错误:{ex}");
                }
                finally
                {
                    companyId = tenantResponse.Tenant.IsEnabled == true ? companyId : -1;//被禁用的租户ID返回-1
                }
                
            }
            Logger.LogInformation($"QueryTennat <<40.5>>:QueryTenant end:{companyId}");
            return companyId;

        }


        /// <summary>
        /// 更新关系表租户状态
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="conn"></param>
        private System.Threading.Tasks.Task UpdateRelation(long companyId, string Projectid, string str_isEnabled, IFreeSql conn)
        {
            Logger.LogInformation($"updateRelation(<<10>>:begin");
            
            conn.Update<RelationshipModel>()
                .Set(item => item.Projectname, str_isEnabled)
                .Where(item => item.CompanyId == companyId && item.Projectid==Projectid)
                .ExecuteAffrows();
            
            Logger.LogInformation($"updateRelation(<<10>>:end");
            return System.Threading.Tasks.Task.CompletedTask;
        }


        /// <summary>
        /// 更新订阅表租户状态
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="conn"></param>
        private System.Threading.Tasks.Task UpdateSubscription(long companyId, string Projectid, string str_isEnabled, IFreeSql conn)
        {
            Logger.LogInformation($"updateSubscription(<<11>>:begin");
            
            conn.Update<SubscriptionModel>()
                .Set(item => item.Spec, str_isEnabled)
                .Where(item => item.Companyid == companyId && item.ProjectId_zsy==Projectid)
                .ExecuteAffrows();
            
            Logger.LogInformation($"updateSubscription(<<11>>:end");
            return System.Threading.Tasks.Task.CompletedTask;
        }



        /// <summary>
        /// 生成邀请码
        /// </summary>
        /// <param name="TenantId"></param>
        /// <param name="tenantClient"></param>
        /// <param name="ticketClient"></param>
        /// <param name="systemConfigurationClient"></param>
        /// <returns></returns>
        private async Task<string> GetAdministratorInvitationUrl(long TenantId, 
            TenantClient tenantClient,
            TicketClient ticketClient,
            SystemConfigurationClient systemConfigurationClient)
        {
            //查询租户信息
            Logger.LogInformation("QueryTennat <<60>>:GetAdministratorInvitationUrl:begin");
            var tenantResponse = await tenantClient.GetTenantByIdAsync(new GetTenantByIdRequest
            {
                Id = TenantId,
                
            });

            

            if (tenantResponse.Tenant == null)
            {
                //throw new LaiyeException(400, "租户不存在");
                Logger.LogInformation("QueryTennat <<60.1>>:GetAdministratorInvitationUrl:Tenant is null");
                return "error";
            }

            if (tenantResponse.Tenant.IsEnabled == false)
            {
                //throw new LaiyeException(400, "租户已被禁用，无法邀请");
                Logger.LogInformation("QueryTennat <<60.2>>:GetAdministratorInvitationUrl:IsEnabled");
                return "error";
            }

            //var employee = employeeClient.GetEmployeeByAdmin(new GetEmployeeByAdminRequest
            //{
            //    CompanyId = TenantId,
            //}); 

            var createTicketRequest = new CreateOrGetTicketByMapIdRequest
            {
                Type = ETicketType.UserJoinToCompanyWithAdministratorRole,
                ExpiredTime = DateTime.UtcNow.AddDays(3).ToTimestamp(),
                MapId = TenantId,
            };

            createTicketRequest.Params.Add("CompanyId", TenantId.ToString());
            createTicketRequest.Params.Add("CompanyName", tenantResponse.Tenant.Name);
            createTicketRequest.Params.Add("InviterUserId", "1");
            createTicketRequest.Params.Add("InviterUserName", "admin");
            Logger.LogInformation("QueryTennat <<60.3>>:GetAdministratorInvitationUrl:createTicketReques");
            var createTicketResponse = await ticketClient.CreateOrGetTicketByMapIdAsync(createTicketRequest);
            Logger.LogInformation($"QueryTennat <<60.4>>:GetAdministratorInvitationUrl:createTicketResponse {createTicketResponse}" );
            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            Logger.LogInformation($"QueryTennat <<60.5>>:GetAdministratorInvitationUrl:webConfigurationResponse {webConfigurationResponse}" );

            return ($"{EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl)}/api/identity/invite/commanderAdministrator?ticket={createTicketResponse.Ticket.Ticket}");

        }

        /// <summary>
        /// 生成邀请码前先检查之前是否已经生成邀请码，避免重复生成邀请码
        /// </summary>
        /// <param name="TenantId"></param>
        /// <param name="request"></param>
        /// <param name="conn"></param>
        /// <param name="tenantClient"></param>
        /// <param name="ticketClient"></param>
        /// <param name="systemConfigurationClient"></param>
        /// <returns></returns>
        private async Task<string> GetAdministratorInvitationUrl2(long TenantId,
            RequestInfo request,
            IFreeSql conn,
            TenantClient tenantClient,
            TicketClient ticketClient,
            SystemConfigurationClient systemConfigurationClient)
        {
            //查询租户信息
            Logger.LogInformation("QueryTennat <<61>>:GetAdministratorInvitationUrl2:begin");
            var tenantResponse = await tenantClient.GetTenantByIdAsync(new GetTenantByIdRequest
            {
                Id = TenantId,

            });

            if (tenantResponse.Tenant == null)
            {
                //throw new LaiyeException(400, "租户不存在");
                Logger.LogInformation("QueryTennat <<61.1>>:GetAdministratorInvitationUrl2:Tenant is null");
                return "error";
            }

            if (tenantResponse.Tenant.IsEnabled == false)
            {
                //throw new LaiyeException(400, "租户已被禁用，无法邀请");
                Logger.LogInformation("QueryTennat <<61.2>>:GetAdministratorInvitationUrl2:Tenant IsDisabled}");
                return "error";
            }

            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            Logger.LogInformation($"QueryTennat <<61.5>>:GetAdministratorInvitationUrl2:webConfigurationResponse {webConfigurationResponse}");

            bool IsExsitedInvitationUrl =await QueryInvitationUrlIsExisted(conn, TenantId, request);
            if(IsExsitedInvitationUrl == true)
            {
                return ($"{EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl)}");
            }


            var createTicketRequest = new CreateOrGetTicketByMapIdRequest
            {
                Type = ETicketType.UserJoinToCompanyWithAdministratorRole,
                ExpiredTime = DateTime.UtcNow.AddDays(3).ToTimestamp(),
                MapId = TenantId,
            };

            createTicketRequest.Params.Add("CompanyId", TenantId.ToString());
            createTicketRequest.Params.Add("CompanyName", tenantResponse.Tenant.Name);
            createTicketRequest.Params.Add("InviterUserId", "1");
            createTicketRequest.Params.Add("InviterUserName", "admin");
            Logger.LogInformation("QueryTennat <<61.3>>:GetAdministratorInvitationUrl2:createTicketReques");
            var createTicketResponse = await ticketClient.CreateOrGetTicketByMapIdAsync(createTicketRequest);
            Logger.LogInformation($"QueryTennat <<61.4>>:GetAdministratorInvitationUrl2:createTicketResponse {createTicketResponse}");


            //bool isSaved = doWriteInvitationUrl(request,conn, TenantId);
            //if (isSaved == false)
            //{
            //    return ($"{EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl)}");
            //}
            try
            {
                await DoWriteInvitationUrl(request, conn, TenantId);
            }catch(Exception ex)
            {
                return ($"{EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl)}");
            }

            return ($"{EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl)}/api/identity/invite/commanderAdministrator?ticket={createTicketResponse.Ticket.Ticket}");

        }

        /// <summary>
        /// 保存生成生成邀请码记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task DoWriteInvitationUrl(RequestInfo request, IFreeSql conn, long TenantId)
        {
            Logger.LogInformation($"doWriteInvitationUrl(<<62.0>>:begin orderNo:{request.orderNo},companyid:{TenantId},projectId:{request.projectId},userName:{request.userName},{ACCESS_KEY}");
            string p_userName = "";
            try
            {
                p_userName = SecurityUtils.dataDecryption(request.userName, ACCESS_KEY);
            }
            catch (Exception ex)
            {
                Logger.LogError($"doWriteInvitationUrl <<62.4>>:doWriteInvitationUrl, {ex},解密异常:{request.userName},key:{ACCESS_KEY}");
            }
            Logger.LogInformation($"doWriteInvitationUrl(<<62.1>>:userName:{p_userName}");
            //try 
            //{ 
                conn.Insert(new InvitationUrlModel
                {
                    Companyid = TenantId,
                    ProjectId_zsy = request.projectId??String.Empty,
                    UserName_zsy = p_userName,
                    CreateTime = DateTime.UtcNow,

                }).ExecuteAffrows();
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogInformation($"doWriteInvitationUrl(<<62.2>>:ex", ex.ToString());
            //    return false;
            //}
            
            Logger.LogInformation($"doWriteInvitationUrl(<<62.3>>:end");
            //return true;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// 用户是否生成过邀请码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="companyId"></param>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        private async Task<bool> QueryInvitationUrlIsExisted(IFreeSql conn, long companyId, RequestInfo requestInfo)
        {
            bool bln_ret = false;
            Logger.LogInformation($"QueryInvitationUrlIsExisted <<63.1>>:QueryInvitationUrlIsExisted begin:{companyId},{requestInfo.projectId},{requestInfo.userName},{ACCESS_KEY}");
            string p_userName = "";
            try
            {
                p_userName = SecurityUtils.dataDecryption(requestInfo.userName, ACCESS_KEY);
            }
            catch (Exception ex)
            {
                Logger.LogError($"QueryInvitationUrlIsExisted <<63.4>>:QueryInvitationUrlIsExisted, 解密异常:{requestInfo.userName},key:{ACCESS_KEY}");
            }
            Logger.LogInformation($"QueryInvitationUrlIsExisted <<63.2>>p_userName:{p_userName}");
            var InvitationUrlModelList = await conn.Select<InvitationUrlModel>()
                                            .Where(a => a.Companyid == companyId && a.ProjectId_zsy == requestInfo.projectId
                                            && a.UserName_zsy == p_userName).ToListAsync();//.ToList();

            if (InvitationUrlModelList != null && InvitationUrlModelList.Count > 0)
            {
                bln_ret = true;
            }

            Logger.LogInformation($"QueryInvitationUrlIsExisted <<63.3>>:QueryInvitationUrlIsExisted end Companyid:{companyId},projectId:{requestInfo.projectId},userName:{p_userName},是否之前已生成邀请码:{bln_ret}");
            return bln_ret;
        }


        private string EnsureNoSlash(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

            if (url.EndsWith("/"))
            {
                return url.Substring(0, url.Length - 1);
            }

            return url;
        }




        private async Task<bool> QuerySubscriptionIsExisted(IFreeSql conn, long companyId, RequestInfo requestInfo, TenantClient tenantClient)
        {
            bool bln_ret = false;

            string str_userName = "";
            try
            {
                str_userName = SecurityUtils.dataDecryption(requestInfo.userName, ACCESS_KEY);
            }
            catch (Exception ex)
            {
                Logger.LogError($"QuerySubscriptionIsExisted <<12.4>>:QuerySubscriptionIsExisted, 解密异常:{requestInfo.userName},key:{ACCESS_KEY}");
            }


            Logger.LogInformation($"QuerySubscriptionIsExisted <<12.1>>:QuerySubscriptionIsExisted begin:{companyId},{requestInfo.projectId},{requestInfo.userName}");
            var SubscriptionModelList = await conn.Select<SubscriptionModel>()
                                            .Where(a => a.Companyid == companyId && a.ProjectId_zsy == requestInfo.projectId
                                            // && SecurityUtils.dataDecryption(a.UserName_zsy, ACCESS_KEY) == str_userName
                                            && a.Status == 1 && a.Ordertype == 1).ToListAsync();//.ToList();

            if (SubscriptionModelList !=null && SubscriptionModelList.Count > 0)
            {
                foreach(var item in SubscriptionModelList)
                {
                    string temp_userName = SecurityUtils.dataDecryption(item.UserName_zsy, ACCESS_KEY);
                    Logger.LogInformation($"QuerySubscriptionIsExisted <<12.2>>:QuerySubscriptionIsExisted, userName:{temp_userName}");
                    if (temp_userName.Equals(str_userName)){
                        bln_ret = true;
                        break;
                    }
                }
                
            }
            
            Logger.LogInformation($"QuerySubscriptionIsExisted <<12.3>>:QuerySubscriptionIsExisted end, Companyid:{companyId},projectId:{requestInfo.projectId},userName:{str_userName},是否之前已经订阅:{bln_ret}");
            return bln_ret;
        }

        
        /// <summary>
        /// 创建租户
        /// </summary>
        /// <param name="TenantName"></param>
        /// <param name="tenantClient"></param>
        /// <returns></returns>
        private async Task<long>  DoCreateTenant(string TenantName, [FromServices] TenantClient tenantClient)
        {
            Logger.LogInformation($"doCreateTenant <<13.1>>:doCreateTenant begin :{TenantName}");
            var resp = await tenantClient.CreateTenantAsync(new EntUC.Service.Tenant.CreateTenantRequest
            {
                //名称
                Name = TenantName,

            });
            Logger.LogInformation($"doCreateTenant <<13.2>>:doCreateTenant end :{resp.Id}");
            return resp.Id;

            
        }

        [HttpPost("getSubscription")]
        public async Task<ResponseSubscriptionList> GetSubscription([FromServices] IFreeSql conn, [FromBody] GetSubscriptionListModel requestModel)
        {
            Logger.LogInformation($"QuerySubscription <<14.1>>:QuerySubscription begin Companyid:{requestModel.companyid},ProductNo:{requestModel.productNo_zsy},StartTime:{DateTime.Parse(requestModel.startTime.ToString())},EndTime:{DateTime.Parse(requestModel.endTime.ToString()).AddDays(1)}");
            if(requestModel.companyid != null && requestModel.companyid.Any())
            {
                //if(requestModel.Companyid[0]) 
                    
                    Logger.LogInformation($"QuerySubscription <<14.4>>requestModel.Companyid[0]:{requestModel.companyid[0]}");
            }
            var SubscriptionModelList = await conn.Select<SubscriptionModel, RelationshipModel>()
                                            .LeftJoin((a, b) => a.Companyid == b.CompanyId)
                                            .Where((a, b) => a.Status == 1 && !a.Ordertype.Equals("1"))
                                            .WhereIf(requestModel.companyid != null && requestModel.companyid.Any(), (a, b) => requestModel.companyid.Contains(a.Companyid))
                                            .WhereIf(requestModel.productNo_zsy != null && requestModel.productNo_zsy.Any(), (a, b) => requestModel.productNo_zsy.Contains(a.ProductNo_zsy))
                                            .WhereIf(requestModel.startTime != null, (a, b) => a.CreateTime >= DateTime.Parse(requestModel.startTime.ToString()))
                                            .WhereIf(requestModel.endTime != null, (a, b) => a.CreateTime < DateTime.Parse(requestModel.endTime.ToString()).AddDays(1))
                                            .OrderByDescending((a, b) => a.CreateTime)
                                            .Count(out var total) //总记录数量
                                            .Page(requestModel.pageIndex, requestModel.pageSize)
                                            .ToListAsync((a, b) => new { a, b });
                                            //.ToList((a,b)=>new {a,b});

            List<ResponseSubscriptionListModel> responseSubscriptionList = new List<ResponseSubscriptionListModel>();
            if (SubscriptionModelList != null && SubscriptionModelList.Count > 0)
            {
                Logger.LogInformation($"QuerySubscription <<14.2>>:SubscriptionModelList:{SubscriptionModelList}");
                
                foreach (var item in SubscriptionModelList)
                {
                    ACCESS_KEY = item.a.Memo;
                    string temp_userName = "";
                    string temp_productNo = item.a.ProductNo_zsy;
                    if (string.IsNullOrEmpty(ACCESS_KEY))
                    {
                        temp_userName = item.a.UserName_zsy;
                    }
                    try
                    {
                        temp_userName = SecurityUtils.dataDecryption(item.a.UserName_zsy, ACCESS_KEY);
                    }
                    catch(Exception ex) 
                    {
                        temp_userName = item.a.UserName_zsy;
                    }

                    if(item.a.ProductNo_zsy.Equals("FtuibotWorkerFormal") || item.a.ProductNo_zsy.Equals("FtuibotWorkerTrial")) { temp_productNo = "无人值守-运行时浮动授权-流程机器人"; }
                    if (item.a.ProductNo_zsy.Equals("FtuibotCreator")) { temp_productNo = "浮动授权-流程创造者"; }
                    if (item.a.ProductNo_zsy.Equals("FtuibotWorkerAttendedFormal")) { temp_productNo = "人机交互浮动授权-流程机器人"; }

                        string[] tmpName = new string[2];
                    try
                    {
                        tmpName = item.b.Name.Split("/");
                    }
                    catch (Exception ex)
                    {
                        tmpName[0] = "";
                        tmpName[1] = "";
                    }

                    responseSubscriptionList.Add(new ResponseSubscriptionListModel
                    {
                        createTime = item.a.CreateTime,
                        userName = temp_userName,
                        orderNo_zsy = item.a.OrderNo_zsy,
                        projectId = item.a.ProjectId_zsy,
                        projectName = tmpName[1],
                        companyid = item.a.Companyid,
                        companyName = tmpName[0],
                        productNo_zsy = temp_productNo,
                        count = item.a.Count,
                        ls_endtime = item.a.Ls_endtime
                    });


                }
                Logger.LogInformation($"QuerySubscription <<14.3>>total:{total}");
            }

            return new ResponseSubscriptionList
            {
                total = total,
                pageIndex = requestModel.pageIndex,
                pageSize = requestModel.pageSize,
                data = responseSubscriptionList
            };

            //return new BaseResponse<ResponseSubscriptionList>(new ResponseSubscriptionList
            //{
            //    total = total,
            //    pageIndex=requestModel.pageIndex,
            //    pageSize=requestModel.pageSize,
            //    data= responseSubscriptionList
            //},"0","SUCCESS");

        }

        private async Task<List<RelationshipModel>> QueryRelationshipByCompanyId(IFreeSql conn, long companyId)
        {

            var RelationshipModelList = await conn.Select<RelationshipModel>()
                                            .Where(a => a.CompanyId == companyId).ToListAsync();//.ToList();
            return RelationshipModelList;
        }

        public class EvnetArgs
        {
            public long Id { get; set; }
        }

        [HttpPost("createSub")]
        public async System.Threading.Tasks.Task CreateSub([FromServices]ICapPublisher publisher)
        {
            // 插入数据库创建订阅记录 处理中

            await publisher.PublishAsync("zsj.createSub", new EvnetArgs { Id = 11 });

        }

        private static Random Random = new Random();


        public class ReceiveEventRequest
        {
            public string? Event { get; set; }

            public CommonEventArgs? Args { get; set; }
        }

        [HttpPost("ReceiveEvent")]
        public async System.Threading.Tasks.Task ReceiveEvent([FromBody] ReceiveEventRequest request,
            [FromServices]ICapPublisher publisher)
        {
            await publisher.PublishAsync(request.Event, request.Args);
        }



        [HttpGet("getFlowById")]
        public async Task<BaseResponse<ResponeSuccess2>> GetFlowById(
            [FromQuery] RequestInfo request,
            [FromServices] IFreeSql conn,
            [FromServices] TenantClient tenantClient,
            [FromServices] FlowClient flowClient,
            [FromServices] EmployeeClient employeeClient,
            [FromServices] Attachment.AttachmentClient attachmentClient,
            [FromServices] CmdAttachmentClient cmdAttachmentClient,
            [FromServices] SystemConfigurationClient systemConfigurationClient,
            [FromServices] ICapPublisher publisher)

        {
            if (!Request.Headers.TryGetValue("authToken", out var headerValue))
            {
                Logger.LogInformation($"getFlowById<<1>>:The authToken field is required");
                return new BaseResponse<ResponeSuccess2>(code: "400", message: "参数authToken是必须的");
            }


            var webConfigurationResponse = await systemConfigurationClient.GetWebConfigurationInfoAsync(new GetWebConfigurationInfoRequest { });
            Logger.LogInformation($"getFlowById<<2>>:GetWebConfigurationInfoAsync:webConfigurationResponse {webConfigurationResponse}");
            string loginurl = EnsureNoSlash(webConfigurationResponse.WebConfigurationInfo.HttpUrl);

            string headerscode = headerValue;
            Logger.LogInformation($"getFlowById<<3>>:getheaderscode {headerscode}");
            string[] projectcode = GetProjectName(request.projectId);
            //Logger.LogInformation($"getFlowById<<4>>:projectcode {projectcode}");
            if (projectcode[0] == "error") { return new BaseResponse<ResponeSuccess2>(code: "000501", message: "未找到项目信息"); }
            //string accesskey = request.tenant + '-' + projectcode[0];
            ACCESS_KEY = request.tenant + '-' + projectcode[0];
            Logger.LogInformation($"getFlowById<<5>>:accesskey,{ACCESS_KEY}");

            bool checkvalue = DoCheckHeadsinfo2(ACCESS_KEY, headerscode, request);
            checkvalue = true;
            Logger.LogInformation($"getFlowById<<6>>:CheckHeadsinfo {checkvalue}");
            if (checkvalue == false)
            {
                GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "VALIDATE_ERROR"), "", "");
                return new BaseResponse<ResponeSuccess2>(code: "000400", message: "验签错误");

            }


            string tenantid = projectcode[2];//根据传入的ProjectId得到的招商云租户ID
            //Logger.LogInformation($"getFlowById<<7>>:tenant {tenantid}");
            string projectid = request.projectId;
            Logger.LogInformation($"getFlowById<<8>>:tenant:{tenantid}, projectId:{projectid}");

            //商品上架：在specValues字段里放如下值，用"|"分隔，companyid|flowid|flowversion
            string[] array = new string[3];
            long originCompanyId = 0;
            long flowId = 0;
            long flowVersion = 0;
            try
            {
                array = request.specValues.ToString().Split("-");
                originCompanyId = Convert.ToInt64(array[0].Substring(1));
                flowId = long.Parse(array[1]);
                flowVersion = long.Parse(array[2]);
                //if (!tenantid.Equals(array[0])) return new BaseResponse<ResponeSuccess2>(code: "000400", message: "租户ID与项目ID不匹配");
            }
            catch
            {
                return new BaseResponse<ResponeSuccess2>(code: "000400", message: "参数错误：specValues字段里放如下值，用|分隔，租户ID|流程ID|流程版本");
            }

            
            Logger.LogInformation($"getFlowById <<9>> CompanyId:{originCompanyId}, flowId:{flowId},flowVersion: {flowVersion}");

            string saasurl = "";
            string returl = "";
            long companyId = 0;//系统生成的租户ID

            //租户名称=租户名称+'/'项目名称
            //string projectname = projectcode[3] + '/' + projectcode[1];
            companyId = await QueryTenant(conn, tenantid, projectid, tenantClient);

            if (companyId == 0)//如果不存在
            {
                Logger.LogError($"GetFlowById <<10>>:QueryTenant tenant is not exist:{companyId}");
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "租户不存在，订阅失败"), "", "");
            }
            else if (companyId == -1)//租户被禁用
            {
                Logger.LogError($"GetFlowById <<11>>:QueryTenant tenant is disabled:{companyId}");
                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "租户已禁用，订阅失败"), "", "");
            }
            else
            {
                //如果租户存在,
                Logger.LogInformation($"getFlowById <<12>>:Tenant is exist:{companyId}");
                string mobile = "";
                try
                {
                    mobile = SecurityUtils.dataDecryption(request.userName, ACCESS_KEY);
                }catch(Exception ex)
                {
                    Logger.LogError($"GetFlowById <<14>>: 解密异常:{request.userName},key:{ACCESS_KEY}");
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "用户解密异常，订阅失败"), "", "");
                }
                Logger.LogInformation($"GetFlowById <<15>>mobile:{mobile}");

                //用户是否存在
                var response = await UserClient.GetUserByMobileAsync(new GetUserByMobileRequest { Mobile = mobile });
                var userObj = response.User;
                Logger.LogInformation($"GetFlowById <<16>>userObj:{userObj}");
                if (userObj == null)
                {
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "用户不存在，订阅失败"), "", "");
                }

                //用户在该租户是否存在
                var employeeResponse = await employeeClient.GetEmployeeByUserIdAsync(new GetEmployeeByUserIdRequest
                {
                    UserId = userObj.Id,
                    CompanyId = companyId
                });
                Logger.LogInformation($"GetFlowById <<17>>employeeResponse:{employeeResponse}");
                if (employeeResponse.Employee == null)
                {
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "用户与租户未关联，订阅失败"), "", "");
                }


                var flowResponse = await flowClient.GetFlowByIdAsync(new GetFlowByIdRequest
                {
                    CompanyId = originCompanyId,// companyId,
                    FlowId = flowId,
                });

                if (flowResponse.Flow == null)
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "找不到流程信息，可能共享用户已删除源文件，订阅失败"), "", "");

                string flowName = flowResponse.Flow.FlowName;
                Logger.LogInformation($"GetFlowById <<18>>flowResponse.Flow.FlowName:{flowName}");

                //获取流程版本信息，包含已删除的
                //var flowVersionResponse = await flowClient.GetFlowVersionSourceByIdAsync(new GetFlowVersionSourceByIdRequest
                //{
                //    CompanyId = originCompanyId,//companyId,
                //    FlowVersionId = flowVersion,
                //});

                //if (flowVersionResponse.FlowVersion == null)
                //    return getReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "找不到流程版本信息"), "", "");

                //Logger.LogInformation($"GetFlowById <<19>>flowVersionResponse.FlowVersion.Id:{flowVersionResponse.FlowVersion.Id}");


                //获取流程版本信息，不包含已删除的
                var flowVersionSourceResponse = await flowClient.GetFlowVersionByIdAsync(new GetFlowVersionByIdRequest
                {
                    CompanyId = originCompanyId,//companyId,
                    FlowVersionId = flowVersion,
                });
                
                //long flowVersionId = flowVersionResponse.FlowVersion.Id; //flowResponse.Flow.FlowVersionId;

                if (flowVersionSourceResponse.FlowVersion == null)
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "找不到流程版本信息，可能共享用户已删除源文件，订阅失败"), "", "");
                
                Logger.LogInformation($"GetFlowById <<20>>flowVersionSourceResponse.FlowVersion.FlowAttachmentId:{flowVersionSourceResponse.FlowVersion.FlowAttachmentId},flowVersionSourceResponse.FlowVersion.FileName:{flowVersionSourceResponse.FlowVersion.FileName}");

                string attachement_id = flowVersionSourceResponse.FlowVersion.FlowAttachmentId;
                //string fileName = flowVersionSourceResponse.FlowVersion.FileName;

                var attachmentResponse = await cmdAttachmentClient.GetAttachmentByIdAsync(new EntCmd.Service.Core.GetAttachmentByIdRequest { 
                    CompanyId= originCompanyId,//companyId, 
                    Id=attachement_id,
                    Type = 10000,

                });
                
                if (attachmentResponse.Attachment == null)
                    return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(code: "000400", message: "找不到流程附件信息"), "", "");
                
                Logger.LogInformation($"GetFlowById <<21>>attachmentResponse.Attachment.FileName:{attachmentResponse.Attachment.FileName},Path:{attachmentResponse.Attachment.Path}");


                //没有加入事务处理
                
                var resultFlow = await flowClient.CreateFlowAsync(new Laiye.EntCmd.Service.Core.CreateFlowRequest
                {
                    CompanyId = companyId,
                    CreateEmployeeId = employeeResponse.Employee.Id,
                    FlowName = flowName + "(" + Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString()+")",
                    MaxRunTime = flowResponse.Flow.MaxRunTime,
                    Remark = flowResponse.Flow.Remark,
                    IsSystem = 0,
                    SavingCost = flowResponse.Flow.SavingCost,
                    SavingHour = flowResponse.Flow.SavingHour,
                    
                });

                Logger.LogInformation($"GetFlowById <<22>>resultFlow.FlowId:{resultFlow.Id}");

                var resultAttachment = await cmdAttachmentClient.CreateAttachmentAsync(new Laiye.EntCmd.Service.Core.CreateAttachmentRequest
                {
                    CompanyId = companyId,
                    ContentType = attachmentResponse.Attachment.ContentType,
                    FileName = attachmentResponse.Attachment.FileName,
                    Path = attachmentResponse.Attachment.Path,
                    Type = attachmentResponse.Attachment.Type,
                    
                });
                Logger.LogInformation($"GetFlowById <<23>>resultAttachment.resultAttachmentId:{resultAttachment.Id}");

                var resultFlowVersion = await flowClient.CreateFlowVersionAsync(new Laiye.EntCmd.Service.Core.CreateFlowVersionRequest
                {
                    CompanyId = companyId,
                    CreateEmployeeId = employeeResponse.Employee.Id,
                    FlowId = resultFlow.Id,// flowId,
                    Version = "1.0",
                    Description = "市场订阅",
                    FlowAttachmentId = resultAttachment.Id,
                    FileName = attachmentResponse.Attachment.FileName,
                    InstructionsAttachmentId = flowVersionSourceResponse.FlowVersion.InstructionsAttachmentId,
                    
                });
                Logger.LogInformation($"GetFlowById <<24>>resultFlowVersion.resultFlowVersionId:{resultFlowVersion.Id}");

                await flowClient.UpdateFlowVersionStateAsync(new UpdateFlowVersionStateRequest
                {
                    CompanyId = companyId,
                    FlowVersionId = resultFlowVersion.Id,
                    UpdateEmployeeId = employeeResponse.Employee.Id,
                    IsActive = 1,
                });

                await doWriteFlowRelation(resultFlow.Id, flowId, conn);
                Logger.LogInformation($"GetFlowById <<25>>end");
                //added by wenz 加入流程共享

                //originCompanyId = Convert.ToInt64(array[0].Substring(1));
                //flowId = long.Parse(array[1]);
               
                await publisher.PublishAsync("zsy.flow.updated", new CommonEventArgs { Id= flowId,CompanyId=originCompanyId });
                Logger.LogInformation($"GetFlowById <<2 6>>Publish end");

                return GetReturnSignature(ACCESS_KEY, new BaseResponse<ResponeSuccess2>(new ResponeSuccess2 { saasUrl = saasurl, loginUrl = loginurl }), loginurl, loginurl);

            }

        }

        [HttpGet("getFlowRelationById")]
        public async Task<long> GetFlowRelationById(long flowId, [FromServices] IFreeSql conn)
        {
            Logger.LogInformation($"GetFlowRelationById <<1>> begin flowId:{flowId}");
            
            var flowRelationModel = await conn.Select<FlowRelationModel>()
                                            .Where(a => a.Flow_id == flowId).FirstAsync();

            if (flowRelationModel != null )
            {
                Logger.LogInformation($"GetFlowRelationById <<2>>: end Origin_flow_id:{flowRelationModel.Origin_flow_id}");
                return flowRelationModel.Origin_flow_id;
            }

            return 0;
        }


        private System.Threading.Tasks.Task doWriteFlowRelation(long flow_id, long origin_flow_id, IFreeSql conn)
        {
            Logger.LogInformation($"doWriteFlowRelation(<<1>>:begin:flowId:{flow_id},origin_flow_id:{origin_flow_id}");
            conn.Insert(new FlowRelationModel
            {
                Flow_id = flow_id,
                Origin_flow_id = origin_flow_id,
                Create_Time = DateTime.UtcNow
            }).ExecuteAffrows();
            Logger.LogInformation($"doWriteFlowRelation(<<2>>:end");
            return System.Threading.Tasks.Task.CompletedTask;
        }





        //[CapSubscribe("szj.createSub")]
        //public async Task ProcessSub(EventArgs args)
        //{
        //    //grpc 是否要创建租户

        //    //using (var hacm = HMACSHA256.Create())
        //    //{
        //    //     //hacm.ComputeHash()
        //    //}


        //    //using (var aes = Aes.Create())
        //    //{
        //    //    aes.Mode = CipherMode.CBC;
        //    //    Convert.FromBase64String()
        //    //}

        //    //Random.Next(,)

        //    //JsonConvert.SerializeObject(new { });

        //    //JsonConvert.DeserializeObject<TestModel>("");

        //    // 生成邀请链接

        //    // 更新数据库的订阅记录状态为已完成
        //}


    }
}