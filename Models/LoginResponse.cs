using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProxy.Models
{
    public class Result
    {
        [JsonProperty("user")]
        public User user;

        [JsonProperty("token")]
        public string token;
    }

    public class LoginResponse
    {
        [JsonProperty("result")]
        public Result result;

        [JsonProperty("isSuccess")]
        public bool isSuccess;

        [JsonProperty("message")]
        public object message;
    }

    public class User
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("email")]
        public string email;

        [JsonProperty("name")]
        public string name;

        [JsonProperty("phoneNumber")]
        public string phoneNumber;
    }
}
