using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace GalleryWeb.Controllers
{
    public class WebController : Controller
    {
        // GET: Tes
        string user_login;
        Models.IKSDAL DAL;
        //constructor to to initialize
        public WebController()
        {
            DAL = new Models.IKSDAL("Server=SERVERIP;Database=DATABASENAME;User Id=DBUSER;Password=DBPW;");
        }

        public ActionResult Index()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            return View();
        }

        public ActionResult Tes()
        {
            return View();
        }

        public ActionResult GroupList()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            } else
            {
                return RedirectToAction("Login", "Auth");
            }
            //check group IT
            List<SqlParameter> parameterc = new List<SqlParameter>();
            string sqlcek = "select b.GroupName from [dbo].[CaptureMobile_GroupMember] a  left join[dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID where b.GroupName = 'IT' and UserID = @userid";
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            string groupname = (string)DAL.ExecuteScalar(sqlcek, parameterc);

            //

            //
            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            string sql;
            if(groupname == "IT")
            {
                sql = "select GroupID, GroupName from [dbo].[CaptureMobile_Group]";

            }
            else
            {
                sql = "select a.GroupID, b.GroupName from [dbo].[CaptureMobile_GroupMember] a join[dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID where a.UserID = @userid";

            }
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            dt = DAL.GetDataTable(sql, parameter);

            List<Models.Group> dirgroup = new List<Models.Group>();

            foreach (DataRow row in dt.Rows)
            {
                dirgroup.Add(new Models.Group
                {
                    GroupID = Convert.ToInt32(row["GroupID"]),
                    GroupName = row["GroupName"].ToString(),
                });
            }

            return View(dirgroup);
        }

        public ActionResult Gallery(int id, int page)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            DataTable dt1 = new DataTable();
            List<SqlParameter> parameter1 = new List<SqlParameter>();

            //
            int count_total;
            string sqlcount = "SELECT COUNT(ID) AS CountPict FROM [dbo].[CaptureMobile] WHERE CrtUsrID in (select UserID from[CaptureMobile_GroupMember] where GroupID = @groupid) AND ActiveFlag='Y' ";
            List<SqlParameter> parameterc = new List<SqlParameter>();
            parameterc.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.Int, Value = id });

            count_total = (int)DAL.ExecuteScalar(sqlcount, parameterc);

            double result = (double)count_total / 8;
            int roundedUpResult = (int)Math.Ceiling(result);

            if (page > roundedUpResult)
            {
                ViewBag.ErrorMessage = "Internal Server Error: Not Found";
                return View("Error");
            }

            ViewBag.PageNum = page;
            ViewBag.MaxPageNum = roundedUpResult;
            ViewBag.GroupID = id;

            ////PICTURES
            DataTable dt2 = new DataTable();
            List<SqlParameter> parameter2 = new List<SqlParameter>();


            string sql2 = "SELECT * FROM [dbo].[CaptureMobile] WHERE CrtUsrID in (select UserID from[CaptureMobile_GroupMember] where GroupID = @groupid) AND ActiveFlag='Y' ORDER BY TsCrt desc OFFSET(@pagenum - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY;";
            parameter2.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.Int, Value = id });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagenum", SqlDbType = SqlDbType.Int, Value = page });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagesize", SqlDbType = SqlDbType.Int, Value = 8 });


            dt2 = DAL.GetDataTable(sql2, parameter2);

            List<Models.Picture> Pictures = new List<Models.Picture>();

            int rowcount = dt2.Rows.Count;

            foreach (DataRow row in dt2.Rows)
            {
                Pictures.Add(new Models.Picture
                {
                    ID = Convert.ToInt32(row["ID"]),
                    DirName = row["DirName"].ToString(),
                    CaptureFile = "https://iksmill.app.co.id/cameraapi" + row["CaptureFIle"].ToString().Substring(1) ,
                    CrtUsrID = row["CrtUsrID"].ToString(),
                    TsCrt = row["TsCrt"].ToString(),
                    ModUsr = row["ModUsrID"].ToString(),
                    TsMod = row["TsMod"].ToString(),
                    ActiveFlag = row["ActiveFlag"].ToString(),
                });
            }

            return View(Pictures);
        }

        public ActionResult DirectoryList()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            string sql = "select DirName, max(TsCrt) TsCrt from CaptureMobile where CrtUsrID IN ( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid)  GROUP BY UserID ) GROUP BY DirName order by max(TsCrt) desc";
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            dt = DAL.GetDataTable(sql, parameter);

            List<Models.DirName> directorylist = new List<Models.DirName>();

            foreach (DataRow row in dt.Rows)
            {
                directorylist.Add(new Models.DirName
                {
                    Name = row["DirName"].ToString(),
                    TsCrt = row["TsCrt"].ToString()
                });
            }

            return View(directorylist);
        }

        public ActionResult Directory(string id, int page)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            DataTable dt1 = new DataTable();
            List<SqlParameter> parameter1 = new List<SqlParameter>();

            //
            int count_total;
            string sqlcount = "SELECT COUNT(ID) AS CountPict FROM CaptureMobile WHERE CrtUsrID IN( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid) GROUP BY UserID) AND DirName = @dirname AND ActiveFlag = 'Y'";
            List<SqlParameter> parameterc = new List<SqlParameter>();
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            parameterc.Add(new SqlParameter() { ParameterName = "@dirname", SqlDbType = SqlDbType.VarChar, Value = id });

            count_total = (int)DAL.ExecuteScalar(sqlcount, parameterc);

            double result = (double)count_total / 8;
            int roundedUpResult = (int)Math.Ceiling(result);

            if (page > roundedUpResult)
            {
                ViewBag.ErrorMessage = "Internal Server Error: Not Found";
                return View("Error");
            }

            ViewBag.PageNum = page;
            ViewBag.MaxPageNum = roundedUpResult;
            ViewBag.DirName = id;

            ////PICTURES
            DataTable dt2 = new DataTable();
            List<SqlParameter> parameter2 = new List<SqlParameter>();


            string sql2 = "SELECT * FROM CaptureMobile WHERE CrtUsrID IN( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid) GROUP BY UserID) AND DirName = @dirname AND ActiveFlag = 'Y' ORDER BY TsCrt desc OFFSET(@pagenum - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY;";
            parameter2.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            parameter2.Add(new SqlParameter() { ParameterName = "@dirname", SqlDbType = SqlDbType.VarChar, Value = id });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagenum", SqlDbType = SqlDbType.Int, Value = page });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagesize", SqlDbType = SqlDbType.Int, Value = 8 });


            dt2 = DAL.GetDataTable(sql2, parameter2);

            List<Models.Picture> Pictures = new List<Models.Picture>();

            int rowcount = dt2.Rows.Count;

            foreach (DataRow row in dt2.Rows)
            {
                Pictures.Add(new Models.Picture
                {
                    ID = Convert.ToInt32(row["ID"]),
                    DirName = row["DirName"].ToString(),
                    CaptureFile = "https://iksmill.app.co.id/cameraapi" + row["CaptureFIle"].ToString().Substring(1),
                    CrtUsrID = row["CrtUsrID"].ToString(),
                    TsCrt = row["TsCrt"].ToString(),
                    ModUsr = row["ModUsrID"].ToString(),
                    TsMod = row["TsMod"].ToString(),
                    ActiveFlag = row["ActiveFlag"].ToString(),
                });
            }

            return View(Pictures);
        }

        public ActionResult Directory2(string id, int page)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            DataTable dt1 = new DataTable();
            List<SqlParameter> parameter1 = new List<SqlParameter>();

            //
            int count_total;
            string sqlcount = "SELECT COUNT(ID) AS CountPict FROM CaptureMobile WHERE CrtUsrID IN( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid) GROUP BY UserID) AND DirName = @dirname AND ActiveFlag = 'Y'";
            List<SqlParameter> parameterc = new List<SqlParameter>();
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            parameterc.Add(new SqlParameter() { ParameterName = "@dirname", SqlDbType = SqlDbType.VarChar, Value = id });

            count_total = (int)DAL.ExecuteScalar(sqlcount, parameterc);

            double result = (double)count_total / 8;
            int roundedUpResult = (int)Math.Ceiling(result);

            if (page > roundedUpResult)
            {
                ViewBag.ErrorMessage = "Internal Server Error: Not Found";
                return View("Error");
            }

            ViewBag.PageNum = page;
            ViewBag.MaxPageNum = roundedUpResult;
            ViewBag.DirName = id;

            ////PICTURES
            DataTable dt2 = new DataTable();
            List<SqlParameter> parameter2 = new List<SqlParameter>();


            string sql2 = "SELECT * FROM CaptureMobile WHERE CrtUsrID IN( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid) GROUP BY UserID) AND DirName = @dirname AND ActiveFlag = 'Y' ORDER BY TsCrt desc OFFSET(@pagenum - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY;";
            parameter2.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            parameter2.Add(new SqlParameter() { ParameterName = "@dirname", SqlDbType = SqlDbType.VarChar, Value = id });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagenum", SqlDbType = SqlDbType.Int, Value = page });
            parameter2.Add(new SqlParameter() { ParameterName = "@pagesize", SqlDbType = SqlDbType.Int, Value = 8 });


            dt2 = DAL.GetDataTable(sql2, parameter2);

            List<Models.Picture> Pictures = new List<Models.Picture>();

            int rowcount = dt2.Rows.Count;

            foreach (DataRow row in dt2.Rows)
            {
                Pictures.Add(new Models.Picture
                {
                    ID = Convert.ToInt32(row["ID"]),
                    DirName = row["DirName"].ToString(),
                    CaptureFile = "https://iksmill.app.co.id/cameraapi" + row["CaptureFIle"].ToString().Substring(1),
                    CrtUsrID = row["CrtUsrID"].ToString(),
                    TsCrt = row["TsCrt"].ToString(),
                    ModUsr = row["ModUsrID"].ToString(),
                    TsMod = row["TsMod"].ToString(),
                    ActiveFlag = row["ActiveFlag"].ToString(),
                });
            }

            return View(Pictures);
        }

        public ActionResult GroupMember()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            //Check if IT if not back to group list
            List<SqlParameter> parameterc = new List<SqlParameter>();
            string sqlcek = "select b.GroupName from [dbo].[CaptureMobile_GroupMember] a  left join[dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID where b.GroupName = 'IT' and UserID = @userid";
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            string groupname = (string)DAL.ExecuteScalar(sqlcek, parameterc);

            //

            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            string sql;

            if (groupname == "IT")
            {
                sql = "SELECT a.GroupID, a.UserId, b.GroupName, a.CrtUsrID, a.TsCrt, a.ModUsrID, a.TsMod, a.ActiveFlag ,(ltrim(rtrim(c.first_name)) + ' ' + ltrim(rtrim(c.middle_name)) + ' ' + ltrim(rtrim(c.last_name))) as FullName FROM [dbo].[CaptureMobile_GroupMember] a LEFT JOIN [dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID LEFT JOIN[SRGSQL11].[User_Registration].[dbo].[employee] c on a.UserID = ltrim(rtrim(c.id_user))";
            } else
            {
                sql = "SELECT a.GroupID, a.UserId, b.GroupName, a.CrtUsrID, a.TsCrt, a.ModUsrID, a.TsMod, a.ActiveFlag ,(ltrim(rtrim(c.first_name)) + ' ' + ltrim(rtrim(c.middle_name)) + ' ' + ltrim(rtrim(c.last_name))) as FullName FROM [dbo].[CaptureMobile_GroupMember] a LEFT JOIN [dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID LEFT JOIN[SRGSQL11].[User_Registration].[dbo].[employee] c on a.UserID = ltrim(rtrim(c.id_user)) WHERE a.GroupID in (SELECT GroupID FROM[dbo].[CaptureMobile_GroupMember] WHERE UserID = @userid)";
            }


            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            dt = DAL.GetDataTable(sql, parameter);

            List<Models.GroupMember> groupmember = new List<Models.GroupMember>();

            foreach (DataRow row in dt.Rows)
            {
                groupmember.Add(new Models.GroupMember
                {
                    GroupID = Convert.ToInt32(row["GroupID"]),
                    UserID = row["UserID"].ToString(),
                    GroupName = row["GroupName"].ToString(),
                    CrtUsrID = row["CrtUsrID"].ToString(),
                    TsCrt = row["TsCrt"].ToString(),
                    ModUsrID = row["ModUsrID"].ToString(),
                    TsMod = row["TsMod"].ToString(),
                    ActiveFlag = row["ActiveFlag"].ToString(),
                    FullName = row["FullName"].ToString(),
                });
            }

            return View(groupmember);
        }



        public ActionResult GroupMemberMod(string id)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //

            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            string action = id;

            if (id == "create")
            {

            }
            if (id == "delete")
            {

            }

            return Redirect(returnUrl);
        }


        public ActionResult AddGroupMember()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //


            return View();
        }

        public ActionResult AddGroupList()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            //Check if IT if not back to group list
            List<SqlParameter> parameterc = new List<SqlParameter>();
            string sqlcek = "select b.GroupName from [dbo].[CaptureMobile_GroupMember] a  left join[dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID where b.GroupName = 'IT' and UserID = @userid";
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            string groupname = (string)DAL.ExecuteScalar(sqlcek, parameterc);


            if (groupname != "IT")
            {
                return RedirectToAction("GroupList", "Web");
            }
            //

            return View();
        }

        [HttpPost]
        public ActionResult AddGroupListProcess(FormCollection form)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            //
            string GroupName = form["GroupName"];
            //
            //cek exist
            //
            List<SqlParameter> parameter = new List<SqlParameter>();
            string sqlcek = "select count(1) as count_data from [dbo].[CaptureMobile_Group] where GroupName = @groupname";
            parameter.Add(new SqlParameter() { ParameterName = "@groupname", SqlDbType = SqlDbType.VarChar, Value = GroupName });


            int count_cek = (int)DAL.ExecuteScalar(sqlcek, parameter);
            if (count_cek == 0)
            {
                List<SqlParameter> parameter2 = new List<SqlParameter>();

                string sql = "INSERT INTO [dbo].[CaptureMobile_Group] ([GroupName],[CrtUsrID],[TsCrt],[ModUsrID],[TsMod],[ActiveFlag])VALUES(@groupname,@userid,getdate(),@userid,getdate(),'Y')";
                parameter2.Add(new SqlParameter() { ParameterName = "@groupname", SqlDbType = SqlDbType.VarChar, Value = GroupName });
                parameter2.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

                DAL.ExecuteNonQuery(sql, parameter2);
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "Group was successfully added to the group list";

            }
            else
            {
                ViewBag.Success = false;
                ModelState.AddModelError("", "Failed add group to group list");
            }
            //

            return View("AddGroupList");
        }

        public ActionResult Tes2()
        {

            return View();
        }

        public ActionResult DeleteGroupMember()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            string UserID = HttpContext.Request.QueryString["id"];
            string GroupID = HttpContext.Request.QueryString["group"];
            

            List<SqlParameter> parameter = new List<SqlParameter>();
            string sqlcek = "select count(1) as count_data from [dbo].[CaptureMobile_GroupMember] where GroupID = @groupid and UserID = @userid";
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = UserID });
            parameter.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.VarChar, Value = GroupID });

            int count_cek = (int)DAL.ExecuteScalar(sqlcek, parameter);
            if (count_cek == 1)
            {
                List<SqlParameter> parameter2 = new List<SqlParameter>();

                string sql = "DELETE FROM [dbo].[CaptureMobile_GroupMember] WHERE UserID = @userid AND GroupID = @groupid";
                parameter2.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = UserID });
                parameter2.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.VarChar, Value = GroupID });
                DAL.ExecuteNonQuery(sql, parameter2);
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "User was successfully removed the group";
            }
            else
            {
                ViewBag.Success = false;
                ModelState.AddModelError("", "Failed remove user from the group");
            }


            return RedirectToAction("GroupMember", "Web");
        }


        [HttpPost]
        public ActionResult AddGroupMemberProcess(Models.UserAdded model)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            //
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            //
            int GroupID = model.GroupID;
            string UserID = model.UserID;
            string CrtUsrID = user_login;
            //
            //cek exist
            //

            List<SqlParameter> parameter = new List<SqlParameter>();
            string sqlcek = "select count(1) as count_data from [dbo].[CaptureMobile_GroupMember] where GroupID = @groupid and UserID = @userid";
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = UserID });
            parameter.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.VarChar, Value = GroupID });

            int count_cek = (int)DAL.ExecuteScalar(sqlcek, parameter);
            if (count_cek == 0)
            {
                List<SqlParameter> parameter2 = new List<SqlParameter>();

                string sql = "INSERT INTO [dbo].[CaptureMobile_GroupMember] ([GroupID],[UserID],[CrtUsrID],[TsCrt],[ModUsrID],[TsMod],[ActiveFlag])VALUES(@groupid,@userid,@crtusrid,getdate(),@crtusrid,getdate(),'Y')";
                parameter2.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = UserID });
                parameter2.Add(new SqlParameter() { ParameterName = "@groupid", SqlDbType = SqlDbType.VarChar, Value = GroupID });
                parameter2.Add(new SqlParameter() { ParameterName = "@crtusrid", SqlDbType = SqlDbType.VarChar, Value = CrtUsrID });
                DAL.ExecuteNonQuery(sql, parameter2);
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "User was successfully added to the group";

            } else
            {
                ViewBag.Success = false;
                ModelState.AddModelError("", "Failed add user to group");
            }
            //

            return View("AddGroupMember", model);
        }

        //
        public ActionResult combo_grouplist()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            List<SqlParameter> parameterc = new List<SqlParameter>();
            string sqlcek = "select b.GroupName from [dbo].[CaptureMobile_GroupMember] a  left join[dbo].[CaptureMobile_Group] b on a.GroupID = b.GroupID where b.GroupName = 'IT' and UserID = @userid";
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });

            string groupname = (string)DAL.ExecuteScalar(sqlcek, parameterc);

            //
            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            string sql;
            if (groupname == "IT")
            {
                sql = "SELECT a.GroupID, a.GroupName  FROM[dbo].[CaptureMobile_Group] a";
            } else
            {
                sql = "SELECT a.GroupID, a.GroupName  FROM[dbo].[CaptureMobile_Group] a WHERE a.GroupID in (SELECT GroupID FROM[dbo].[CaptureMobile_GroupMember] WHERE UserID = @userid)";
            }
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            dt = DAL.GetDataTable(sql, parameter);

            List<Models.SelectItem> group = new List<Models.SelectItem>();

            foreach (DataRow row in dt.Rows)
            {
                group.Add(new Models.SelectItem
                {
                    value = (row["GroupID"]).ToString(),
                    text = row["GroupName"].ToString()
                });
            }

            return Json(group, JsonRequestBehavior.AllowGet);
        }

        public ActionResult combo_userlist()
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            string searchQuery = HttpContext.Request.QueryString["search"];

            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            DAL = new Models.IKSDAL("Server=SERVERIP;Database=DATABASENAME;User Id=DBUSER;Password=DBPW;");

            string sql = "select top 100 rtrim(ltrim(id_user)) as value, case when middle_name <> '' then rtrim(ltrim(id_employee)) + ' | ' + (ltrim(rtrim(first_name)) + ' ' + ltrim(rtrim(middle_name)) + ' ' + ltrim(rtrim(last_name)))  else rtrim(ltrim(id_employee)) + ' | ' + (ltrim(rtrim(first_name)) + ' ' + ltrim(rtrim(last_name))) end as text from[dbo].[employee] where (id_user like '%'+@keyword+'%') or(id_employee like '%'+@keyword+'%') or(ltrim(rtrim(first_name)) + ' ' + ltrim(rtrim(middle_name)) + ' ' + ltrim(rtrim(last_name))) like '%'+@keyword+'%' or(ltrim(rtrim(first_name)) + ' ' + ltrim(rtrim(last_name))) like '%'+@keyword+'%'";
            parameter.Add(new SqlParameter() { ParameterName = "@keyword", SqlDbType = SqlDbType.VarChar, Value = searchQuery });
            dt = DAL.GetDataTable(sql, parameter);
            List<Models.SelectItem> users = new List<Models.SelectItem>();

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new Models.SelectItem
                {
                    value = (row["value"]).ToString(),
                    text = row["text"].ToString()
                });
            }

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult json_directorylist(int id, int page)
        {
            //Check login
            if (Session["UserID"] != null)
            {
                user_login = (string)Session["UserID"];
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            //
            string userid = (string)Session["UserID"];
            int count_total;
            int pagenum = id;
            int pagesize = page;
            string sqlcount = "SELECT count(distinct DirName) from CaptureMobile where CrtUsrID IN ( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid)  GROUP BY UserID ) ";
            List<SqlParameter> parameterc = new List<SqlParameter>();
            parameterc.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.Int, Value = userid });

            count_total = (int)DAL.ExecuteScalar(sqlcount, parameterc);

            //ViewBag.PageNum = page;
            //ViewBag.MaxPageNum = roundedUpResult;
            //ViewBag.GroupID = id;
            //

            DataTable dt = new DataTable();
            List<SqlParameter> parameter = new List<SqlParameter>();

            string sql = "select DirName, max(TsCrt) TsCrt from CaptureMobile where CrtUsrID IN ( SELECT UserID FROM CaptureMobile_GroupMember WHERE GroupID IN(SELECT GroupID FROM CaptureMobile_GroupMember WHERE UserID = @userid)  GROUP BY UserID ) GROUP BY DirName order by max(TsCrt) desc OFFSET(@pagenum - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY";
            parameter.Add(new SqlParameter() { ParameterName = "@userid", SqlDbType = SqlDbType.VarChar, Value = user_login });
            parameter.Add(new SqlParameter() { ParameterName = "@pagenum", SqlDbType = SqlDbType.Int, Value = pagenum });
            parameter.Add(new SqlParameter() { ParameterName = "@pagesize", SqlDbType = SqlDbType.Int, Value = pagesize });

            dt = DAL.GetDataTable(sql, parameter);

            List<Models.DirName> directorylist = new List<Models.DirName>();
            Models.pagination pagination = new Models.pagination {
                totalItems = count_total,
                itemPerPage = pagesize,
                currentPage = pagenum,
            };

            foreach (DataRow row in dt.Rows)
            {
                directorylist.Add(new Models.DirName
                {
                    Name = row["DirName"].ToString(),
                    TsCrt = row["TsCrt"].ToString()
                });
            }

            Models.JsonDirectory JsonDirectory = new Models.JsonDirectory {
                data = directorylist,
                pagination = pagination
            };

            return Json(JsonDirectory, JsonRequestBehavior.AllowGet);
        }

    }
}