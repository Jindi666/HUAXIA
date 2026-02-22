using FreeSql.DataAnnotations;

namespace Laiye.Customer.WebApi.Model
{
    [Table(Name = "tbl_test")]
    public class TestModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column(Name = "id", IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
    }
}
