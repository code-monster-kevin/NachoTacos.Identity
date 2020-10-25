using System;

namespace NachoTacos.Identity.Admin.Service.DTO
{
    public class AppUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LastLoginAttempt { get; set; }
        public string LoginStatus { get; set; }

        public static AppUser Create(string id, string email, int accessFailedCount, DateTime? lastLoginAttempt, string loginStatus)
        {
            return new AppUser
            {
                Id = id,
                Email = email,
                AccessFailedCount = accessFailedCount,
                LastLoginAttempt = lastLoginAttempt,
                LoginStatus = loginStatus
            };
        }
    }
}
