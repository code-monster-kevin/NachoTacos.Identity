using NachoTacos.Identity.Model.Abstracts;
using System;

namespace NachoTacos.Identity.Model.Entities
{
    public class LoginAudit : Entity
    {
        public string Application { get; set; }
        public string Email { get; set; }
        public int LoginStatus { get; set; }
        public DateTime CreatedDate { get; set; }

        public static LoginAudit Create(string application, string email, int loginStatus)
        {
            return new LoginAudit
            {
                Id = Guid.NewGuid(),
                Application = application,
                Email = email,
                LoginStatus = loginStatus,
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}
