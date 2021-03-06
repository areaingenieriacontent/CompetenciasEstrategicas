using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCORM1.Models.VSDR;

namespace SCORM1.Models.ViewModel
{
    public class VsdrUserVM : BaseViewModel
    {
        public VsdrSession actualVsdr { get; set; }
        public List<VsdrSession> listOfVsdr { get; set; }
        public List<VsdrUserFile> listOfIssuedFiles { get; set; }
        public List<VsdrTeacherComment> listOfComments { get; set; }
        public string teacherName { get; set; }
        public string teacherLastName { get; set; }
        public bool meetingAvailable { get; set; }

        /*file upload variables*/
        public VsdrUserFile vsdrFileToAdd { get; set; }
    }

    public class CommentVM
    {
        public string studentName { get; set; }
        public int commentId { get; set; }
        public string content { get; set; }
        public string teacherName { get; set; }
    }

    public class CommentViewVM : BaseViewModel
    {
        public List<CommentVM> commentList { get; set; }
        public VsdrSession actualVSDR { get; set; }
        public VsdrUserFile actualFile {get;set;}
        public VsdrTeacherComment commentToAdd { get; set; }
    }
}