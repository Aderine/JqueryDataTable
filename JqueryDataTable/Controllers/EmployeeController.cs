using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JqueryDataTable.Models;
using System.Linq.Dynamic;


namespace JqueryDataTable.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetList()
        { 
            //server side params
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDir = Request["order[0][dir]"];

            using (DBModels db = new DBModels())
            {
                var empList = db.Employees.ToList<Employee>();

                int totalRows = empList.Count;

                //searching
                if(!string.IsNullOrEmpty(searchValue))
                {
                    empList = empList.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())||
                    x.Position.ToLower().Contains(searchValue.ToLower()) ||
                    x.Office.ToLower().Contains(searchValue.ToLower())||
                    x.Age.ToString().Contains(searchValue.ToLower()) ||
                    x.Salary.ToString().Contains(searchValue.ToLower())
                    ).ToList<Employee>();   
                }
                int totalRowsAfterFiltering = empList.Count;

                //sorting
                empList = empList.OrderBy(sortColName + " " + sortDir).ToList<Employee>();

                //paging
                empList = empList.Skip(start).Take(length).ToList<Employee>();

                return Json(new {data = empList, draw = Request["draw"], recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFiltering}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}