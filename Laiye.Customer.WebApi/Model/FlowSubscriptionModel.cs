using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    /// <summary>
    /// 流程订阅
    /// </summary>
    [Table(Name = "tbl_flow_subscription")]
    public class FlowSubscriptionModel
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; }

        /// <summary>
        /// 招商云传来的文件名
        /// </summary>
        [Column(Name = "fileName_zsy")]
        public string FileName_zsy { get; set; }
        /// <summary>
        /// 异步标记,如果是true则必须提供callback接口，应用市场会定时调度
        /// </summary>
        [Column(Name = "async")]
        public string Async { get; set; }
        
        /// <summary>
        /// 产品编码哈希值
        /// </summary>
        [Column(Name = "productNo_zsy")]
        public string ProductNo_zsy { get; set; }
        /// <summary>
        /// 产品编码哈希值
        /// </summary>
        [Column(Name = "httpUrl")]
        public string HttpUrl { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [Column(Name = "versionNo")]
        public string VersionNo { get; set; }
        /// <summary>
        /// 渠道编码，表明请求来源appStore
        /// </summary>
        [Column(Name = "channel")]
        public string Channel { get; set; }
        /// <summary>
        /// 招商云传来的项目ID
        /// </summary>
        [Column(Name = "projectId_zsy")]
        public string ProjectId_zsy { get; set; }

        [Column(Name = "createTime")]
        public DateTime CreateTime { get; set; }

        [Column(Name = "updateTime")]
        public DateTime UpdateTime { get; set; }

        [Column(Name = "memo")]
        public string Memo { get; set; }
    }
}
