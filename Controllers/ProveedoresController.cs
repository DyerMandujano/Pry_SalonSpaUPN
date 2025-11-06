using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly Conexion _context;

        public ProveedoresController(Conexion context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, int? estado)
        {
            string? busquedaParam = string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim();
            ViewBag.Busqueda = busquedaParam;
            CargarCombos(estado);
            var proveedores = busquedaParam == null && estado == null
                ? await _context.Proveedor.FromSqlRaw("EXEC dbo.sp_Listar_Proveedores").ToListAsync()
                : await _context.Proveedor
                    .FromSqlInterpolated($"EXEC dbo.sp_Buscar_Proveedor @Busqueda = {busquedaParam}, @Estado = {estado}")
                    .ToListAsync();

            return View(proveedores);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarCombos();
            return View("_CrearProveedor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Proveedor model)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(model.Estado);
                return View("_CrearProveedor", model);
            }

            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@Nom_Prove", model.NomProve),
                    new SqlParameter("@Ruc", model.Ruc),
                    new SqlParameter("@Telefono", model.Telefono),
                    new SqlParameter("@Correo", model.Correo),
                    new SqlParameter("@Tipo_Proveedor", model.TipoProveedor),
                    new SqlParameter("@Estado", model.Estado)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_Crear_Proveedor @Nom_Prove, @Ruc, @Telefono, @Correo, @Tipo_Proveedor, @Estado",
                    parametros
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarCombos(model.Estado);
                return View("_CrearProveedor", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor == null) return NotFound();

            CargarCombos(proveedor.Estado);
            return View("_EditarProveedor", proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Proveedor model)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(model.Estado);
                return View("_EditarProveedor", model);
            }

            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@Id_Proveedor", model.IdProveedor),
                    new SqlParameter("@Nom_Prove", model.NomProve),
                    new SqlParameter("@Ruc", model.Ruc),
                    new SqlParameter("@Telefono", model.Telefono),
                    new SqlParameter("@Correo", model.Correo),
                    new SqlParameter("@Tipo_Proveedor", model.TipoProveedor),
                    new SqlParameter("@Estado", model.Estado)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_Editar_Proveedor @Id_Proveedor, @Nom_Prove, @Ruc, @Telefono, @Correo, @Tipo_Proveedor, @Estado",
                    parametros
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarCombos(model.Estado);
                return View("_EditarProveedor", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Estado_Proveedor @IdProveedor = {0}", id);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        private void CargarCombos(int? estadoSeleccionado = null)
        {
            var estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos" },
                new SelectListItem { Value = "1", Text = "Activo" },
                new SelectListItem { Value = "0", Text = "Inactivo" }
            };

            ViewBag.Estados = new SelectList(estados, "Value", "Text", estadoSeleccionado?.ToString());
        }
    }
}
