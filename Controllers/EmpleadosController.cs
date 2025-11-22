using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly Conexion _context;

        public EmpleadosController(Conexion context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string busqueda,
            int? idPerfil,
            int? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string orden = "DESC",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var listaEmpleados = new List<Empleado>();
            int totalRegistros = 0;
            int totalActivos = 0;
            int totalRetirados = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_Empleados", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Busqueda", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);
                command.Parameters.AddWithValue("@Id_Perfil", idPerfil ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Estado", estado ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaDesde", fechaDesde ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaHasta", fechaHasta ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Orden", orden);
                command.Parameters.AddWithValue("@NumeroPagina", pageNumber);
                command.Parameters.AddWithValue("@TamanoPagina", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    try
                    {
                        var empleado = new Empleado
                        {
                            IdEmpleado = reader.GetInt32(reader.GetOrdinal("Id_Empleado")),
                            IdPersona = reader.GetInt32(reader.GetOrdinal("Id_persona")),
                            IdPerfil = reader.GetInt32(reader.GetOrdinal("Id_Perfil")),
                            Sueldo = reader.GetDecimal(reader.GetOrdinal("Sueldo")),
                            FechaRetiro = reader["Fecha_retiro"] != DBNull.Value
                                ? DateOnly.FromDateTime(Convert.ToDateTime(reader["Fecha_retiro"]))
                                : null
                        };

                        empleado.IdPersonaNavigation = new Persona
                        {
                            IdPersona = reader.GetInt32(reader.GetOrdinal("Id_persona")),
                            Nombres = reader["Nombres"]?.ToString() ?? "",
                            Apellidos = reader["Apellidos"]?.ToString() ?? "",
                            Telefono = reader["Telefono"]?.ToString() ?? "",
                            Dni = reader["Dni"]?.ToString() ?? "",
                            Genero = reader["Genero"]?.ToString() ?? "",
                            FechaNacimiento = reader["Fecha_nacimiento"] != DBNull.Value
                                ? DateOnly.FromDateTime(Convert.ToDateTime(reader["Fecha_nacimiento"]))
                                : DateOnly.MinValue,
                            FechaRegistro = reader["Fecha_Registro"] != DBNull.Value
                                ? Convert.ToDateTime(reader["Fecha_Registro"])
                                : DateTime.MinValue,
                            Estado = reader["Estado_Persona"] != DBNull.Value
                                ? Convert.ToInt32(reader["Estado_Persona"])
                                : 0
                        };

                        empleado.IdPerfilNavigation = new Perfil
                        {
                            IdPerfil = reader.GetInt32(reader.GetOrdinal("Id_Perfil")),
                            NombrePerfil = reader["Nombre_perfil"]?.ToString() ?? "",
                            Estado = 1
                        };

                        if (totalRegistros == 0)
                        {
                            totalRegistros = reader["Total_Registros"] != DBNull.Value
                                ? Convert.ToInt32(reader["Total_Registros"])
                                : 0;
                            totalActivos = reader["Total_Activos"] != DBNull.Value
                                ? Convert.ToInt32(reader["Total_Activos"])
                                : 0;
                            totalRetirados = reader["Total_Retirados"] != DBNull.Value
                                ? Convert.ToInt32(reader["Total_Retirados"])
                                : 0;
                            totalPaginas = reader["Total_Paginas"] != DBNull.Value
                                ? Convert.ToInt32(reader["Total_Paginas"])
                                : 0;
                        }

                        listaEmpleados.Add(empleado);
                    }
                    catch (Exception innerEx)
                    {
                        TempData["Mensaje"] = $"Error al leer registro: {innerEx.Message}";
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar empleados: {ex.Message}";
            }

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalActivos = totalActivos;
            ViewBag.TotalRetirados = totalRetirados;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            ViewBag.Busqueda = busqueda;
            ViewBag.IdPerfil = idPerfil;
            ViewBag.Estado = estado;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            ViewBag.Orden = orden;

            CargarListasDesplegables(idPerfil);

            return View(listaEmpleados);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarListasDesplegables();

            ViewBag.Generos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Masculino", Value = "Masculino" },
                new SelectListItem { Text = "Femenino", Value = "Femenino" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            return View("_CrearEmpleados");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            string Nombres,
            string Apellidos,
            string Telefono,
            string Dni,
            string Genero,
            DateTime Fecha_nacimiento,
            int Id_Perfil,
            decimal Sueldo)
        {
            if (string.IsNullOrWhiteSpace(Nombres) || string.IsNullOrWhiteSpace(Apellidos) ||
                string.IsNullOrWhiteSpace(Dni) || Id_Perfil == 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                CargarListasDesplegables(Id_Perfil);
                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro" }
                };
                return View("_CrearEmpleados");
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Crear_Empleado", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Nombres", Nombres);
                command.Parameters.AddWithValue("@Apellidos", Apellidos);
                command.Parameters.AddWithValue("@Telefono", Telefono);
                command.Parameters.AddWithValue("@Dni", Dni);
                command.Parameters.AddWithValue("@Genero", Genero);
                command.Parameters.AddWithValue("@Fecha_nacimiento", Fecha_nacimiento);
                command.Parameters.AddWithValue("@Id_Perfil", Id_Perfil);
                command.Parameters.AddWithValue("@Sueldo", Sueldo);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    if (reader.GetSchemaTable().Select("ColumnName = 'Error'").Length > 0 &&
                        reader["Error"] != DBNull.Value)
                    {
                        ViewBag.Error = reader["Error"]?.ToString();
                        CargarListasDesplegables(Id_Perfil);
                        ViewBag.Generos = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Masculino", Value = "Masculino" },
                            new SelectListItem { Text = "Femenino", Value = "Femenino" },
                            new SelectListItem { Text = "Otro", Value = "Otro" }
                        };
                        return View("_CrearEmpleados");
                    }

                    TempData["Mensaje"] = "Empleado creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Mensaje"] = "Empleado creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear el empleado: {ex.Message}";
                CargarListasDesplegables(Id_Perfil);
                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro" }
                };
                return View("_CrearEmpleados");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(
            int id,
            string? busqueda,
            int? idPerfil,
            int? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? orden,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                if (id == 0)
                {
                    TempData["Mensaje"] = "ID de empleado inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var empleado = await _context.Empleados
                    .Include(e => e.IdPersonaNavigation)
                    .Include(e => e.IdPerfilNavigation)
                    .FirstOrDefaultAsync(e => e.IdEmpleado == id);

                if (empleado == null)
                {
                    TempData["Mensaje"] = $"No se encontró el empleado con ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                if (empleado.IdPersonaNavigation == null)
                {
                    TempData["Mensaje"] = "Error: Datos de persona del empleado no encontrados.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.IdEmpleado = empleado.IdEmpleado;
                ViewBag.EstadoActual = empleado.IdPersonaNavigation.Estado;

                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino", Selected = empleado.IdPersonaNavigation.Genero == "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino", Selected = empleado.IdPersonaNavigation.Genero == "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro", Selected = empleado.IdPersonaNavigation.Genero == "Otro" }
                };

                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Activo", Value = "1", Selected = empleado.IdPersonaNavigation.Estado == 1 },
                    new SelectListItem { Text = "Inactivo", Value = "0", Selected = empleado.IdPersonaNavigation.Estado == 0 }
                };

                ViewBag.ReturnBusqueda = busqueda;
                ViewBag.ReturnIdPerfil = idPerfil;
                ViewBag.ReturnEstado = estado;
                ViewBag.ReturnFechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
                ViewBag.ReturnFechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
                ViewBag.ReturnOrden = orden;
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                CargarListasDesplegables(empleado.IdPerfil);

                return View("~/Views/Empleados/_EditarEmpleados.cshtml", empleado);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar el empleado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int Id_Empleado,
            string Nombres,
            string Apellidos,
            string Telefono,
            string Dni,
            string Genero,
            DateTime Fecha_nacimiento,
            int Id_Perfil,
            decimal Sueldo,
            int Estado,
            string? busqueda,
            int? idPerfil,
            int? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? orden,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(Nombres) || string.IsNullOrWhiteSpace(Apellidos) ||
                string.IsNullOrWhiteSpace(Dni) || Id_Perfil == 0)
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                ViewBag.IdEmpleado = Id_Empleado;
                ViewBag.EstadoActual = Estado;
                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino", Selected = Genero == "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino", Selected = Genero == "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro", Selected = Genero == "Otro" }
                };
                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Activo", Value = "1", Selected = Estado == 1 },
                    new SelectListItem { Text = "Inactivo", Value = "0", Selected = Estado == 0 }
                };

                var empleadoError = new Empleado
                {
                    IdEmpleado = Id_Empleado,
                    IdPerfil = Id_Perfil,
                    Sueldo = Sueldo,
                    IdPersonaNavigation = new Persona
                    {
                        Nombres = Nombres,
                        Apellidos = Apellidos,
                        Telefono = Telefono,
                        Dni = Dni,
                        Genero = Genero,
                        FechaNacimiento = DateOnly.FromDateTime(Fecha_nacimiento),
                        Estado = Estado
                    }
                };

                CargarListasDesplegables(Id_Perfil);
                return View("~/Views/Empleados/_EditarEmpleados.cshtml", empleadoError);
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Editar_Empleado", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Empleado", Id_Empleado);
                command.Parameters.AddWithValue("@Nombres", Nombres);
                command.Parameters.AddWithValue("@Apellidos", Apellidos);
                command.Parameters.AddWithValue("@Telefono", Telefono);
                command.Parameters.AddWithValue("@Dni", Dni);
                command.Parameters.AddWithValue("@Genero", Genero);
                command.Parameters.AddWithValue("@Fecha_nacimiento", Fecha_nacimiento);
                command.Parameters.AddWithValue("@Id_Perfil", Id_Perfil);
                command.Parameters.AddWithValue("@Sueldo", Sueldo);
                command.Parameters.AddWithValue("@Estado", Estado);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    if (reader.GetSchemaTable().Select("ColumnName = 'Error'").Length > 0 &&
                        reader["Error"] != DBNull.Value)
                    {
                        ViewBag.Error = reader["Error"]?.ToString();
                        ViewBag.IdEmpleado = Id_Empleado;
                        ViewBag.EstadoActual = Estado;
                        ViewBag.Generos = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Masculino", Value = "Masculino", Selected = Genero == "Masculino" },
                            new SelectListItem { Text = "Femenino", Value = "Femenino", Selected = Genero == "Femenino" },
                            new SelectListItem { Text = "Otro", Value = "Otro", Selected = Genero == "Otro" }
                        };
                        ViewBag.Estados = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Activo", Value = "1", Selected = Estado == 1 },
                            new SelectListItem { Text = "Inactivo", Value = "0", Selected = Estado == 0 }
                        };

                        var empleadoError = new Empleado
                        {
                            IdEmpleado = Id_Empleado,
                            IdPerfil = Id_Perfil,
                            Sueldo = Sueldo,
                            IdPersonaNavigation = new Persona
                            {
                                Nombres = Nombres,
                                Apellidos = Apellidos,
                                Telefono = Telefono,
                                Dni = Dni,
                                Genero = Genero,
                                FechaNacimiento = DateOnly.FromDateTime(Fecha_nacimiento),
                                Estado = Estado
                            }
                        };

                        CargarListasDesplegables(Id_Perfil);
                        return View("~/Views/Empleados/_EditarEmpleados.cshtml", empleadoError);
                    }

                    TempData["Mensaje"] = "Empleado actualizado correctamente.";
                }
                else
                {
                    TempData["Mensaje"] = "Empleado actualizado correctamente.";
                }

                return RedirectToAction(nameof(Index), new
                {
                    busqueda = busqueda,
                    idPerfil = idPerfil,
                    estado = estado,
                    fechaDesde = fechaDesde,
                    fechaHasta = fechaHasta,
                    orden = orden,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar el empleado: {ex.Message}";
                ViewBag.IdEmpleado = Id_Empleado;
                ViewBag.EstadoActual = Estado;
                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino", Selected = Genero == "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino", Selected = Genero == "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro", Selected = Genero == "Otro" }
                };
                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Activo", Value = "1", Selected = Estado == 1 },
                    new SelectListItem { Text = "Inactivo", Value = "0", Selected = Estado == 0 }
                };

                var empleadoError = new Empleado
                {
                    IdEmpleado = Id_Empleado,
                    IdPerfil = Id_Perfil,
                    Sueldo = Sueldo,
                    IdPersonaNavigation = new Persona
                    {
                        Nombres = Nombres,
                        Apellidos = Apellidos,
                        Telefono = Telefono,
                        Dni = Dni,
                        Genero = Genero,
                        FechaNacimiento = DateOnly.FromDateTime(Fecha_nacimiento),
                        Estado = Estado
                    }
                };

                CargarListasDesplegables(Id_Perfil);
                return View("~/Views/Empleados/_EditarEmpleados.cshtml", empleadoError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string accion,
            string? busqueda,
            int? idPerfil,
            int? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? orden,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Estado_Empleado", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Empleado", id);
                command.Parameters.AddWithValue("@Accion", accion);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    if (reader.GetSchemaTable().Select("ColumnName = 'Error'").Length > 0 &&
                        reader["Error"] != DBNull.Value)
                    {
                        TempData["Mensaje"] = $"Error: {reader["Error"]}";
                    }
                    else
                    {
                        TempData["Mensaje"] = reader["Mensaje"]?.ToString() ?? "Estado actualizado correctamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                busqueda = busqueda,
                idPerfil = idPerfil,
                estado = estado,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                orden = orden,
                pageNumber = pageNumber,
                pageSize = pageSize
            });
        }

        private void CargarListasDesplegables(int? idPerfilSeleccionado = null)
        {
            ViewData["Perfiles"] = new SelectList(
                _context.Perfils.AsNoTracking().ToList(),
                "IdPerfil",
                "NombrePerfil",
                idPerfilSeleccionado
            );
        }
    }
}
