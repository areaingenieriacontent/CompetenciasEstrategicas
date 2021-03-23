using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using SCORM1.Models.Lms;
using SCORM1.Models;

namespace SCORM1.Controllers
{
    public class ContactoController : Controller
    {
        protected ApplicationDbContext ApplicationDb { get; set; }

        // GET: Contacto/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Contacto/Create
        [HttpPost]
        public ActionResult Create(string correos,string identificacion, string empresa, string nombres, string categoria, string descripcion)
        {
            ApplicationDb = new ApplicationDbContext();
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("competenciasestrategicas@sanmateo.edu.co");
            correo.To.Add("competenciasestrategicas@sanmateo.edu.co");
            correo.Subject = categoria + identificacion;
            string caso = "El usuario " + nombres + " identicado con el numero " + identificacion + " genero un nuevo caso de soporte: ";
            string casoPrueba = "<div id='cuerpo'>" +
        "<div id='nombre'>" +
            "Nombre del estudiante si es de dos renglones" +
        "</div>" +

        "<a href='#' target='_blank'>" +
            "<div id='guia'>" +
            "GUÍA DE NAVEGACIÓN" +
                "<div id='clic'>Dando clic aquí</div>" +
            "/div>" +
        "</a>" +

        "<div id='contacto'>" +
            "competenciasestrategicas@sanmateo.edu.co" +
            "<br>" +
            "(+57) 300 6651 560" +
        "</div>" +

        "<a href='#' target='_blank'>" +
            "<div id='plataforma'>" +
            "Ingresa Aquí" +
            "</div>" +
        "</a>" +

    "</div>";
            correo.Body = caso + descripcion;
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            //
            var smtp = new SmtpClient();
            smtp.Send(correo);
            int count = ApplicationDb.Correos.ToList().Count();
            CorreoModel correoDB;
            if (count == 0)
            {

                correoDB = new CorreoModel
                {
                    Nombre = nombres,
                    Categoria = categoria,
                    Documento = identificacion,
                    Mensaje = descripcion,
                    Empresa = empresa,
                    IdMensaje = "1",
                    Correos=correos
                };
            }
            else
            {
                count = count + 1;
                correoDB = new CorreoModel
                {
                    Nombre = nombres,
                    Categoria = categoria,
                    Documento = identificacion,
                    Mensaje = descripcion,
                    Empresa = empresa,
                    IdMensaje = count.ToString(),
                    Correos=correos
                  
                };
            }

            ApplicationDb.Correos.Add(correoDB);
            ApplicationDb.SaveChanges();

            CreateUser(correos);
            return RedirectToAction("Index", "Home");

        }
        public ActionResult CreateUser(string correos)
        {
            ApplicationDb = new ApplicationDbContext();
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("competenciasestrategicas@sanmateo.edu.co");
            correo.To.Add(correos);
            correo.Subject = "Inquietud a la mesa de servicio Competencias Estratégicas";
            string caso = "Acabamos de recibir tu mensaje. Nuestro equipo de trabajo analizará tu solicitud para determinar el nivel de complejidad."+'\n'+
                "Te estaremos informando en máximo 3 horas al correo registrado el tiempo de repuesta asignado.";
            string casoPrueba = "	<div id='cuerpo'>" +
        "<div id='nombre'>" +
            "Nombre del estudiante si es de dos renglones" +
        "</div>" +

        "<a href='#' target='_blank'>" +
            "<div id='guia'>" +
            "GUÍA DE NAVEGACIÓN" +
                "div id='clic'>Dando clic aquí</div>" +
            "/div>" +
        "</a>" +

        "<div id='contacto'>" +
            "competenciasestrategicas@sanmateo.edu.co" +
            "<br>" +
            "(+57) 300 6651 560" +
        "</div>" +

        "<a href='#' target='_blank'>" +
            "<div id='plataforma'>" +
            "Ingresa Aquí" +
            "</div>" +
        "</a>" +

    "</div>";
            correo.Body = caso;
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            //
            var smtp = new SmtpClient();

            smtp.Send(correo);

            return RedirectToAction("Index", "Home");

        }
    }
}
