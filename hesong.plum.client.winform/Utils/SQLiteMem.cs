//Copyright (C) Innocall Corporation
//All rights reserved.
//author:       wulei
//version:      1.0
//create time:  2010-10-25
//remark:       操作SQLite内存表
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;


namespace hesong.plum.client.Utils
{
    /// <summary>
    /// 用于SQLite内存表的DBOperator，从DBOperator继承，并实现IDBOperator接口
    /// </summary>
    public class SQLiteMem
    {
        /// <summary>
        /// 静态引用的实例，这样就可以全局使用一个内存数据库实例
        /// </summary>
        public static SQLiteMem InstanceSqLiteMem { get; set; }
        private ISaveLog _logSaver;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logSaver"></param>
        public SQLiteMem(ISaveLog logSaver)
        {
            _logSaver = logSaver;
            InitConnection();
        }

        private void Log(string logType, string logMsg)
        {
            _logSaver?.Log(logType, logMsg);
        }

        private void Log(string logMsg)
        {
            _logSaver?.Log(logMsg);
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString = "Data Source=:memory:";
        /// <summary>
        /// SQLiteConnection连接实例
        /// </summary>
        public SQLiteConnection conn = null;
        /// <summary>
        /// 连接超时时间（秒）
        /// </summary>
        private int _DefaultTimeout = 30;

        /// <summary>
        /// 连接超时时间（秒）
        /// </summary>
        public int DefaultTimeout
        {
            get { return conn.ConnectionTimeout; }
            set { _DefaultTimeout = value; }
        }

        /// <summary>
        /// 指示执行命令期间在终止尝试和产生错误之前需等待的时间。
        /// </summary>
        private int _CommandTimeout = 60;

        /// <summary>
        /// 指示执行命令期间在终止尝试和产生错误之前需等待的时间。
        /// </summary>
        public int CommandTimeout
        {
            get { return _CommandTimeout; }
            set { _CommandTimeout = value; }
        }

        /// <summary>
        /// 初始化SQLiteConnection
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection InitConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection();
                conn.ConnectionString = ConnectionString;
                conn.Open();
                Log("SQLite内存数据库被创建");
            }
            if(conn.State==ConnectionState.Closed)
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                Log("SQLite内存数据库被打开");
            }
            return conn;
        }
        /// <summary>
        /// 关闭SQLiteConnection
        /// </summary>
        public void DisposeConnection()
        {
            conn.Close();
            conn.Dispose();
            Log("SQLite内存数据库被关闭");
        }

        /// <summary>
        /// 执行sql并返回dDataSet
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public DataSet Query(string queryString)
        {
            DataSet ds = new DataSet();
            InitConnection();
            SQLiteDataAdapter da = null;
            SQLiteCommand cmd = null;
            try
            {
                cmd = new SQLiteCommand(queryString, conn);
                cmd.CommandTimeout = _CommandTimeout;
                da = new SQLiteDataAdapter(cmd);
                da.Fill(ds);
                Log($"SQLite内存数据库查询SQL成功:{queryString}");
            }
            catch (Exception err)
            {
                Log("Error", $"方法Query报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}");
                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                if (cmd != null) cmd.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// 执行sql并返回dDataSet
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DataSet Query(string queryString, SQLiteParameter[] paras)
        {
            DataSet ds = new DataSet();
            InitConnection();
            SQLiteDataAdapter da = null;
            SQLiteCommand cmd = null;
            try
            {
                cmd = new SQLiteCommand(queryString, conn);
                cmd.CommandTimeout = _CommandTimeout;
                foreach (SQLiteParameter para in paras)
                    cmd.Parameters.Add(para);
                da = new SQLiteDataAdapter(cmd);
                da.Fill(ds);
                Log($"SQLite内存数据库查询SQL成功:{queryString}\r\nSQLite参数如下:\r\n{GetParaString(paras)}");
            }
            catch (Exception err)
            {
                Log("Error",
                    $"方法Query报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}\r\nSQLite参数如下:\r\n{GetParaString(paras)}");
                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                if (cmd != null) cmd.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// 执行sql语句不返回结果集
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public int ExecuteSql(string queryString)
        {
            int result;
            InitConnection();
            SQLiteCommand cmd = null;
            try
            {
                cmd = new SQLiteCommand(queryString, conn);
                cmd.CommandTimeout = _CommandTimeout;
                result = cmd.ExecuteNonQuery();
                Log($"SQLite内存数据库执行SQL成功:{queryString}");
            }
            catch (Exception err)
            {
                Log("Error", $"方法ExecuteSql报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}");
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 执行sql语句不返回结果集
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int ExecuteSql(string queryString, SQLiteParameter[] paras)
        {
            int result;
            InitConnection();
            SQLiteCommand cmd = null;
            try
            {
                cmd = new SQLiteCommand(queryString, conn);
                cmd.CommandTimeout = _CommandTimeout;
                foreach (SQLiteParameter para in paras)
                    cmd.Parameters.Add(para);
                result = cmd.ExecuteNonQuery();
                Log($"SQLite内存数据库执行SQL成功:{queryString}\r\nSQLite参数如下:\r\n{GetParaString(paras)}");
            }
            catch (Exception err)
            {
                Log("Error",
                    $"方法ExecuteSql报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}\r\nSQLite参数如下:\r\n{GetParaString(paras)}");
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 得到自增长列最大ID
        /// </summary>
        /// <param name="IDColumnName">自增长列名</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetMaxID(string IDColumnName,string TableName)
        {
            int result;
            try
            {
                string sql = $"SELECT MAX([{IDColumnName}]) FROM [{TableName}]";
                DataSet ds = Query(sql);
                if (ds.Tables.Count == 0)
                    result = 0;
                else if (ds.Tables[0].Rows.Count == 0)
                    result = 0;
                else
                    result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            catch (Exception err)
            {
                Log("Error",
                    $"方法GetMaxID报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\n表名：{TableName}\r\n自增长列名：{IDColumnName}");
                throw;
            }
            return result;
        }

        /// <summary>
        /// Connection属性用于获取成员conn的IDbConnection接口
        /// </summary>
        public IDbConnection Connection
        {
            get { return conn; }
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public void Open()
        {
            try
            {
                InitConnection();
            }
            catch (Exception err)
            {
                Log("Error", $"方法Open报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\n连接串：{ConnectionString}");
            }
        }

        /// <summary>
        /// 释放数据库连接
        /// </summary>
        public void Close()
        {
            try
            {
                //DisposeConnection();
            }
            catch (Exception err)
            {
                Log("Error", $"方法Close报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\n连接串：{ConnectionString}");
            }
        }


        /// <summary>
        /// 执行sql并返回dDataSet
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <param name="paramList">参数列表,Key:参数名,value:参数值</param>
        /// <returns>DataSet</returns>
        public DataSet ExeSql(string queryString, IDictionary paramList)
        {
            return Query(queryString, TransfParas(paramList));
        }

        /// <summary>
        /// 执行sql语句并返回DataSet
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExeSql(string queryString)
        {
            return Query(queryString);
        }

        /// <summary>
        /// 执行sql语句并返回DataReader
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <param name="paramList">参数列表，Key:参数名,value:参数值</param>
        /// <returns>IDataReader接口</returns>
        public IDataReader ExeSqlReader(string queryString, IDictionary paramList)
        {
            SQLiteCommand cmd = null;
            try
            {
                InitConnection();
                cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandTimeout = _CommandTimeout;
                SetParameters(cmd, paramList);
                //dbc.SaveLog(cmd.CommandText);
                SQLiteDataReader dr = cmd.ExecuteReader();
                Log("SQLite内存数据库执行SQL成功:" + queryString + "\r\nSQLite参数如下:\r\n" + GetParaString(TransfParas(paramList)));
                return dr;
            }
            catch (Exception err)
            {
                Log("Error",
                    $"方法ExeSqlReader报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}\r\nSQLite参数如下:\r\n{GetParaString(TransfParas(paramList))}");
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行sql语句并返回DataReader
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <returns>IDataReader接口</returns>
        public IDataReader ExeSqlReader(string queryString)
        {
            SQLiteCommand cmd = null;
            try
            {
                InitConnection();
                cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = queryString;
                cmd.CommandTimeout = _CommandTimeout;
                //dbc.SaveLog(cmd.CommandText);
                SQLiteDataReader dr = cmd.ExecuteReader();
                Log("SQLite内存数据库执行SQL成功:" + queryString);
                return dr;
            }
            catch (Exception err)
            {
                Log("Error", $"方法ExeSqlReader报错。消息：{err.Message}\r\n堆栈：{err.StackTrace}\r\nSQL语句：{queryString}");
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行sql语句不返回结果集
        /// </summary>
        /// <param name="queryString">sql语句</param>
        /// <param name="paramList">参数列表，Key:参数名,value:参数值</param>
        public void ExeSqlNonQuery(string queryString, IDictionary paramList)
        {
            ExecuteSql(queryString, TransfParas(paramList));
        }

        /// <summary>
        /// 执行sql语句不返回结果集
        /// </summary>
        /// <param name="queryString">sql语句</param>
        public void ExeSqlNonQuery(string queryString)
        {
            ExecuteSql(queryString);
        }

        /// <summary>
        /// 为Command对象添加参数
        /// </summary>
        /// <param name="cmd">command对象</param>
        /// <param name="paramList">参数列表，Key:参数名,value:参数值</param>
        public void SetParameters(IDbCommand cmd, IDictionary paramList)
        {
            if (paramList != null)
            {
                SortedList list = new SortedList(paramList);
                foreach (DictionaryEntry item in list)
                {
                    SQLiteParameter para = new SQLiteParameter(item.Key.ToString(), item.Value);
                    ((SQLiteCommand) cmd).Parameters.Add(para);
                }
            }
        }

        /// <summary>
        /// 将IDictionary接口的参数列表转换成SQLiteParameter数组
        /// </summary>
        /// <param name="paramList">IDictionary接口的参数列表</param>
        /// <returns></returns>
        public SQLiteParameter[] TransfParas(IDictionary paramList)
        {
            if (paramList == null) return null;
            if (paramList.Count == 0) return null;

            SQLiteParameter[] result = new SQLiteParameter[paramList.Count];
            SortedList list = new SortedList(paramList);
            int i = 0;
            foreach (DictionaryEntry item in list)
            {
                result[i++] = new SQLiteParameter(item.Key.ToString(), item.Value);
            }
            return result;
        }



        /// <summary>
        /// 向数据库更新DataSet中的内容
        /// </summary>
        /// <param name="ds">需要更新的DataSet</param>
        /// <param name="strSQL">查询语句</param>
        /// <param name="tableName">查询表</param>
        /// <param name="keyField">主键字段</param>
        public void UpdateDataSet(DataSet ds,string strSQL, string tableName,string keyField)
        {
            SQLiteDataAdapter myOda = new SQLiteDataAdapter(strSQL, conn);
            //SQLiteCommandBuilder myCommandBuilder = new SQLiteCommandBuilder(myODA);
            try
            {
                myOda.Update(ds, tableName);
                ds.AcceptChanges();
            }
            finally
            {
                myOda.Dispose();
            }
        }

        /// <summary>
        /// 向DataSet更新查询语句中所查得的表的Schema信息
        /// </summary>
        /// <param name="ds">需要填充的DataSet</param>
        /// <param name="strSQL">查询语句</param>
        /// <param name="tableName">查询表</param>
        /// <param name="keyField">主键字段</param>
        public void FillSchemaDataSet(DataSet ds, string strSQL, string tableName, string keyField)
        {
            SQLiteDataAdapter myOda = new SQLiteDataAdapter(strSQL, conn);
            //SQLiteCommandBuilder myCommandBuilder = new SQLiteCommandBuilder(myODA);
            try
            {
                myOda.FillSchema(ds, SchemaType.Source, tableName);
                ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[keyField] };
                ds.AcceptChanges();
            }
            finally
            {
                myOda.Dispose();
            }
        }

        private static string GetParaString(IEnumerable<SQLiteParameter> paras)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SQLiteParameter para in paras)
                sb.AppendFormat("{0}={1}\r\n", para.ParameterName, para.Value);
            return sb.ToString();
        }

        /// <summary>
        /// 批量插入DataTable表中的数据至数据库
        /// </summary>
        /// <param name="dataTable">源数据DataTable</param>
        /// <returns>返回受影响的记录行数</returns>
        public int BulkCopyDataTable(DataTable dataTable)
        {
            return 0;
        }

        /// <summary>
        /// 取得一张表的Schema信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>返回该表的DataTable对象</returns>
        public DataTable GetDataTableSchema(string tableName)
        {
            string sql = "SELECT * FROM " + tableName + " WHERE 0=1";
            try
            {
                return ExeSql(sql).Tables[0];
            }
            catch (Exception ex)
            {
                Log("Error", $"SQLiteCommand ExeSql Error!\r\n消息：{ex.Message}\r\n堆栈：{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 释放所有被引用的资源
        /// </summary>
        public void Dispose()
        {
            // 内存数据库不能释放连接对象，一释放整个数据库就没了
            //if (conn != null && conn.State != ConnectionState.Closed)
            //    conn.Dispose();
        }
    }
}