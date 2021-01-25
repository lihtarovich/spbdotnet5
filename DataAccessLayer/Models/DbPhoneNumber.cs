using System;

namespace DataAccessLayer
{
    public class DbPhoneNumber
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DbUser User { get; set; }
        public String PhoneNumber { get; set; }
    }
}