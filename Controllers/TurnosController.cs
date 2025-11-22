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
    public class TurnosController : Controller
    {
        private readonly Conexion _context;

        public TurnosController(Conexion context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string busqueda, int? idPerfil, int? estado,
                                               int pageNumber = 1, int pageSize = 10)
        {
            var listaEmpleados = new List<Empleado>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_EmpleadoHorario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Busqueda", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);
                command.Parameters.AddWithValue("@Id_Perfil", idPerfil ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Estado", estado ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var empleado = new Empleado
                    {
                        IdEmpleado = reader.GetInt32(reader.GetOrdinal("Id_Empleado")),
                        IdPersonaNavigation = new Persona
                        {
                            Nombres = reader["Nombres"]?.ToString() ?? "",
                            Apellidos = reader["Apellidos"]?.ToString() ?? ""
                        },
                        IdPerfilNavigation = new Perfil
                        {
                            NombrePerfil = reader["Nombre_perfil"]?.ToString() ?? ""
                        }
                    };

                    var horario = new Horario
                    {
                        IdHorario = reader["Id_Horario"] != DBNull.Value ? Convert.ToInt32(reader["Id_Horario"]) : 0,
                        HoraInicio = reader["Hora_Inicio"] != DBNull.Value ? (TimeSpan?)reader["Hora_Inicio"] : null,
                        HoraFin = reader["Hora_Fin"] != DBNull.Value ? (TimeSpan?)reader["Hora_Fin"] : null,
                        DiasSemana = reader["Dias_Semana"]?.ToString() ?? "",
                        Estado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0
                    };

                    var empHorario = new EmpleadoHorario
                    {
                        IdHorarioNavigation = horario
                    };

                    empleado.EmpleadoHorarios.Add(empHorario);
                    listaEmpleados.Add(empleado);
                }

                // Si el SP retorna TotalRegistros y TotalPaginas en un segundo result set
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    totalRegistros = reader["TotalRegistros"] != DBNull.Value ? Convert.ToInt32(reader["TotalRegistros"]) : 0;
                    totalPaginas = reader["TotalPaginas"] != DBNull.Value ? Convert.ToInt32(reader["TotalPaginas"]) : 0;
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar horarios: {ex.Message}";
                return View(new List<Empleado>());
            }

            ViewBag.Busqueda = busqueda;
            ViewBag.IdPerfil = idPerfil;
            ViewBag.Estado = estado;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = totalPaginas;

            CargarCombos(idPerfil);

            return View(listaEmpleados);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.EmpleadoEncontrado = null;
            ViewBag.DniBuscado = null;
            ViewBag.Error = null;
            return View("_CrearEmpleadoHorario");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarEmpleadoPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                ViewBag.Error = "Debe ingresar un DNI válido.";
                return View("_CrearEmpleadoHorario");
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Buscar_Empleado_Por_DNI", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@DNI", dni.Trim());

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var tieneHorario = Convert.ToInt32(reader["TieneHorario"]);

                    if (tieneHorario == 1)
                    {
                        ViewBag.Error = "Este empleado ya tiene un horario asignado. Debe editarlo desde la lista.";
                        ViewBag.DniBuscado = dni;
                    }
                    else
                    {
                        ViewBag.EmpleadoEncontrado = new
                        {
                            IdEmpleado = reader.GetInt32(reader.GetOrdinal("Id_Empleado")),
                            Nombres = reader["Nombres"]?.ToString(),
                            Apellidos = reader["Apellidos"]?.ToString(),
                            DNI = reader["DNI"]?.ToString(),
                            Perfil = reader["Nombre_perfil"]?.ToString()
                        };
                        ViewBag.DniBuscado = dni;
                    }
                }
                else
                {
                    ViewBag.Error = "No se encontró ningún empleado activo con el DNI ingresado.";
                    ViewBag.DniBuscado = dni;
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error al buscar empleado: {ex.Message}";
            }

            return View("_CrearEmpleadoHorario");
        }

        [HttpGet]
        public async Task<IActionResult> Asignar(int idEmpleado, string busqueda, int? idPerfil, int? estado,
                                         int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (idEmpleado == 0)
                {
                    TempData["Mensaje"] = "ID de empleado inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var empleado = await _context.Empleados
                    .Include(e => e.IdPersonaNavigation)
                    .Include(e => e.IdPerfilNavigation)
                    .Include(e => e.EmpleadoHorarios)
                        .ThenInclude(eh => eh.IdHorarioNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

                if (empleado == null)
                {
                    TempData["Mensaje"] = $"No se encontró el empleado con ID {idEmpleado}.";
                    return RedirectToAction(nameof(Index));
                }

                var empHorario = empleado.EmpleadoHorarios?.FirstOrDefault();
                var horario = empHorario?.IdHorarioNavigation;

                ViewBag.IdEmpleado = empleado.IdEmpleado;
                ViewBag.IdHorario = horario?.IdHorario ?? 0;
                ViewBag.NombreCompleto = $"{empleado.IdPersonaNavigation?.Nombres} {empleado.IdPersonaNavigation?.Apellidos}";
                ViewBag.Perfil = empleado.IdPerfilNavigation?.NombrePerfil;
                ViewBag.HoraInicio = horario?.HoraInicio?.ToString(@"hh\:mm") ?? "";
                ViewBag.HoraFin = horario?.HoraFin?.ToString(@"hh\:mm") ?? "";
                ViewBag.DiasSemana = horario?.DiasSemana ?? "";
                ViewBag.Estado = horario?.Estado ?? 1;

                ViewBag.ReturnBusqueda = busqueda;
                ViewBag.ReturnIdPerfil = idPerfil;
                ViewBag.ReturnEstado = estado;
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                return View("_AsignarEmpleadoHorario");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar el horario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(int IdEmpleado, int IdHorario, string HoraInicio, string HoraFin,
                                                string Dia_1, string Dia_2, string Dia_3, string Dia_4,
                                                string Dia_5, string Dia_6, string Dia_7,
                                                string busqueda, int? idPerfil, int? estado,
                                                int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(HoraInicio) || string.IsNullOrWhiteSpace(HoraFin))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                return View("_AsignarHorario");
            }

            // Construir el array de días seleccionados
            var diasSeleccionados = new List<string>();
            if (!string.IsNullOrEmpty(Dia_1)) diasSeleccionados.Add("1");
            if (!string.IsNullOrEmpty(Dia_2)) diasSeleccionados.Add("2");
            if (!string.IsNullOrEmpty(Dia_3)) diasSeleccionados.Add("3");
            if (!string.IsNullOrEmpty(Dia_4)) diasSeleccionados.Add("4");
            if (!string.IsNullOrEmpty(Dia_5)) diasSeleccionados.Add("5");
            if (!string.IsNullOrEmpty(Dia_6)) diasSeleccionados.Add("6");
            if (!string.IsNullOrEmpty(Dia_7)) diasSeleccionados.Add("7");

            if (diasSeleccionados.Count == 0)
            {
                ViewBag.Error = "Debe seleccionar al menos un día de la semana.";
                return View("_AsignarHorario");
            }

            string DiasSemana = "[" + string.Join(",", diasSeleccionados) + "]";

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                // Si IdHorario es 0, es creación, sino es edición
                if (IdHorario == 0)
                {
                    using var command = new SqlCommand("sp_insertar_EmpleadoHorario", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Id_Empleado", IdEmpleado);
                    command.Parameters.AddWithValue("@Hora_Inicio", TimeSpan.Parse(HoraInicio));
                    command.Parameters.AddWithValue("@Hora_Fin", TimeSpan.Parse(HoraFin));
                    command.Parameters.AddWithValue("@Dias_Semana", DiasSemana);

                    await command.ExecuteNonQueryAsync();
                    TempData["Mensaje"] = "Horario asignado correctamente.";
                }
                else
                {
                    using var command = new SqlCommand("sp_editar_EmpleadoHorario", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Id_Horario", IdHorario);
                    command.Parameters.AddWithValue("@Hora_Inicio", TimeSpan.Parse(HoraInicio));
                    command.Parameters.AddWithValue("@Hora_Fin", TimeSpan.Parse(HoraFin));
                    command.Parameters.AddWithValue("@Dias_Semana", DiasSemana);

                    await command.ExecuteNonQueryAsync();
                    TempData["Mensaje"] = "Horario actualizado correctamente.";
                }

                return RedirectToAction(nameof(Index), new
                {
                    busqueda,
                    idPerfil,
                    estado,
                    pageNumber,
                    pageSize
                });
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View("_AsignarHorario");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int idHorario, string busqueda, int? idPerfil, int? estado,
                                                        int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_estado_EmpleadoHorario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Horario", idHorario);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var nuevoEstado = reader.GetInt32(reader.GetOrdinal("NuevoEstado"));
                    var mensaje = reader["Mensaje"]?.ToString();

                    TempData["Mensaje"] = $"{mensaje} Estado: {(nuevoEstado == 1 ? "Activo" : "Inactivo")}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                busqueda,
                idPerfil,
                estado,
                pageNumber,
                pageSize
            });
        }


        private void CargarCombos(int? idPerfilSeleccionado = null)
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
