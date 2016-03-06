using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Common.Json
{
    [Serializable]
    public class JsonResponse
    {
        [NonSerialized]
        public static readonly string OperationSuccess = "success";

        [NonSerialized]
        public static readonly string OperationFailure = "error";

        public string Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}
