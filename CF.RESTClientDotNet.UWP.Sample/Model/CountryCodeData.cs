using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groupkt
{
    public class CountriesResult
    {
        public string name { get; set; }
        public string alpha2_code { get; set; }
        public string alpha3_code { get; set; }
    }

    public class IPInfoResult
    {
        public string countryIso2 { get; set; }
        public string stateAbbr { get; set; }
        public string postal { get; set; }
        public string continent { get; set; }
        public string state { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ds { get; set; }
        public string network { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string ip { get; set; }
    }

    public class RestResponse<T>
    {
        public List<string> messages { get; set; }
        public List<T> result { get; set; }
    }

    public class groupktResult<T>
    {
        public RestResponse<T> RestResponse { get; set; }
    }
}
