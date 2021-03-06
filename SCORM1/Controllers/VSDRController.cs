using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using SCORM1.Models.PageCustomization;
using PagedList;
using SCORM1.Models.Logs;
using SCORM1.Models.ratings;
using System.Security.Cryptography.X509Certificates;
using SCORM1.Models.RigidCourse;
using iTextSharp.text.pdf.events;
using SCORM1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.ViewModel;
using SCORM1.Models.VSDR;
using System.Collections.Generic;
using System;

namespace SCORM1.Controllers
{
    public class VSDRController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public VSDRController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: VSDR
        public ActionResult Index()
        {
            return View();
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        #region VSDR Teachers
        //Returns all available VSDR sessions available
        [Authorize]
        public ActionResult VsdrUserListTeacher()
        {
            VsdrUserVM vsdrModel = new VsdrUserVM();
            //Get list from data base;
            List<VsdrSession> vsdrList = new List<VsdrSession>();
            List<VsdrSession> tempList = new List<VsdrSession>();
            vsdrList = ApplicationDbContext.VsdrSessions.ToList();

            //Removes not available and date expired vsdr sessions
            if (vsdrList.Count > 0)
            {
                foreach (VsdrSession debateRoom in vsdrList)
                {
                    if (!debateRoom.available)
                    {
                        tempList.Add(debateRoom);
                    }
                    else if (debateRoom.end_date < DateTime.Now)
                    {
                        tempList.Add(debateRoom);
                    }
                }
                foreach (VsdrSession debateRoom in tempList)
                {
                    vsdrList.Remove(debateRoom);
                }
            }
            //Fillter list by date and availability;
            vsdrModel.listOfVsdr = vsdrList;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [Authorize]
        public ActionResult VsdrTeacher(int id)
        {
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            VsdrUserVM vsdrModel = new VsdrUserVM();
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles.Where(x => x.vsdr_id == id).ToList();
            VsdrUserFile file = new VsdrUserFile();
            if (vsdrToReturn.end_date.Subtract(DateTime.Now).TotalMinutes < 30 && vsdrUserIssuedFiles.Count > 0)
            {
                vsdrModel.meetingAvailable = true;
            }
            else
            {
                TempData["ButtonInfo"] = "Asegurate de que el tiempo de la sesión sea el indicado";
            }
            //add loaded data to view model
            vsdrModel.vsdrFileToAdd = file;
            vsdrModel.listOfIssuedFiles = vsdrUserIssuedFiles;
            vsdrModel.actualVsdr = vsdrToReturn;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddCommentView(CommentViewVM model)
        {
            var actualUser = GetActualUserId();
            model.Sesion = actualUser.SesionUser;
            VsdrTeacherComment comment = new VsdrTeacherComment
            {
                user_id = model.actualFile.user_id,
                vsdr_id = model.actualFile.vsdr_id,
                teacher_id = actualUser.Id,
                content = model.commentToAdd.content,
                commentDate = DateTime.Now
            };
            VsdrSession actualSesion = ApplicationDbContext.VsdrSessions.Find(model.actualFile.vsdr_id);
            model.actualVSDR = actualSesion;
            ApplicationDbContext.VsdrTeacherComments.Add(comment);
            ApplicationDbContext.SaveChanges();
            return View(model);
        }

        [Authorize]
        public ActionResult AddCommentView(int id)
        {
            CommentViewVM model = new CommentViewVM();
            VsdrUserFile file = ApplicationDbContext.VsdrUserFiles.Find(id);
            VsdrSession actualSesion = ApplicationDbContext.VsdrSessions.Find(file.vsdr_id);
            model.actualFile = file;
            model.actualVSDR = actualSesion;
            model.commentToAdd = new VsdrTeacherComment();
            model.Sesion = GetActualUserId().SesionUser;
            return View(model);
        }
        
        #endregion

        #region VSDR Users
        //Returns all available VSDR sessions available
        [Authorize]
        public ActionResult VsdrUserList()
        {
            VsdrUserVM vsdrModel = new VsdrUserVM();
            //Get list from data base;
            List<VsdrSession> vsdrList= new List<VsdrSession>();
            List<VsdrSession> tempList= new List<VsdrSession>();
            vsdrList = ApplicationDbContext.VsdrSessions.ToList();

            //Removes not available and date expired vsdr sessions
            if (vsdrList.Count > 0)
            {
                foreach (VsdrSession debateRoom in vsdrList)
                {
                    if (!debateRoom.available)
                    {
                        tempList.Add(debateRoom);
                    }
                    else if (debateRoom.end_date < DateTime.Now)
                    {
                        tempList.Add(debateRoom);
                    }
                }
                foreach (VsdrSession debateRoom in tempList)
                {
                    vsdrList.Remove(debateRoom);
                }
            }
            //Fillter list by date and availability;
            vsdrModel.listOfVsdr = vsdrList;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        [Authorize]
        public ActionResult VsdrContent(int id)
        {
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            VsdrUserVM vsdrModel = new VsdrUserVM();
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id).ToList();
            List<VsdrTeacherComment> vsdrTeacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id).ToList();
            VsdrUserFile file = new VsdrUserFile(); 
            if (vsdrToReturn.end_date.Subtract(DateTime.Now).TotalMinutes < 15&&vsdrUserIssuedFiles.Count>0)
            {
                vsdrModel.meetingAvailable = true;
            }
            else
            {
                TempData["ButtonInfo"] = "Asegurate de haber subido por lo menos 1 archivo y de que el tiempo de la sesión sea el indicado";
            }
            //add loaded data to view model
            vsdrModel.vsdrFileToAdd = file;
            vsdrModel.listOfIssuedFiles = vsdrUserIssuedFiles;
            vsdrModel.listOfComments = vsdrTeacherComments;
            vsdrModel.actualVsdr = vsdrToReturn;
            vsdrModel.Sesion = GetActualUserId().SesionUser;
            return View(vsdrModel);
        }

        //Function thats receive the view model information and an uploaded file
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVSDRFile(VsdrUserVM model, HttpPostedFileBase upload)
        {
            var actualUser = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var vsdre = ApplicationDbContext.VsdrSessions.Find(model.actualVsdr.id);
            List<VsdrUserFile> vsdrUserIssuedFiles = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id).ToList();
            List<VsdrTeacherComment> vsdrTeacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id).ToList();
            VsdrUserFile fileEmpty = new VsdrUserFile();
            model.vsdrFileToAdd = fileEmpty;
            model.listOfIssuedFiles = vsdrUserIssuedFiles;
            model.listOfComments = vsdrTeacherComments;
            model.Sesion = GetActualUserId().SesionUser;
            if (upload != null && upload.ContentLength > 0 && upload.ContentLength <= (2 * 1000000))
            {
                string[] allowedExtensions = new[] { ".pdf", ".doc", ".pptx", ".xls", ".xlsx", ".docx" };
                var ext = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                var file = "";
                foreach(var extention in allowedExtensions)
                {
                    if (extention.Contains(ext))
                    {
                        file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                        upload.SaveAs(Server.MapPath("~/VSDRUploads/" + file));
                        VsdrUserFile fileToAdd = new VsdrUserFile
                        {
                            user_id = actualUser.Id,
                            vsdr_id = vsdre.id,
                            register_name = model.vsdrFileToAdd.register_name,
                            file_description = model.vsdrFileToAdd.file_description,
                            file_extention = ext,
                            file_name = file,
                            registered_date = DateTime.Now
                        };
                        ApplicationDbContext.VsdrUserFiles.Add(fileToAdd);
                        ApplicationDbContext.SaveChanges();
                        TempData["Info"] = "Archivo cargado exitosamente";
                        List<VsdrUserFile> fileList = ApplicationDbContext.VsdrUserFiles.Where(x => x.user_id == actualUser.Id).ToList();
                        List<VsdrTeacherComment> teacherComments = ApplicationDbContext.VsdrTeacherComments.Where(x => x.user_id == actualUser.Id).ToList();
                        model.listOfIssuedFiles = fileList;
                        model.listOfComments = teacherComments;
                        model.actualVsdr = vsdre;
                        return View("VsdrContent", model);
                    }
                }
                TempData["Info"] = "El formato del archivo no es valido";
                return View("VsdrContent", model);
            }
            else
            {
                TempData["Info"] = "Los campos no pueden estar vacios";
                return View("VsdrContent", model);
            }
        }
        public ActionResult RedirectToUrl(int id)
        {
            VsdrSession vsdrToReturn = ApplicationDbContext.VsdrSessions.Find(id);
            return Redirect(vsdrToReturn.resource_url);
        }
        
        #endregion

        #region VSDR Admin
        #endregion

    }
}