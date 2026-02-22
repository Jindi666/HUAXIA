using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto.Digests;
using System.Web;
using System.IO;
using javax.crypto;
using java.security;
using javax.crypto.spec;

namespace Laiye.Customer.WebApi.Utils
{
    




    //安全工具类,签名+加解密
    public class SecurityUtils
    {
        //加密算法名称
        private static string HMAC_SHA_256 = "HMACSHA256";//
        private static string AUTH_TOKEN = "authToken";
        private static string JOIN_SIGN = "&";
        private static string EQUAL_SIGN = "=";
        private static string CHARSET = "UTF-8";
        private static int ENCRYPT_TYPE_128 = 128;
        /**
         * 租户编码+项目编码，需要根据saas服务方具体情况赋值情况说明
         */
        private static string ACCESS_KEY = "CMHK-GRD-PAAS-PORTAL";

        //演示代码test2
        //public static void Main(string[] args) //throws Exception
        //{
        //    //string str = SecurityUtils.getRandomChars(10, true, true, true, true);
        //    //Console.WriteLine(str);

        //    //string[] strs = new string[] { "wangpeiqun", "hello" };
        //    //Console.WriteLine( ComputeSign(strs));
        //    //Console.ReadKey();
        //    //423c9ee3ccfd53b31e1b65b2167106066b4ca2c7e1ee14a259f81fa2cfba7784

        //    //应用市场方发送authToken参数
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    dic.Add("spec", "1.0.0 | small");
        //    dic.Add(AUTH_TOKEN, "test");
        //    dic.Add("userName", "zhangsan");
        //    string authToken = getSignature(getRequestParamsForSign(dic));
        //    Console.WriteLine(authToken);

        //    //应用市场方增加authToken参数至header中
        //    string authTokenHeader = "";
        //    bool checkResult = verificateRequest(dic, authTokenHeader);
        //    Console.WriteLine("saas服务方验证结果:{0}", checkResult);



        //    Console.ReadKey();
        //}

        public static void Main(string[] args)
        {
            string s1 = HttpUtility.UrlEncode("3|1", Encoding.UTF8);
            Console.WriteLine(s1);

        }



