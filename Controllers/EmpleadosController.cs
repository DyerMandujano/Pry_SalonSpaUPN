﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using System.Linq;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly Conexion _context;

        public IActionResult Index()
        {

            return View();
        }
    }


}
