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
        public string GenerateToken()
        {
            string secret = "GoodNightTainanORoKaMoNo";//加解密的key,如果不一樣會無法成功解密
            Dictionary<string, Object> claim = new Dictionary<string, Object>();//payload 需透過token傳遞的資料
            claim.Add("Account", "Admin");
            claim.Add("Company", "GoodNightTainan.com");
            claim.Add("Exp", DateTime.Now.AddDays(Convert.ToInt32("1")).ToString());//Token 時效設定100秒
            var payload = claim;
            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);//產生token
            return token;
        }
    }
}