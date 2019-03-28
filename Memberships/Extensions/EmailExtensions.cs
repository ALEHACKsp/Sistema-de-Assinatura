using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Memberships.Extensions
{
    public static class EmailExtensions
    {

        public static void Send(this IdentityMessage message)
        {

            try
            {
                //verifica a configurações do envio do email no Web.config
                var password = ConfigurationManager.AppSettings["password"];
                var from = ConfigurationManager.AppSettings["from"];
                var host = ConfigurationManager.AppSettings["host"];
                var port = int.Parse(ConfigurationManager.AppSettings["port"]);

                //cria o email que vai enviar
                var email = new MailMessage(from, message.Destination, message.Subject, message.Body);
                email.IsBodyHtml = true;

                //Cria SMTP pra enviar o email
                var client = new SmtpClient(host, port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from, password);

                //Envia o email
                client.Send(email);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}