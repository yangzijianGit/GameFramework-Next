using System;


namespace jc
{
    //时间表示形式
    public enum TimeType
    {
        TT_ALL_FORMAT,      //0000-00-00 00:00:00
        TT_ALL_COMPACT,     //00000000000000
        TT_DATE_FORMAT,     //0000-00-00
        TT_DATE_COMPACT,    //00000000
        TT_TIME_FORMAT,     //00:00:00
        TT_TIME_COMPACT,    //000000
    }


    //时间工具类（无实例）
    public sealed class TimerUtils
    {
        /** 属性变量 **/
        //起始时间
        private static DateTime m_objStartTime = new DateTime(1970, 1, 1, 8, 0, 0);


        /** 构造函数 **/
        private TimerUtils() {}


        /** 公有函数 **/
        /*  
         * 描  述：获取当前时间(静态)
         * 参  数：表现形式
         * 返回值：时间字符串
         */
        public static string GetCurTime(TimeType tt)
        {
            string strTime = string.Empty;

            switch (tt) {
                case TimeType.TT_ALL_FORMAT: {
                    strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                }
                case TimeType.TT_ALL_COMPACT: {
                    strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    break;
                }
                case TimeType.TT_DATE_FORMAT: {
                    strTime = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                }
                case TimeType.TT_DATE_COMPACT: {
                    strTime = DateTime.Now.ToString("yyyyMMdd");
                    break;
                }
                case TimeType.TT_TIME_FORMAT: {
                    strTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                }
                case TimeType.TT_TIME_COMPACT: {
                    strTime = DateTime.Now.ToString("HHmmss");
                    break;
                }
            }

            return strTime;
        }

        /*  
         * 描  述：转换时间(静态)
         * 参  数：世界时间形式(单位：ms)
         * 返回值：时间串
         */
        public static string ConvertTimeMS(long t)
        {
            return TimerUtils.m_objStartTime.AddMilliseconds(t).ToString();
        }
        
        /// <summary>
        /// 时间格式转换
        /// </summary>
        /// <param name="ms">毫秒</param>
        /// <returns></returns>
        public static string ConvertTime(long ms)
        {
            string strTime = string.Empty;

            long day = (long)Math.Floor(ms / 86400000.0f);
            ms = ms - day * 86400000;
            long hour = (long)Math.Floor(ms / 3600000.0f);
            ms = ms - hour * 3600000;
            long minute = (long)Math.Floor(ms / 60000.0f);
            ms = ms - minute * 60000;
            long second = (long)Math.Ceiling(ms / 1000.0f);

            // if (day > 0) {
            //     if (0 == hour && minute > 0) {
            //         hour = hour + 1;
            //     }
            //     if (hour > 0) {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_3",new string[] { day.ToString(), hour.ToString() });
            //     }
            //     else
            //     {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_8", new string[] { day.ToString() });
            //     }
            // }
            // else if (hour > 0) {
            //     if (0 == minute && second > 0) {
            //         minute = minute + 1;
            //     }
            //     if (minute > 0)
            //     {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_2", new string[] { hour.ToString(), minute.ToString() });
            //     }
            //     else
            //     {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_7", new string[] { hour.ToString() });
            //     }
            // }
            // else if (minute > 0) {
            //     if (second > 0) {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_1", new string[] { minute.ToString(), second.ToString() });
            //     }
            //     else {
            //         strTime = LanguageManager.Instance.GetLanguage("Time_6", new string[] { minute.ToString()});
            //     }
            // }
            // else if (second > 0)
            // {
            //     strTime = LanguageManager.Instance.GetLanguage("Time_5", new string[] { second.ToString() });
            // }
            // else
            // {
            //     strTime = LanguageManager.Instance.GetLanguage("Time_5", new string[] { "0" });
            // }

            return strTime;
        }

        /*  
         * 描  述：获取时间戳(静态)
         * 参  数：世界时间形式(单位：ms)
         * 返回值：时间串
         */
        public static long GetTimeStamp()
        {
            DateTime dateTime = DateTime.Now;
            
            return Convert.ToInt64(dateTime.Subtract(TimerUtils.m_objStartTime).TotalMilliseconds);
        }
    }
}