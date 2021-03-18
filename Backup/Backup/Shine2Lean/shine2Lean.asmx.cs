using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace Shine2Lean
{
    /// <summary>
    /// shine2Lean 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class shine2Lean : System.Web.Services.WebService
    {
        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-06-16， HelloWorld可用于测试服务是否正常。")]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-06-16 。")]
        public string queueTypeAllTree() //队列树状结构
        {
            const string sql = @"SELECT
	                        0 AS pid,
	                        tr.id AS id,
	                        tr.name AS name,
	                        tr.ip AS traige_id,
	                        '' AS login_id
                        FROM
	                        triage tr
                        UNION
	                        SELECT
		                        traige_id AS pid,
		                        queue_type_id AS id,
		                        queue_type_name AS name,
		                        '' AS traige_id,
		                        login_id AS login_id
	                        FROM
		                        traige2queue_type";
            var dt = SqlHelper.ExecuteQuery(sql, null);
            var json = "error";
            if (dt != null)
            {
                json = DataTable2Json(dt, "queueTypeAllTree");
            }

            return json;
        }

        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-06-16 。")]
        public string queueTypeCurrentTree()
        {
            const string sql = @"SELECT DISTINCT
	                0 AS pid,
	                traige_id AS id,
	                traige_name AS name,
	                null AS login_id
                FROM
	                current_patients
                WHERE
	                len(traige_id) > 0
                UNION
	                SELECT DISTINCT
		                traige_id AS pid,
		                queue_type_id AS id,
		                queue_type_name AS name,
		                login_id AS login_id
	                FROM
		                current_patients
	                WHERE
		                len(traige_id) > 0";
            var dt = SqlHelper.ExecuteQuery(sql, null);
            var json = "error";
            if (dt != null)
            {
                json = DataTable2Json(dt, "queueTypeCurrentTree");
            }

            return json;
        }

        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-06-16
         。")]
        public string currentPatient(string json)
        {
            try
            {
                var jsonData = json;
                var strSql = new StringBuilder();
                strSql.Append("select * from current_patients  where 1=1");
                var serializer = new JavaScriptSerializer();
                var Json = (Dictionary<string, object>) serializer.DeserializeObject(jsonData);
                for (var i = 0; i < Json.Count; i++)
                {
                    switch (Json.ElementAt(i).Key.Trim())
                    {
                        case "patient_id":
                            var patientId = Json.ElementAt(i).Value.ToString();
                            if (!string.IsNullOrEmpty(patientId))
                            {
                                strSql.Append("and patient_id='" + patientId + "'");
                            }

                            break;
                        case "traige_id":
                            var traigeId = Json.ElementAt(i).Value.ToString();
                            if (!string.IsNullOrEmpty(traigeId))
                            {
                                strSql.Append("and traige_id='" + traigeId + "'");
                            }

                            break;
                        case "queue_type_id":
                            var queueTypeId = Json.ElementAt(i).Value.ToString();
                            if (!string.IsNullOrEmpty(queueTypeId))
                            {
                                strSql.Append("and queue_type_id='" + queueTypeId + "'");
                            }

                            break;
                        case "login_id":
                            var loginId = Json.ElementAt(i).Value.ToString();
                            if (!string.IsNullOrEmpty(loginId))
                            {
                                strSql.Append("and login_id='" + loginId + "'");
                            }

                            break;
                    }
                }

                var returnJson = "error";
                var dt = SqlHelper.ExecuteQuery(strSql.ToString());
                if (dt == null) return "";
                returnJson = DataTable2Json(dt, "currentPatient");
                return returnJson;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-07-28，获取当前时间的门诊候诊人数。")]
        public string GetHzrc()
        {
            const string sql = @"SELECT  [1号楼1楼候诊区/心胸外科、普外科、普外（甲乳）、肛肠外科、神经外科、骨科、骨科（关节、脊柱、创伤）、手足外科、烧伤外科] as 'a_1'
                        ,[1号楼2楼候诊区/康复医学科、疼痛科、中医科、老年病科] as 'a_2'
                        ,[1号楼3楼候诊区/妇科、产科] as 'a_3'
                        ,[3号楼1楼候诊区/肝病门诊、内科门诊] as 'c_1'
                        ,[3号楼2楼候诊区/皮肤科、泌尿科] as 'c_2'
                        ,[3号楼3楼候诊区/心血管内科、呼吸内科、戒烟、消化内科、血液肿瘤科、内分泌科、风湿免疫科、神经内科、肾内科、全科医疗科、精神卫生门诊、营养咨询门诊、放射科门诊] as 'c_3'
                        ,[3号楼4楼候诊区/儿科、耳鼻喉科] as 'c_4'
                        ,[3号楼5楼候诊区/口腔科] as 'c_5'
                        FROM [ShineTriage].[dbo].[v_sstj_hzrc]";
            var dt = SqlHelper.ExecuteQuery(sql, null);
            var json = Table2Json(dt);
            return json;
        }

        [WebMethod(Description = @"台州市第一人民医院 开发者zzj,于2020-07-28， 补全医生信息。")]
        public string DocInfo(string input)
        {
            XmlTextReader reader = null;
            try
            {
                var xmlDs = new DataSet();
                var stream = new StringReader(input);
                reader = new XmlTextReader(stream);
                xmlDs.ReadXml(reader);
                var docInfoDt = xmlDs.Tables["BodyList"];
                if (docInfoDt != null)
                {
                    var hour = DateTime.Now.Hour;
                    docInfoDt.Columns.Add("description", Type.GetType("System.String"));
                    docInfoDt.Columns.Add("title", Type.GetType("System.String"));
                    const string sql = "select [login_id],[title],[description] FROM [ShineTriage].[dbo].[doctor]";
                    var dt = SqlHelper.ExecuteQuery(sql, null);
                    var rtDocInfo = new DataTable();
                    rtDocInfo.Columns.Add("DoctorId", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("DeptName", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("RegistedCount", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("NumCount", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("DoctorName", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("description", Type.GetType("System.String"));
                    rtDocInfo.Columns.Add("title", Type.GetType("System.String"));
                    foreach (DataRow dr in docInfoDt.Rows)
                    {
                        var drw = dt.Select("login_id='" + dr["DoctorId"].ToString() + "'");
                        if (drw.Length <= 0) continue;
                        dr["description"] = drw[0]["description"].ToString().Trim().Replace("\n", "")
                            .Replace("\r", "");
                        dr["title"] = drw[0]["title"].ToString().Trim();
                        if (hour < 12 && dr["TimeDesc"].ToString() == "上午") //判断是早上的时候 筛除掉下午出诊的医生
                        {
                            DataRow dar = rtDocInfo.NewRow();
                            dar["DoctorId"] = dr["DoctorId"];
                            dar["DeptName"] = dr["DeptName"];
                            dar["RegistedCount"] = dr["RegistedCount"];
                            dar["NumCount"] = dr["NumCount"];
                            dar["DoctorName"] = dr["DoctorName"];
                            dar["description"] = dr["description"];
                            dar["title"] = dr["title"];
                            rtDocInfo.Rows.Add(dar);
                        }

                        if (hour > 12 && dr["TimeDesc"].ToString() == "下午")
                        {
                            var dar = rtDocInfo.NewRow();
                            dar["DoctorId"] = dr["DoctorId"];
                            dar["DeptName"] = dr["DeptName"];
                            dar["RegistedCount"] = dr["RegistedCount"];
                            dar["NumCount"] = dr["NumCount"];
                            dar["DoctorName"] = dr["DoctorName"];
                            dar["description"] = dr["description"];
                            dar["title"] = dr["title"];
                            rtDocInfo.Rows.Add(dar);
                        }
                    }

                    var rt = Data2Json(rtDocInfo, "docInfo");
                    return rt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                var a = e.ToString();
                return null;
            }
            finally
            {
                reader?.Close();
            }
        }

        private static string Table2Json(DataTable dt)
        {
            var jsonBuilder = new System.Text.StringBuilder();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            return jsonBuilder.ToString();
        }

        private static string DataTable2Json(DataTable dt, string tableName)
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"" + tableName + "\":[ ");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        private static string Data2Json(DataTable dt, string tableName)
        {
            var jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"" + tableName + "\":[ ");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dt.Rows[i]["DoctorName"].ToString().Trim())) continue;
                jsonBuilder.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }

                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
    }
}