using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GrafanaApiClient
{
    public class MessageModel<T>
    {
        [DataMember(Name ="message")]
        public string Message { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }
    }
}
