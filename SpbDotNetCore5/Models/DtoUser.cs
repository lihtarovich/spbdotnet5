using System;
using System.Collections.Generic;

namespace SpbDotNetCore5.Models
{
    public class DtoUser
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public DateTime Birthday { get; set; }
        public List<DtoPhoneNumber> PhoneNumbers { get; set; }
    }
}