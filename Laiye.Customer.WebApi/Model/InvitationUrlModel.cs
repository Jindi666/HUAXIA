using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    [Table(Name = "tbl_invitationUrl")]
    public class InvitationUrlModel
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; }
        /// <summary>
        /// 租户ID
        /// </summary>
        [Column(Name = "companyid")]
        public long Companyid { get; set; }
        /// <summary>
        /// 招商云传来的项目ID
        /// </summary>
        [Column(Name = "projectId_zsy")]
        public string ProjectId_zsy { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Name = "userName_zsy")]
        public string UserName_zsy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "createTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column(Name = "memo")]
        public string Memo { get; set; }

    }
}
