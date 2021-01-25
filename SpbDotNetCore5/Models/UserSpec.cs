using System;
using System.Collections.Generic;

namespace SpbDotNetCore5.Models
{
    public class UserSpec
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public DateTime Birthday { get; set; }
        public List<String> PhoneNumbers { get; set; }
    }
}