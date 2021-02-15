using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fated.Application.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("用户名"), Required(ErrorMessage = "请输入用户名")]
        public string UserName { get; set; }
        [DisplayName("密码"), Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }
        [DisplayName("记住我")]
        public bool RememberMe { get; set; }
    }
}
