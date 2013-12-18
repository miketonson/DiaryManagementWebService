using System;
using System.Data;
using System.Data.OleDb;
public class DBFunction
{
    private OleDbConnection con;
    private string file;

    /// <summary>
    /// 初始化数据库地址
    /// </summary>
    public DBFunction()
    {

        //file = "c:\\crawler\\1.accdb";
        file = @"C:\databases\diaryservice\1.accdb";
    }

    /// <summary>
    /// 打开数据库连接
    /// </summary>
    /// <param name="DBpath">数据库路径(包括数据库名)</param>
    private void Open(String DBpath)
    {
        if (con == null)
            con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + DBpath);
        if (con.State == ConnectionState.Closed)
            con.Open();
    }

    /// <summary>
    /// 创建一个命令对象并返回该对象
    /// </summary>
    /// <param name="sqlStr">数据库语句</param>
    /// <param name="file">数据库所在路径</param>
    /// <returns>OleDbCommand</returns>
    public OleDbCommand CreateCommand(string sqlStr)
    {
        Open(file);
        OleDbCommand cmd = new OleDbCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlStr;
        cmd.Connection = con;
        return cmd;
    }

    public int insertAndGetLastID(string sqlStr)
    {
        //OleDbCommand cmd = CreateCommand("Select @@Identity");
        OleDbCommand cmd = CreateCommand(sqlStr);
        int result = cmd.ExecuteNonQuery();
        cmd.CommandText = "Select @@Identity";
        return (int)cmd.ExecuteScalar();
        /*if (result == -1 | result == 0)
        {
            cmd.Dispose();
            Close();
            return false;
        }
        else
        {
            cmd.Dispose();
            Close();
            return true;
        }            
        return result;*/
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="sqlStr">SQL语句</param>
    /// <param name="file">数据库所在路径</param>
    /// <returns>返回数值当执行成功时候返回true,失败则返回false</returns>
    public bool ExecuteNonQuery(string sqlStr)
    {
        OleDbCommand cmd = CreateCommand(sqlStr);
        int result = cmd.ExecuteNonQuery();
        if (result == -1 | result == 0)
        {
            cmd.Dispose();
            Close();
            return false;
        }
        else
        {
            cmd.Dispose();
            Close();
            return true;
        }
    }
    /// <summary>
    /// 执行数据库查询
    /// </summary>
    /// <param name="sqlStr">查询语句</param>
    /// <param name="tableName">填充数据集表格的名称</param>
    /// <param name="file">数据库所在路径</param>
    /// <returns>查询的数据集</returns>
    public DataSet GetDataSet(string sqlStr)
    {
        DataSet ds = new DataSet();
        OleDbCommand cmd = CreateCommand(sqlStr);
        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
        dataAdapter.Fill(ds);
        cmd.Dispose();
        Close();
        dataAdapter.Dispose();
        return ds;
    }
    /// <summary>
    /// 生成一个数据读取器OleDbDataReader并返回该OleDbDataReader
    /// </summary>
    /// <param name="sqlStr">数据库查询语句</param>
    /// <returns>返回一个DataReader对象</returns>
    public OleDbDataReader GetReader(string sqlStr)
    {
        OleDbCommand cmd = CreateCommand(sqlStr);
        OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //CommadnBehavior.CloseConnection是将于DataReader的数据库链接关联起来
        //当关闭DataReader对象时候也自动关闭链接
        return reader;
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    public void Close()
    {
        if (con != null)
            con.Close();
        con = null;
    }
}
