using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;


    public class SqlHelper
    {
        //private SqlConnection conn;
        /// <summary>
        /// 返回数据库连接对象（状态：Close）
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetCon()
        {
            string strConn = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            //string strAppSetting = ConfigurationManager.AppSettings["AppName"];
            return new SqlConnection(strConn);
        }

        /// <summary>
        /// 执行变更操作（INSERT、UPDATE或INSERT语句）
        /// </summary>
        /// <param catID="sql">SQL语句</param>
        /// <param catID="sqlParams">SQL语句的参数集合</param>
        /// <returns>执行成功则返回执行命令所影响的行数，失败则返回0，类型为int。</returns>
        public static int ExecuteNonQuery(String sqlStr, params SqlParameter[] sqlParams)
        {
            SqlConnection con = GetCon();
            SqlCommand cmd = new SqlCommand(sqlStr, con);
            if (sqlParams != null)
                foreach (SqlParameter parm in sqlParams)
                    cmd.Parameters.Add(parm);
            // 启动并添加事务（添加之前必须打开数据库连接）
            con.Open();
            SqlTransaction myTrans = con.BeginTransaction();
            cmd.Transaction = myTrans;
            int ok = 0;
            try
            {
                ok = cmd.ExecuteNonQuery();
                // 提交事务
                myTrans.Commit();
                return ok;
            }
            catch (Exception err)
            {
                // 发生异常情况，则回滚事务
                myTrans.Rollback();
                Console.WriteLine(err.Message);
                return 0;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// 执行SELECT查询操作，返回DataReader集合
        /// </summary>
        /// <param catID="sql">SQL语句</param>
        /// <param catID="cmdParams">SQL语句的参数集合</param>
        /// <returns>返回DataReader集合</returns>
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] sqlParams)
        {
            SqlConnection con = GetCon();
            SqlCommand cmd = new SqlCommand(sql, con);
            if (sqlParams != null)
            {
                foreach (SqlParameter parm in sqlParams)
                    cmd.Parameters.Add(parm);
            }
            try
            {
                con.Open();
                //CommandBehavior.CloseConnection：
                //当SqlDataReader对象被关闭时,其依赖的连接也会被自动关闭。
                SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return sdr;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 执行SELECT（函数）查询操作，返回单值（首行首列的值）
        /// </summary>
        /// <param catID="sql">SQL语句</param>
        /// <param catID="cmdParams">SQL语句的参数集合</param>
        /// <returns>返回函数查询结果（如求和、统计等），类型为object对象。</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] sqlParams)
        {
            SqlConnection con = GetCon();
            SqlCommand cmd = new SqlCommand(sql, con);
            if (sqlParams != null)
            {
                foreach (SqlParameter parm in sqlParams)
                    cmd.Parameters.Add(parm);
            }
            try
            {
                con.Open();
                object val = cmd.ExecuteScalar();
                return val;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// 执行SELECT查询操作，返回DataTable对象
        /// </summary>
        /// <param catID="sql">SQL语句</param>
        /// <param catID="cmdParams">执行SQL语句所用参数的集合，无值则设置为null</param>
        /// <returns>返回DataTable对象</returns>
        public static DataTable ExecuteQuery(String sql, params SqlParameter[] cmdParms)
        {
            SqlConnection con = GetCon();
            SqlCommand cmd = new SqlCommand(sql, con);
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            try
            {
                sda.Fill(dt);
                return dt;
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
                return null;
            }
            finally
            {
                //sda.Dispose();
                //cmd.Dispose();
                if (con.State != ConnectionState.Closed)
                    con.Close();
            }

        }

    }
