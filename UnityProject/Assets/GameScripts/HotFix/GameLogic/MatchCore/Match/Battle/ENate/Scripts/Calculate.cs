/*
 * @Description: 
 * @Author: yangzijian
 * @Date: 2020-04-17 17:47:53
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-04-17 17:52:04
 */
using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;
using System.Text;

namespace ExpressionEnate
{
    public class Calculate
    {
        static DataTable m_table = new DataTable();

        public static object Compute(string expression)
        {
            return m_table.Compute(expression, "");
        }
    }
}