using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<IActionResult> Index(string? busqueda, int categoriaSeleccionada = 0)
        {
            var categorias = await _context.Categoria
                .Where(c => c.Estado == 1)
                .ToListAsync();

            categorias.Insert(0, new Categoria { IdCategoria = 0, NomCate = "-- Todas --" });

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NomCate", categoriaSeleccionada);
            ViewBag.CategoriaSeleccionada = categoriaSeleccionada;
            ViewBag.Busqueda = busqueda;

            var proveedores = await _context.Proveedor
                .FromSqlRaw("EXEC sp_Buscar_Proveedor @p0, @p1", busqueda ?? "", categoriaSeleccionada)
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
            if (!model.IdCategoria.HasValue || model.IdCategoria <= 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría válida");

            if (!ModelState.IsValid)
            {
                CargarCombos(model.IdCategoria ?? 0, model.Estado);
                return View("_CrearProveedor", model);
            }

            try
            {
                _context.Proveedor.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarCombos(model.IdCategoria ?? 0, model.Estado);
                return View("_CrearProveedor", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor == null) return NotFound();

            CargarCombos(proveedor.IdCategoria ?? 0, proveedor.Estado);
            return View("_EditarProveedor", proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Proveedor model)
        {
            if (!model.IdCategoria.HasValue || model.IdCategoria <= 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría válida");

            if (!ModelState.IsValid)
            {
                CargarCombos(model.IdCategoria ?? 0, model.Estado);
                return View("_EditarProveedor", model);
            }

            try
            {
                _context.Proveedor.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                CargarCombos(model.IdCategoria ?? 0, model.Estado);
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
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private void CargarCombos(int categoriaSeleccionada = 0, int estadoSeleccionado = 1)
        {
            var categorias = _context.Categoria
                .Where(c => c.Estado == 1)
                .Select(c => new { c.IdCategoria, c.NomCate })
                .ToList();

            categorias.Insert(0, new { IdCategoria = 0, NomCate = "-- Seleccione una categoría --" });
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "NomCate", categoriaSeleccionada);

            var estados = new List<object>
            {
                new { Valor = 1, Texto = "Activo" },
                new { Valor = 0, Texto = "Inactivo" }
            };

            ViewBag.Estados = new SelectList(estados, "Valor", "Texto", estadoSeleccionado);
        }
    }
}
