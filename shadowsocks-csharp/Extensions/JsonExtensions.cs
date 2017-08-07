using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.Extensions
{
    public static class JsonExtensions
    {
        public static bool ok(this JToken token)
        {
            if (token == null) return false;
            var ok = token["ok"];
            return ok != null && ok.Type == JTokenType.Boolean && (bool)ok;
        }
    }
}
