using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public class DbUser
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public DateTime Birthday { get; set; }
        public List<DbPhoneNumber> PhoneNumbers { get; set; }
    }
}