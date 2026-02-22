using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    /// <summary>
    /// 订阅信息
    /// </summary>
    [Table(Name = "tbl_subscription")]
    public class SubscriptionModel
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get;}

        ///// <summary>
        ///// 系统生成的订单号
        ///// </summary>
        //[Column(Name = "orderNo")]
        //public string OrderNo { get; set; }
        /// <summary>
        /// 招商云传来的订单号
        /// </summary>
        [Column(Name = "orderNo_zsy")]
        public string? OrderNo_zsy { get; set; }
        /// <summary>
        /// 异步标记,如果是true则必须提供callback接口，应用市场会定时调度
        /// </summary>
        [Column(Name = "async")]
        public string? Async { get; set; }
        /// <summary>
        /// 规格key：多规格使用“|”连接
        /// </summary>
        [Column(Name = "spec")]
        public string? Spec { get; set; }
        /// <summary>
        /// 规格名字
        /// </summary>
        [Column(Name = "specNames")]
        public string? SpecNames { get; set; }
        /// <summary>
        /// 规格值
        /// </summary>
        [Column(Name = "specValues")]
        public string? SpecValues { get; set; }
        /// <summary>
        /// 计费方式：period 周期，buyOut 一次性
        /// </summary>
        [Column(Name = "chargeType")]
        public string? ChargeType { get; set; }
        /// <summary>
        /// 订单类型1:command 2:worker 3:creator
        /// </summary>
        [Column(Name = "ordertype")]
        public int Ordertype { get; set; }

        [Column(Name = "count")]
        public long Count { get; set; }
        /// <summary>
        /// 产品编码
        /// </summary>
        [Column(Name = "productNo_zsy")]
        public string? ProductNo_zsy { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [Column(Name = "versionNo")]
        public string? VersionNo { get; set; }
        /// <summary>
        /// 渠道编码，表明请求来源appStore
        /// </summary>
        [Column(Name = "channel")]
        public string? Channel { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        [Column(Name = "companyid")]
        public long Companyid { get; set; }
        /// <summary>
        /// 招商云传来的项目ID
        /// </summary>
        [Column(Name = "projectId_zsy")]
        public string? ProjectId_zsy { get; set; }
        /// <summary>
        ///1:资源项目；2：开发项目；默认为1
        /// </summary>
        [Column(Name = "projectType_zsy")]
        public string? ProjectType_zsy { get; set; }
        /// <summary>
        /// 域账号（加密）
        /// </summary>
        [Column(Name = "userName_zsy")]
        public string? UserName_zsy { get; set; }
        /// <summary>
        /// 请求时间戳
        /// </summary>
        [Column(Name = "requestTime_zsy")]
        public DateTime RequestTime_zsy { get; set; }

        [Column(Name = "createTime")]
        public DateTime CreateTime { get; set; }

        [Column(Name = "updateTime")]
        public DateTime UpdateTime { get; set; }

        [Column(Name = "status")]
        public int Status { get; set; }

        [Column(Name = "ls_begintime")]
        public DateTime Ls_begintime { get; set; }

        [Column(Name = "ls_endtime")]
        public DateTime Ls_endtime { get; set; }

        [Column(Name = "memo")]
        public string? Memo { get; set; }

    }

    public class GetSubscriptionListModel
    {

        /// <summary>
        /// 租户ID
        /// </summary>
        public long[]? companyid { set; get; } 
        /// <summary>
        /// 授权类型
        /// </summary>
        public string[]? productNo_zsy { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; } = 1;
        /// <summary>
        /// 每页数量
        /// </summary>
        public int pageSize { get; set; } = 20;
    }

    public class ResponseSubscriptionListModel
    {
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string? userName { get; set; }
        /// <summary>
        /// 招商云订单号
        /// </summary>
        public string? orderNo_zsy { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>
        public string? projectId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? projectName { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public long companyid { set; get; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string? companyName { get; set; }
        /// <summary>
        /// 授权类型
        /// </summary>
        public string? productNo_zsy { set; get; }
        /// <summary>
        /// 授权分配数量
        /// </summary>
        public long count { get; set; }
        /// <summary>
        /// 授权有效期
        /// </summary>
        public DateTime ls_endtime { set; get; }
        
    }

    public class ResponseSubscriptionList
    {
        public long total { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public List<ResponseSubscriptionListModel> data { get; set; }

    }
}