        public static string getSignature(string accessKey, string macData)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(accessKey)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(macData));
                signRet = Convert.ToBase64String(hash);
            }
            return signRet;
        }



        /***
        * 应用市场验签
        **/
        public static bool verifySignatrue(string genSignature, string signatureHeader)
        {
            //string signatureHeader = response.getHeader("signature");
            return signatureHeader.Equals(genSignature);
        }



        /***
         * @Description saas服务商校验
         * @Param Dictionary<string, string> dic, String authTokenHeader
         * @return bool 
         **/
        public static bool verificateRequest(Dictionary<string, string> dic, string authTokenHeader)
        {
            var str = getRequestParamsForSign(dic);
            //string signatureHeader = response.getHeader("authToken");
            string signature = getSignature(ACCESS_KEY, str);
            return authTokenHeader.Equals(signature);
        }

        /***
         * 组装待签名的报文体
         * @Param Dictionary<string, string> dic
         * @return string
         */
        public static String getRequestParamsForSign(Dictionary<string, string> dic)
        {
            //Console.WriteLine(dic.ContainsKey(AUTH_TOKEN));
            //原报文去除TOKEN
            dic.Remove(AUTH_TOKEN);
            //Console.WriteLine(dic.ContainsKey(AUTH_TOKEN));
            Dictionary<String, String> sortedDic = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);//排序升序
            Dictionary<string, string>.KeyCollection keyCol = sortedDic.Keys;
            string str = "";
            string value = "";
            foreach (string key in keyCol)
            {
                value = dic[key];
                //Console.WriteLine("value = {0}", value);
                str = str + JOIN_SIGN + key + EQUAL_SIGN + value;
                //Console.WriteLine(str);
            }
            str = str.Substring(1);
            //Console.WriteLine(str);
            //Console.ReadKey();
            return str;
        }

        /***
         * 生成报文签名,请求及返回时使用
         * @Param args
         * @return string
         */
        /*
        public static string getSignature(params string[] args)
        {
            var Key = Encoding.UTF8.GetBytes("KvssrH7FZZaEGGBYwJlmIt3Cg3ugrWmMBoHV");
            var strToSign = string.Join(",", args);
            using (var hmac = new HMACSHA256(Key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(strToSign));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        */



        /***
        * 随机+AES 加密
        * @Param [isvBody, decryptAccessKey, sEncryptType]
        * @return String
        */
        static string dataEncryption(string isvBody, string accessKey) //throws Exception 
        {
            //16位随机
            //String iv = getRandomChars(16);
            //String isvEncryptBody = encryptEncode(isvBody, accessKey, iv, ENCRYPT_TYPE_128);
            //return iv + isvEncryptBody;
            return null;
        }

        /***
         * AES/CBC数据加密
         * @Param [content, key, iv, encryptType]
         * return string
         */
        static string encryptEncode(string content, string key, string iv, int encryptType)
        {
            return null;
        }

        /// AES/CBC模式 加密算法test
        /// <param name="plainText">加密内容</param>
        /// <param name="key">密钥</param>
        /// <param name="IV">密钥</param>
        /// <returns>string：密文</returns>
        public static string encryptAESCBC(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted = Encoding.UTF8.GetBytes(plainText);
            string retStr = "";
            Console.WriteLine("AESCBC encrypted.length:{0}", encrypted.Length);
            // Create an Aes object
            // with the specified key and IV.
            Aes aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            // Create an encryptor to perform the stream transform.
            System.Security.Cryptography.ICryptoTransform encryptor = aes.CreateEncryptor();

            byte[] resultArray = encryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            retStr = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            Console.WriteLine("AESCBC jiami:{0}", retStr);
            return retStr;
            
        }

        /***
         * 数据解密
         *  @Param [encryptStr,accessKey]
         *  @return String
         */
        public static string dataDecryption(string encryptStr, string accessKey)
        {
            string iv = encryptStr.Substring(0, 16);
            string decryptBody = decryptAESCBCEncode(encryptStr.Substring(16), accessKey, iv, ENCRYPT_TYPE_128);
            return decryptBody;
        }

        /***
         * 解密
         * @Param [content, key, iv, encryptType]
         * @return String
         */
        private static string decryptAESCBCEncode(string content, string key, string iv, int encryptType)
        {
            byte[] para1 = Convert.FromBase64String(content);
            byte[] para2 = Encoding.Default.GetBytes(key);
            byte[] para3 = Encoding.Default.GetBytes(iv);

            byte[] res = decryptAESCBC(para1, para2, para3, encryptType);
            string str = Encoding.UTF8.GetString(res);
            return str;
            // return new String(decryptAESCBC( Base64.decodeBase64(content.getBytes()),key.getBytes(), iv.getBytes(), encryptType)                         );

        }

        /// AES/CBC模式 解密算法
        /// <param name="cipherText">加密内容</param>
        /// <param name="key">密钥</param>
        /// <param name="IV">密钥</param>
        /// <returns>string：密文</returns>
        private static byte[] decryptAESCBC(byte[] content, byte[] keyBytes, byte[] iv, int encryptType)
        {
            //byte[] result = new byte[0];

            KeyGenerator keyGenerator = KeyGenerator.getInstance("AES");
            java.security.SecureRandom secureRandom = java.security.SecureRandom.getInstance("SHA1PRNG");
            secureRandom.setSeed(keyBytes);
            keyGenerator.init(encryptType, secureRandom);
            SecretKey key = keyGenerator.generateKey();
            Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
            cipher.init(Cipher.DECRYPT_MODE, key, new IvParameterSpec(iv));

            byte[] result = cipher.doFinal(content);

            return result;
        }

        ///加密
        public static string EncryptString(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted = Encoding.UTF8.GetBytes(plainText);
            string retStr = "";
            Console.WriteLine("encrypted.length:{0}", encrypted.Length);
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor();//rijAlg.Key, rijAlg.IV

                byte[] resultArray = encryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                retStr = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                //Console.WriteLine("jiami:{0}",retStr);
                return retStr;
            }

        }

        ///解密
        static string DecryptString(string cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Create an RijndaelManaged object
            // with the specified key and IV.
            byte[] byCon = Convert.FromBase64String(cipherText);
            string decryptStr = "";
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                byte[] resultArray = decryptor.TransformFinalBlock(byCon, 0, byCon.Length);
                decryptStr = Encoding.UTF8.GetString(resultArray);
                return decryptStr;
            }

        }

        /***
         * 获取指定长度的随机字符串
         *<param name="intLength">随机字符串长度</param>
         *<param name="booNumber">生成的字符串中是否包含数字</param>
         *<param name="booSign">生成的字符串中是否包含符号</param>
         *<param name="booSmallword">生成的字符串中是否包含小写字母</param>
         *<param name="booBigword">生成的字符串中是否包含大写字母</param>
         *<returns></returns>
         */
        public static string getRandomChars(int intLength, bool booNumber, bool booSign,
            bool booSmallword, bool booBigword)
        {
            Random ranA = new Random();
            int intResultRound = 0;
            int intA = 0;
            string strB = "";
            while (intResultRound < intLength)
            {
                //生成随机数A，1=数字，2=符号，3=小写字母，4=大写字母
                intA = ranA.Next(1, 5);

                //如果随机数A=1，则运行生成数字  
                //生成随机数A，范围在0-10  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  

                if (intA == 1 && booNumber)
                {
                    intA = ranA.Next(0, 10);
                    strB = intA.ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果随机数A=2，则运行生成符号  
                //生成随机数A，表示生成值域  
                //1：33-47值域，2：58-64值域，3：91-96值域，4：123-126值域  

                if (intA == 2 && booSign == true)
                {
                    intA = ranA.Next(1, 5);

                    //如果A=1  
                    //生成随机数A，33-47的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  

                    if (intA == 1)
                    {
                        intA = ranA.Next(33, 48);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=2  
                    //生成随机数A，58-64的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  

                    if (intA == 2)
                    {
                        intA = ranA.Next(58, 65);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=3  
                    //生成随机数A，91-96的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  

                    if (intA == 3)
                    {
                        intA = ranA.Next(91, 97);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //如果A=4  
                    //生成随机数A，123-126的Ascii码  
                    //把随机数A，转成字符  
                    //生成完，位数+1，字符串累加，结束本次循环  

                    if (intA == 4)
                    {
                        intA = ranA.Next(123, 127);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                }

                //如果随机数A=3，则运行生成小写字母  
                //生成随机数A，范围在97-122  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  

                if (intA == 3 && booSmallword == true)
                {
                    intA = ranA.Next(97, 123);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果随机数A=4，则运行生成大写字母  
                //生成随机数A，范围在65-90  
                //把随机数A，转成字符  
                //生成完，位数+1，字符串累加，结束本次循环  

                if (intA == 4 && booBigword == true)
                {
                    intA = ranA.Next(65, 89);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }
            }
            return strB;
        }

    }
}
