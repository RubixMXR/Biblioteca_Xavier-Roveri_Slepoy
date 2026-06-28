using System;
using System.Linq;
using System.Collections.Generic;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca
{
    class Program
    {
        private static BibliotecaContext _context;

        static void Main(string[] args)
        {
            _context = new BibliotecaContext();

            bool continuar = true;
            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("SISTEMA DE BIBLIOTECA");
                Console.WriteLine("1. Registrar Prestamo");
                Console.WriteLine("2. Registrar Devolucion");
                Console.WriteLine("3. Registrar Reserva");
                Console.WriteLine("4. Ver Detalle de Socio");
                Console.WriteLine("5. Libros mas prestados");
                Console.WriteLine("6. Socios con multas pendientes");
                Console.WriteLine("7. Prestamos vencidos");
                Console.WriteLine("8. Disponibilidad de libro");
                Console.WriteLine("9. Historial de socio");
                Console.WriteLine("10. Ranking de socios");
                Console.WriteLine("0. Salir");
                Console.Write("\nOpcion: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1": RegistrarPrestamo(); break;
                    case "2": RegistrarDevolucion(); break;
                    case "3": RegistrarReserva(); break;
                    case "4": VerDetalleSocio(); break;
                    case "5": LibrosMasPrestados(); break;
                    case "6": SociosConMultas(); break;
                    case "7": PrestamosVencidos(); break;
                    case "8": DisponibilidadLibro(); break;
                    case "9": HistorialSocio(); break;
                    case "10": RankingSocios(); break;
                    case "0": continuar = false; break;
                    default: Console.WriteLine("Opcion no valida"); break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static void RegistrarPrestamo()
        {
            Console.WriteLine("REGISTRAR PRESTAMO");
            Console.Write("Ingrese Nro de Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int nroSocio))
            {
                Console.WriteLine("Numero de socio invalido");
                return;
            }

            var socio = _context.Socio
                .Include(s => s.TiposDeSocios)
                .Include(s => s.Prestamos.Where(p => p.Estado == 1))
                .FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado");
                return;
            }

            if (socio.Activo != 1)
            {
                Console.WriteLine("El socio esta inactivo");
                return;
            }

            var multasPendientes = _context.Multas.Where(m => m.SocioId == socio.NroSocio && !m.Pagada).Sum(m => m.Monto);
            if (multasPendientes > 0)
            {
                Console.WriteLine($"El socio tiene multas pendientes por ${multasPendientes}");
                return;
            }

            int prestamosActivos = socio.Prestamos.Count(p => p.Estado == 1);
            if (prestamosActivos >= socio.TiposDeSocios.MaxLibros)
            {
                Console.WriteLine($"Limite maximo de libros: {socio.TiposDeSocios.MaxLibros}");
                return;
            }

            Console.Write("Ingrese titulo o autor del libro: ");
            string busqueda = Console.ReadLine();

            var libros = _context.Libros
                .Where(l => l.Titulo.Contains(busqueda) || l.Autor.Contains(busqueda))
                .ToList();

            if (!libros.Any())
            {
                Console.WriteLine("No se encontraron libros");
                return;
            }

            Console.WriteLine("\nLibros encontrados:");
            for (int i = 0; i < libros.Count; i++)
            {
                var libro = libros[i];
                int prestados = _context.Prestamos.Count(p => p.Libro == libro.ISBN && p.Estado == 1);
                int disponibles = libro.CantidadCopias - prestados;
                Console.WriteLine($"{i + 1}. {libro.Titulo} - {libro.Autor} (Disponibles: {disponibles})");
            }

            Console.Write("\nSeleccione libro: ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > libros.Count)
            {
                Console.WriteLine("Seleccion invalida");
                return;
            }

            var libroSeleccionado = libros[seleccion - 1];

            int copiasPrestadas = _context.Prestamos.Count(p => p.Libro == libroSeleccionado.ISBN && p.Estado == 1);
            int copiasDisponibles = libroSeleccionado.CantidadCopias - copiasPrestadas;

            if (copiasDisponibles <= 0)
            {
                Console.WriteLine("No hay copias disponibles.");
                Console.Write("¿Desea reservar el libro? (S/N): ");
                if (Console.ReadLine().ToUpper() == "S")
                {
                    CrearReserva(socio.NroSocio, libroSeleccionado.ISBN);
                }
                return;
            }

            DateTime fechaPrestamo = DateTime.Now;
            DateTime fechaVencimiento = fechaPrestamo.AddDays(socio.TiposDeSocios.DiasPrestamo);

            var prestamo = new Prestamo
            {
                Socio = socio.NroSocio,
                Libro = libroSeleccionado.ISBN,
                FechaPrestamo = fechaPrestamo,
                FechaVencimiento = fechaVencimiento,
                Estado = 1
            };

            _context.Prestamos.Add(prestamo);
            _context.SaveChanges();

            Console.WriteLine($"\nPrestamo registrado");
            Console.WriteLine($"Libro: {libroSeleccionado.Titulo}");
            Console.WriteLine($"Fecha vencimiento: {fechaVencimiento:dd/MM/yyyy}");

            var reservaPendiente = _context.Reservas
                .Include(r => r.SocioNavigation)
                .Where(r => r.Libro == libroSeleccionado.ISBN && r.Estado == 1)
                .OrderBy(r => r.FechaReserva)
                .FirstOrDefault();

            if (reservaPendiente != null)
            {
                reservaPendiente.Estado = 2;
                _context.SaveChanges();
                Console.WriteLine($"\nReserva de {reservaPendiente.SocioNavigation.Nombre} {reservaPendiente.SocioNavigation.Apellido} cumplida");
            }
        }

        static void RegistrarDevolucion()
        {
            Console.WriteLine("REGISTRAR DEVOLUCION");
            Console.Write("Ingrese Nro de Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int nroSocio))
            {
                Console.WriteLine("Numero de socio invalido");
                return;
            }

            var socio = _context.Socio
                .Include(s => s.Prestamos.Where(p => p.Estado == 1))
                .ThenInclude(p => p.LibroNavigation)
                .FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado");
                return;
            }

            var prestamosActivos = socio.Prestamos.Where(p => p.Estado == 1).ToList();

            if (!prestamosActivos.Any())
            {
                Console.WriteLine("No tiene prestamos activos");
                return;
            }

            Console.WriteLine("\nPrestamos activos:");
            for (int i = 0; i < prestamosActivos.Count; i++)
            {
                var p = prestamosActivos[i];
                Console.WriteLine($"{i + 1}. {p.LibroNavigation.Titulo} - Vence: {p.FechaVencimiento:dd/MM/yyyy}");
            }

            Console.Write("\nSeleccione prestamo: ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > prestamosActivos.Count)
            {
                Console.WriteLine("Seleccion invalida");
                return;
            }

            var prestamo = prestamosActivos[seleccion - 1];
            prestamo.FechaDevolucion = DateTime.Now;

            decimal multa = 0;
            if (prestamo.FechaDevolucion > prestamo.FechaVencimiento)
            {
                int diasDemora = (prestamo.FechaDevolucion.Value - prestamo.FechaVencimiento).Days;
                var tipoSocio = _context.TiposSocios.Find(socio.TipoSocio);
                multa = diasDemora * tipoSocio.MultaPorDia;

                if (multa > 0)
                {
                    var nuevaMulta = new Multa
                    {
                        SocioId = socio.NroSocio,
                        PrestamoId = prestamo.Id,
                        Monto = multa,
                        FechaGeneracion = DateTime.Now,
                        Pagada = false
                    };
                    _context.Multas.Add(nuevaMulta);
                }
            }

            prestamo.Estado = 2;
            _context.SaveChanges();

            Console.WriteLine($"\nDevolucion registrada");
            if (multa > 0)
            {
                Console.WriteLine($"Multa: ${multa} ({(prestamo.FechaDevolucion.Value - prestamo.FechaVencimiento).Days} dias de atraso)");
            }

            var reservaPendiente = _context.Reservas
                .Include(r => r.SocioNavigation)
                .Where(r => r.Libro == prestamo.Libro && r.Estado == 1)
                .OrderBy(r => r.FechaReserva)
                .FirstOrDefault();

            if (reservaPendiente != null)
            {
                Console.WriteLine($"\nEl libro esta reservado por {reservaPendiente.SocioNavigation.Nombre} {reservaPendiente.SocioNavigation.Apellido}");
            }
        }

        static void RegistrarReserva()
        {
            Console.WriteLine("REGISTRAR RESERVA");
            Console.Write("Ingrese Nro de Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int nroSocio))
            {
                Console.WriteLine("Numero de socio invalido");
                return;
            }

            var socio = _context.Socio.FirstOrDefault(s => s.NroSocio == nroSocio);
            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado");
                return;
            }

            if (socio.Activo != 1)
            {
                Console.WriteLine("El socio esta inactivo");
                return;
            }

            Console.Write("Ingrese ISBN del libro: ");
            string isbn = Console.ReadLine();

            var libro = _context.Libros.FirstOrDefault(l => l.ISBN == isbn);
            if (libro == null)
            {
                Console.WriteLine("Libro no encontrado");
                return;
            }

            CrearReserva(nroSocio, isbn);
        }

        static void CrearReserva(int socioId, string libroIsbn)
        {
            var reservaExistente = _context.Reservas
                .FirstOrDefault(r => r.Socio == socioId && r.Libro == libroIsbn && r.Estado == 1);

            if (reservaExistente != null)
            {
                Console.WriteLine("Ya tiene una reserva activa para este libro");
                return;
            }

            var reserva = new Reserva
            {
                Socio = socioId,
                Libro = libroIsbn,
                FechaReserva = DateTime.Now,
                Estado = 1
            };

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            Console.WriteLine("Reserva registrada");
        }

        static void VerDetalleSocio()
        {
            Console.WriteLine("DETALLE DE SOCIO");
            Console.Write("Ingrese Nro de Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int nroSocio))
            {
                Console.WriteLine("Numero de socio invalido");
                return;
            }

            var socio = _context.Socio
                .Include(s => s.TiposDeSocios)
                .Include(s => s.Prestamos)
                .ThenInclude(p => p.LibroNavigation)
                .Include(s => s.Reservas)
                .ThenInclude(r => r.LibroNavigation)
                .FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado");
                return;
            }

            var multas = _context.Multas.Where(m => m.SocioId == socio.NroSocio).ToList();
            decimal totalMultas = multas.Where(m => !m.Pagada).Sum(m => m.Monto);

            Console.WriteLine($"\n{socio.Nombre} {socio.Apellido}");
            Console.WriteLine($"Email: {socio.Email}");
            Console.WriteLine($"Tipo: {socio.TiposDeSocios.Tipo}");
            Console.WriteLine($"Estado: {(socio.Activo == 1 ? "Activo" : "Inactivo")}");
            Console.WriteLine($"Multas pendientes: ${totalMultas}");
            Console.WriteLine($"Limite de libros: {socio.TiposDeSocios.MaxLibros}");

            Console.WriteLine("\nPRESTAMOS ACTIVOS");
            var prestamosActivos = socio.Prestamos.Where(p => p.Estado == 1).ToList();
            if (prestamosActivos.Any())
            {
                foreach (var p in prestamosActivos)
                {
                    Console.WriteLine($"- {p.LibroNavigation.Titulo} (Vence: {p.FechaVencimiento:dd/MM/yyyy})");
                }
            }
            else
            {
                Console.WriteLine("No tiene prestamos activos");
            }

            Console.WriteLine("\nHISTORIAL");
            var historial = socio.Prestamos.Where(p => p.Estado == 2 || p.Estado == 3).ToList();
            if (historial.Any())
            {
                foreach (var p in historial)
                {
                    string estado = p.Estado == 2 ? "Devuelto" : "Vencido";
                    Console.WriteLine($"- {p.LibroNavigation.Titulo} ({estado})");
                }
            }
            else
            {
                Console.WriteLine("No tiene historial");
            }

            Console.WriteLine("\nRESERVAS");
            if (socio.Reservas.Any())
            {
                foreach (var r in socio.Reservas)
                {
                    string estado = r.Estado == 1 ? "Pendiente" : r.Estado == 2 ? "Cumplida" : "Cancelada";
                    Console.WriteLine($"- {r.LibroNavigation.Titulo} ({estado})");
                }
            }
            else
            {
                Console.WriteLine("No tiene reservas");
            }
        }

        static void LibrosMasPrestados()
        {
            Console.WriteLine("LIBROS MAS PRESTADOS");

            var librosMasPrestados = _context.Prestamos
                .GroupBy(p => p.Libro)
                .Select(g => new
                {
                    ISBN = g.Key,
                    Cantidad = g.Count(),
                    Libro = _context.Libros.FirstOrDefault(l => l.ISBN == g.Key)
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(5)
                .ToList();

            Console.WriteLine($"{"Titulo",-50} {"Autor",-30} {"Prestamos",-10}");
            Console.WriteLine(new string('-', 90));

            foreach (var item in librosMasPrestados)
            {
                if (item.Libro != null)
                {
                    Console.WriteLine($"{item.Libro.Titulo,-50} {item.Libro.Autor,-30} {item.Cantidad,-10}");
                }
            }
        }

        static void SociosConMultas()
        {
            Console.WriteLine("SOCIOS CON MULTAS PENDIENTES");

            var sociosConMultas = _context.Socio
                .Where(s => _context.Multas.Any(m => m.SocioId == s.NroSocio && !m.Pagada))
                .Select(s => new
                {
                    Socio = s,
                    TotalMultas = _context.Multas.Where(m => m.SocioId == s.NroSocio && !m.Pagada).Sum(m => m.Monto)
                })
                .OrderByDescending(x => x.TotalMultas)
                .ToList();

            Console.WriteLine($"{"Socio",-30} {"Total Multas",-15}");
            Console.WriteLine(new string('-', 45));

            foreach (var item in sociosConMultas)
            {
                Console.WriteLine($"{item.Socio.Nombre} {item.Socio.Apellido,-25} ${item.TotalMultas,-15}");
            }
        }

        static void PrestamosVencidos()
        {
            Console.WriteLine("PRESTAMOS VENCIDOS");

            var prestamos = _context.Prestamos
                .Include(p => p.SocioNavigation)
                .Include(p => p.LibroNavigation)
                .Where(p => p.Estado == 3)
                .ToList();

            Console.WriteLine($"{"Socio",-30} {"Libro",-40} {"Vencimiento",-15} {"Dias atraso",-12}");
            Console.WriteLine(new string('-', 97));

            foreach (var p in prestamos)
            {
                int diasAtraso = (DateTime.Now - p.FechaVencimiento).Days;
                string socioNombre = $"{p.SocioNavigation.Nombre} {p.SocioNavigation.Apellido}";
                Console.WriteLine($"{socioNombre,-30} {p.LibroNavigation.Titulo,-40} {p.FechaVencimiento:dd/MM/yyyy,-15} {diasAtraso,-12}");
            }
        }

        static void DisponibilidadLibro()
        {
            Console.WriteLine("DISPONIBILIDAD DE LIBRO");
            Console.Write("Ingrese ISBN o Titulo: ");
            string busqueda = Console.ReadLine();

            var libro = _context.Libros
                .FirstOrDefault(l => l.ISBN == busqueda || l.Titulo.Contains(busqueda));

            if (libro == null)
            {
                Console.WriteLine("Libro no encontrado");
                return;
            }

            int prestados = _context.Prestamos.Count(p => p.Libro == libro.ISBN && p.Estado == 1);
            int disponibles = libro.CantidadCopias - prestados;
            int reservasPendientes = _context.Reservas.Count(r => r.Libro == libro.ISBN && r.Estado == 1);

            Console.WriteLine($"\n{libro.Titulo}");
            Console.WriteLine($"ISBN: {libro.ISBN}");
            Console.WriteLine($"Autor: {libro.Autor}");
            Console.WriteLine($"Genero: {libro.Genero}");
            Console.WriteLine($"Copias totales: {libro.CantidadCopias}");
            Console.WriteLine($"Copias disponibles: {disponibles}");
            Console.WriteLine($"Reservas pendientes: {reservasPendientes}");
        }

        static void HistorialSocio()
        {
            Console.WriteLine("HISTORIAL DE SOCIO");
            Console.Write("Ingrese Nro de Socio: ");
            if (!int.TryParse(Console.ReadLine(), out int nroSocio))
            {
                Console.WriteLine("Numero de socio invalido");
                return;
            }

            var socio = _context.Socio
                .Include(s => s.Prestamos)
                .ThenInclude(p => p.LibroNavigation)
                .Include(s => s.Reservas)
                .ThenInclude(r => r.LibroNavigation)
                .FirstOrDefault(s => s.NroSocio == nroSocio);

            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado");
                return;
            }

            Console.WriteLine($"\n{socio.Nombre} {socio.Apellido}");

            Console.WriteLine("\nPRESTAMOS");
            foreach (var p in socio.Prestamos.OrderByDescending(p => p.FechaPrestamo))
            {
                string estado = p.Estado == 1 ? "Activo" : p.Estado == 2 ? "Devuelto" : "Vencido";
                Console.WriteLine($"- {p.LibroNavigation.Titulo} ({estado}) - {p.FechaPrestamo:dd/MM/yyyy}");
            }

            Console.WriteLine("\nRESERVAS");
            foreach (var r in socio.Reservas.OrderByDescending(r => r.FechaReserva))
            {
                string estado = r.Estado == 1 ? "Pendiente" : r.Estado == 2 ? "Cumplida" : "Cancelada";
                Console.WriteLine($"- {r.LibroNavigation.Titulo} ({estado}) - {r.FechaReserva:dd/MM/yyyy}");
            }
        }

        static void RankingSocios()
        {
            Console.WriteLine("RANKING DE SOCIOS MAS ACTIVOS");

            var ranking = _context.Socio
                .Select(s => new
                {
                    Socio = s,
                    TotalPrestamos = _context.Prestamos.Count(p => p.Socio == s.NroSocio),
                    MultasPendientes = _context.Multas.Any(m => m.SocioId == s.NroSocio && !m.Pagada)
                })
                .OrderByDescending(x => x.TotalPrestamos)
                .Take(10)
                .ToList();

            Console.WriteLine($"{"Posicion",-10} {"Socio",-30} {"Tipo",-15} {"Prestamos",-12} {"Multas",-10}");
            Console.WriteLine(new string('-', 77));

            int posicion = 1;
            foreach (var item in ranking)
            {
                string tipo = _context.TiposSocios.Find(item.Socio.TipoSocio)?.Tipo ?? "Desconocido";
                string multas = item.MultasPendientes ? "Si" : "No";
                Console.WriteLine($"{posicion,-10} {item.Socio.Nombre} {item.Socio.Apellido,-23} {tipo,-15} {item.TotalPrestamos,-12} {multas,-10}");
                posicion++;
            }
        }
    }
}