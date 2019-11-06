using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data;
using Devart.Common;
using System.IO;


namespace OraMailLoad
{
    public class Mail
    {
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public DateTime DateMail { get; set; }
        public string Text { get; set; }

        public List<MailFile> Attach;

        public Mail(string MailFrom, string MailTo, string Subject, DateTime DateMail, string Text
            , List<MailFile> Attach
            )
        {
            this.MailFrom = MailFrom;
            this.MailTo = MailTo;
            this.Subject = Subject;
            this.DateMail = DateMail;
            this.Text = Text;
            this.Attach = Attach;
        }


    }

    public class MailFile
    {
        public string FileName { get; set; }
        public byte[] FileBlob { get; set; }

        public MailFile(string FileName, byte[] FileBlob)
        {
            this.FileName = FileName;
            this.FileBlob = FileBlob;
        }

      
    }
}
