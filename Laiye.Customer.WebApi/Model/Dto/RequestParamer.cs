using FreeSql.DataAnnotations;
using java.sql;


namespace Laiye.Customer.WebApi.Model.Dto
{
    public class RequestParamer
    {
        public string company_id { get; set; }
    }

    //  登录用户名与密码
    public class LoginParamer
    {
        public string userLoginId { get; set; }

        public string password { get; set; }
    }

    // 修改密码
    public class PasswordParamer
    {
        public string userLoginId { get; set; }

        public string password { get; set; }                  // 新密码

        public string oldPassword { get; set; }               // 旧密码
    }

    //  分页参数对象
    public class PageParamer
    {
        public int pageSize { get; set; }

        public int pageIndex { get; set; }
    }


    //  登录用户名与密码
    [Table(Name = "HUAXIA.user_login")]  // 达梦数据库需要包含Schema前缀
    public class UserLogin
    {
        [Column(Name = "user_login_id", IsPrimary = true, IsIdentity = true)]
        public string userLoginId { get; set; }

        [Column(Name = "current_password")]
        public string currentPassword { get; set; }
       
    }

}
