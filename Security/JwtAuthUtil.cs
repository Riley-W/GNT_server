﻿using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GNT_server.Security
{
    public class JwtAuthUtil
    {
        public string GenerateToken(string name)
        {
            string secret = "GoodNightTainan";//加解密的key,如果不一樣會無法成功解密
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            ExpiredTime.ETime = (DateTime.Now.AddDays(1)-startTime).TotalMilliseconds.ToString();
            Dictionary<string, Object> claim = new Dictionary<string, Object>();//payload 需透過token傳遞的資料
            claim.Add("Account", name);
            claim.Add("Company", "GoodNightTainan.com");
            claim.Add("Exp", ExpiredTime.ETime);//Token 時效設定1天
            var payload = claim;
            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);//產生token
            return token;
        }
    }
}