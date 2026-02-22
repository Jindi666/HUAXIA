using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    /// <summary>
    /// 流程关系映射
    /// </summary>
    [Table(Name = "tbl_flow_relation")]
    public class FlowRelationModel
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; }

        /// <summary>
        /// 系统生成的流程ID
        /// </summary>
        [Column(Name = "flow_id")]
        public long Flow_id { get; set; }

        /// <summary>
        /// 传入的流程ID
        /// </summary>
        [Column(Name = "origin_flow_id")]
        public long Origin_flow_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime Create_Time { get; set; }
    }
}
