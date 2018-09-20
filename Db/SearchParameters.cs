using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public class SearchParameters
    {
        /// <summary>
        ///     构造一个空的查询条件
        /// </summary>
        public SearchParameters()
        {
            QueryModel = new QueryModel();
            PageInfo = new PageInfo
            {
                CurrentPage = 1,
                IsPaging = true,
                PageSize = 20,
                SortDirection = "",
                SortField = "",
            };
        }

        /// <summary>
        ///     具体的查询条件
        /// </summary>
        [DataMember]
        public QueryModel QueryModel { get; set; }

        /// <summary>
        ///     分页参数
        /// </summary>
        [DataMember]
        public PageInfo PageInfo { get; set; }

        /// <summary>
        ///     根据JqGrid 的过滤参数转换成查询条件枚举
        ///     [{ oper:'eq', text:'equal'},{ oper:'ne', text:'not equal'},{ oper:'lt', text:'less'},
        ///     { oper:'le', text:'less or equal'},{ oper:'gt', text:'greater'},{ oper:'ge', text:'greater or equal'},
        ///     { oper:'bw', text:'begins with'},{ oper:'bn', text:'does not begin with'},{ oper:'in', text:'is in'},
        ///     { oper:'ni', text:'is not in'},{ oper:'ew', text:'ends with'},{ oper:'en', text:'does not end with'},
        ///     { oper:'cn', text:'contains'},{ oper:'nc', text:'does not contain'}]
        /// 补充
        ///      { oper:'mcn', text:'contains'},{ oper:'mcn', text:'more like'}
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private QueryMethod GetMethodByJqGridOp(string op)
        {
            switch (op)
            {
                case "eq":
                    return QueryMethod.Equal;

                case "ne":
                    return QueryMethod.NotEqual;

                case "lt":
                    return QueryMethod.LessThan;

                case "le":
                    return QueryMethod.LessThanOrEqual;

                case "gt":
                    return QueryMethod.GreaterThan;

                case "ge":
                    return QueryMethod.GreaterThanOrEqual;

                case "bw":
                    return QueryMethod.StartsWith;

                case "in":
                    return QueryMethod.StdIn;

                case "ni":
                    return QueryMethod.StdNotIn;

                case "ew":
                    return QueryMethod.EndsWith;

                case "cn":
                    return QueryMethod.Contains;

                case "nc":
                    return QueryMethod.NotLike;

                default:
                    throw new Exception("不支持此查询条件");
            }
        }

        /// <summary>
        /// 根据QueryModel组织sqlWhere语句,如果有字段前缀的话,需要提前增加进来
        /// </summary>
        /// <returns></returns>
        public string GetSqlWhere()
        {
            var sb = new StringBuilder();
            List<string> groups = new List<string>();
            foreach (var conditionItem in QueryModel.Items)
            {
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(conditionItem.OrGroup))
                {
                    if (!groups.Contains(conditionItem.OrGroup))
                    {
                        var sbChild = new StringBuilder();
                        foreach (var senItem in QueryModel.Items)
                        {
                            if (senItem.OrGroup == conditionItem.OrGroup)
                            {
                                if (sbChild.Length > 0)
                                {
                                    sbChild.Append(" or ");
                                }
                                sbChild.Append((string.IsNullOrEmpty(senItem.Prefix) ? "" : (senItem.Prefix + ".")) + senItem.Field + " " + ConvertMethodToSql(senItem.Method, senItem.Value));
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

        /// <summary>
        /// 将Method转换成sql语法的查询语句
        /// </summary>
        /// <param name="method"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertMethodToSql(QueryMethod method, object value)
        {
            switch (method)
            {
                ////字符串类型处理
                case QueryMethod.Contains:
                    return "like '%" + value + "%'";

                case QueryMethod.StdIn:
                    return "in ('" + string.Join("','", value as string[]) + "')";

                case QueryMethod.NotLike:
                    return "not like '%" + value + "%'";
                ////数字类型处理
                case QueryMethod.GreaterThan:
                    return "> '" + value + "'";

                case QueryMethod.GreaterThanOrEqual:
                    return ">= '" + value + "'";

                case QueryMethod.Equal:
                    return "= '" + value + "'";

                case QueryMethod.LessThan:
                    return "< '" + value + "'";

                case QueryMethod.LessThanOrEqual:
                    return "<= '" + value + "'";

                case QueryMethod.StdNotIn:
                    return "not in  ('" + string.Join("','", value as string[]) + "')";

                case QueryMethod.NotEqual:
                    return "<> '" + value + "'";

                case QueryMethod.StartsWith:
                    return "like '" + value + "%'";

                case QueryMethod.EndsWith:
                    return "like '%" + value + "'";

                default:
                    return "";
            }
        }
    }
}
