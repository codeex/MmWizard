using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using MmWizard.Models;
using Newtonsoft.Json;

namespace MmWizard.Db
{
    /// <summary>
    /// 封装的操作
    /// </summary>
    public partial class DbConnectionWrapper : IDisposable
    {
        /// <summary>
        /// 使用SearchParameter查询并分页
        /// 先使用SearchParamerters 创建带分页的查询语句,以及动态查询参数
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sql">key</param>
        /// <param name="searchParameters">查询条件</param>
        /// <returns>列表</returns>
        public List<T> QueryByPage<T>(string key, SearchParameters searchParameters)
        {
            var sql = SiteConfig.GetSql(key);
            return QueryByPageSql<T>(sql, searchParameters);
        }

        public List<T> QueryByPageSql<T>(string sql, SearchParameters searchParameters)
        {
            searchParameters.PageInfo.TotalCount = GetTotalCountBySearchParameter(sql, searchParameters);
            string whereSql = GetSqlBySearchParameter(sql, searchParameters);
            var pageSql = GetPageSql(searchParameters, whereSql);
            DynamicParameters parameters = GetDynamicParametersBySearchParameter(searchParameters);
            try
            {
                
                return this.Conn.Query<T>(pageSql, parameters, null, false, null, CommandType.Text).AsList();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetTotalCountBySearchParameter(string sql, SearchParameters searchParameters)
        {
            if (!searchParameters.PageInfo.IsGetTotalCount)
            {
                searchParameters.PageInfo.TotalCount = 5000;
            }
            else
            {
                // 只替换第一次的select xxxx from. 子查询不替换
                var countSql = Regex.Replace(sql.Trim(), @"select[\s|\r|\n][\s\S]*?[\s|\r|\n]from", match =>
                {
                    if (match.Index == 0) return "select count(1) from";
                    return match.Value;
                }, RegexOptions.IgnoreCase);
                var countParaSql = GetSqlBySearchParameter(countSql, searchParameters);
                DynamicParameters parameters = GetDynamicParametersBySearchParameter(searchParameters);

                try
                {
                    searchParameters.PageInfo.TotalCount = this.Conn.QueryFirst<int>(countParaSql, parameters, null, null, CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return searchParameters.PageInfo.TotalCount;
        }

        /// <summary>
        /// 根据SearchParamerters 的 FiledName 转换成数据库的 ColumnName
        /// </summary>
        /// <param name="searchParameters">查询参数</param>
        /// <returns>Dapper动态参数</returns>
        private DynamicParameters GetDynamicParametersBySearchParameter(SearchParameters searchParameters)
        {
            var paramsList = new List<string>();

            List<object> valuesList = new List<object>();
            for (int i = 0; i < searchParameters.QueryModel.Items.Count; i++)
            {
                var item = searchParameters.QueryModel.Items[i];
                paramsList.Add(item.Field + i);
                switch (item.Method)
                {
                    /*
                      *   sqlText += " and Name like @Name";
                            p.Add("Name","%"+ model.Name+"%");
                      */
                    case QueryMethod.Contains:
                    case QueryMethod.NotLike:
                        valuesList.Add("%" + item.Value + "%");
                        break;

                    case QueryMethod.StartsWith:
                        valuesList.Add(item.Value + "%");
                        break;

                    case QueryMethod.EndsWith:
                        valuesList.Add("%" + item.Value);
                        break;
                    /*
                     * string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
                        var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 }});
                     */
                    case QueryMethod.StdIn:
                    case QueryMethod.StdNotIn:
                    case QueryMethod.GreaterThan:
                    case QueryMethod.GreaterThanOrEqual:
                    case QueryMethod.Equal:
                    case QueryMethod.LessThan:
                    case QueryMethod.LessThanOrEqual:
                    case QueryMethod.NotEqual:
                        valuesList.Add(item.Value);
                        break;

                    default:
                        valuesList.Add(item.Value);
                        break;
                }
            }

            return PrepareCommand(paramsList.ToArray(), valuesList.ToArray());
        }

        protected DynamicParameters PrepareCommand(string[] paramsList, object[] valuesList)
        {
            DynamicParameters parameters = new DynamicParameters();
            if (paramsList != null && paramsList.Length > 0 && valuesList != null && valuesList.Length > 0)
            {
                if (paramsList.Length < valuesList.Length)
                {
                    throw new Exception("paramsList.length<values.length error");
                }

                for (int i = 0; i < paramsList.Length; i++)
                {
                    // 把前缀补上
                    // string key = paramsList[i].Trim().StartsWith(Prefix) ? paramsList[i].Trim() : Prefix + paramsList[i].Trim();
                    string key = Prefix + paramsList[i].Trim(); // 不能有上面的判断，要不会埋坑，顶层的参数不能携带特定数据库的前缀
                    parameters.Add(key, valuesList[i]);
                }
            }

            return parameters;
        }
        /// <summary>
        /// 使用SearchParamerters 自动追加 sql 的 where 条件, 并用占位符处理value   例: `ORDER_ID`=@OrderId
        /// 并根据PageInfo信息生成sql分页内容
        /// </summary>
        /// <param name="sql">sqlkey</param>
        /// <param name="searchParameters">查询条件</param>
        /// <returns>返回一个组织好的sql语句</returns>
        private string GetSqlBySearchParameter(string sql, SearchParameters searchParameters)
        {
            StringBuilder sb = new StringBuilder(sql);
            if (searchParameters?.QueryModel?.Items != null &&
                searchParameters.QueryModel.Items.Count > 0)
            {
                sb.Append(" where " + ConvertToSqlWhere(searchParameters));
            }
            if (string.IsNullOrEmpty(searchParameters?.PageInfo?.SortDirection))
            {
                if (!string.IsNullOrEmpty(searchParameters?.PageInfo?.SortField))
                {
                    sb.Append(" Order by ");
                    //如果是多字段排序，则会把排序字段和排序方式记录到sort上，dir为空
                    char[] delimiters = { ',' };
                    string[] sorts =
                        searchParameters.PageInfo.SortField.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < sorts.Length; i++)
                    {
                        string[] values = sorts[i].Trim().Split(' ');
                        string sortName = values[0];
                        string sortDir = values.Length == 2 ? values[1] : "ASC";
                        if (i == sorts.Length - 1)
                        {
                            sb.Append(" " + sortName + " " + sortDir + " ");
                        }
                        else
                        {
                            sb.Append(" " + sortName + " " + sortDir + ", ");
                        }
                    }
                }
            }
            else
            {
                var sortName = searchParameters?.PageInfo?.SortField;
                if (!string.IsNullOrEmpty(sortName))
                {
                    sb.Append(" Order by " + sortName + " " + searchParameters.PageInfo.SortDirection + " ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取一个分页Sql
        /// </summary>
        /// <param name="searchParameters">查询条件</param>
        /// <param name="sql">查询主sql</param>
        /// <returns>分页sql</returns>
        private string GetPageSql(SearchParameters searchParameters, string sql)
        {
            if (searchParameters?.PageInfo == null)
            {
                return sql;
            }

            var pageInfo = searchParameters.PageInfo;
            pageInfo.CurrentPage = pageInfo.CurrentPage < 1 ? 1 : pageInfo.CurrentPage;
            var skipCount = pageInfo.SkipCount > 0 ? pageInfo.SkipCount : ((pageInfo.CurrentPage - 1) * pageInfo.PageSize);

            if (skipCount >= 0 && pageInfo.PageSize > 0)
            {
                //通过总条数检查当前页是有效(有数据), 如果没数据,则自动将页码设置为最后一页
                if (pageInfo.TotalCount > 0 && skipCount >= pageInfo.TotalCount)
                {
                    //获取总页数
                    var totalPage = pageInfo.TotalCount / pageInfo.PageSize + (pageInfo.TotalCount % pageInfo.PageSize > 0 ? 1 : 0);
                    //设置查询最后一页数据
                    skipCount = (totalPage - 1) * pageInfo.PageSize;
                }
                sql += $" limit {skipCount} , {pageInfo.PageSize}";
            }
            else
            {
                sql += $" limit 0 , 0";
            }

            return sql;
        }

        protected readonly string Prefix = "@";
        /// <summary>
        /// 根据QueryModel组织sqlWhere语句,如果有字段前缀的话,需要提前增加进来
        /// 如果是Dapper 需要转换成
        /// </summary>
        /// <returns>Where 语句</returns>
        public string ConvertToSqlWhere(SearchParameters searchParameters, string dbType = "mysql")
        {
            if (searchParameters?.QueryModel?.Items == null) return "";
            // 复制一个,避免修改的时候影响外部数据
            var copyCondition = searchParameters.QueryModel.Items?.ToArray().ToList();
            var sb = new StringBuilder();
            List<string> groups = new List<string>();

            for (int i = 0; i < copyCondition.Count; i++)
            {
                var item = copyCondition[i];
                
                if (item.Value != null)
                {
                    item.Value = Prefix + item.Field + i;
                }
            }

            foreach (var conditionItem in copyCondition)
            {
                if (!string.IsNullOrEmpty(conditionItem.OrGroup))
                {
                    if (!groups.Contains(conditionItem.OrGroup))
                    {
                        var sbChild = new StringBuilder();
                        foreach (var senItem in copyCondition)
                        {
                            if (senItem.OrGroup == conditionItem.OrGroup)
                            {
                                if (sbChild.Length > 0)
                                {
                                    sbChild.Append(" or ");
                                }
                                sbChild.Append(GetQueryCloumn(senItem) + " " + ConvertMethodToSql(senItem.Method, senItem.Value));
                            }
                        }
                        if (sb.Length > 0)
                            sb.Append(" and ");
                        sb.Append("(" + sbChild.ToString() + ")");
                        groups.Add(conditionItem.OrGroup);
                    }
                }
                else
                {
                    if (sb.Length > 0)
                        sb.Append(" and ");
                    sb.Append((string.IsNullOrEmpty(conditionItem.Prefix) ? "" : (conditionItem.Prefix + ".")) + conditionItem.Field + " " + ConvertMethodToSql(conditionItem.Method, conditionItem.Value));
                }
            }
            return sb.ToString();
        }

        protected  string GetQueryCloumn(ConditionItem senItem)
        {
            return (string.IsNullOrEmpty(senItem.Prefix) ? "" : (senItem.Prefix + ".")) + senItem.Field;
        }
        /// <summary>
        /// 将Method转换成Dapper语法的查询语句
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="value">值</param>
        /// <returns>条件</returns>
        protected virtual string ConvertMethodToSql(QueryMethod method, object value)
        {
            switch (method)
            {
                /*
                  *   sqlText += " and Name like @Name";
                        p.Add("Name","%"+ model.Name+"%");
                  */
                case QueryMethod.Contains:
                case QueryMethod.StartsWith:
                case QueryMethod.EndsWith:
                    return "like " + value;
                /*
                 * string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
                    var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 }});
                 */
                case QueryMethod.StdIn:
                    return "in " + value;

                case QueryMethod.StdNotIn:
                    return "not in " + value;

                case QueryMethod.NotLike:
                    return "not like " + value;
                ////数字类型处理
                case QueryMethod.GreaterThan:
                    return "> " + value;

                case QueryMethod.GreaterThanOrEqual:
                    return ">= " + value;

                case QueryMethod.Equal:
                    if (value == null)
                    {
                        return " is null ";
                    }
                    return "= " + value;

                case QueryMethod.LessThan:
                    return "< " + value;

                case QueryMethod.LessThanOrEqual:
                    return "<= " + value;

                case QueryMethod.NotEqual:
                    if (value == null)
                    {
                        return " is not null ";
                    }
                    return "<> " + value;

                default:
                    return "";
            }
        }

    }
}
