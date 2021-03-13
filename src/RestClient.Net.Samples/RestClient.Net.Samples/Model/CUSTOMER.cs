using System.Collections.Generic;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ThomasBayer
{

    public class CUSTOMERList : List<CUSTOMER>
    {

    }

    public class CUSTOMER
    {
        public int ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string STREET { get; set; }
        public string CITY { get; set; }
    }
}
