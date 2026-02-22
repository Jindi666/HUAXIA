using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    [Table(Name = "tbl_relationship")]
    public class RelationshipModel
    {
        [Column(Name = "companyId", IsPrimary = true)]
        public long CompanyId { get; set; }

        [Column(Name = "tenantId_zsy")]
        public string TenantId_zsy { get; set; }

        [Column(Name="name")]
        public string Name { get; set; }

        [Column(Name = "projectid")]
        public string Projectid { get; set; }

        [Column(Name = "projectname")]
        public string Projectname { get; set; }

        [Column(Name = "createTime")]
        public DateTime CreateTime { get; set; }

    }
}
