using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;

namespace ProFrame.Mail
{
    /// <summary>
    /// Рассылка сообщений по внутризаводской почте
    /// </summary>
    public class MailSender
    {
        #region параметры
        /// <summary>
        /// Почтовый сервер(по умолчанию)
        /// </summary>
        string smtpServer = "mail01.uuap.com";
        /// <summary>
        /// отправитель(по умолчанию notifprog@uuap.com)
        /// </summary>
        string from ="notifprog@uuap.com";
        /// <summary>
        /// Получает или задает логин отправителя
        /// </summary>
        public string From
        {
            get { return from; }
            set { from = value; }
        }
        /// <summary>
        /// Пароль отправителя(по умолчанию - automailsender78)
        /// </summary>
        string password = "automailsender78";
        /// <summary>
        /// Задает пароль отправителя
        /// </summary>
        public string Password
        {
            set { password = value; }
        }
        /// <summary>
        /// Адресаты
        /// </summary>
        List<string> adressee = new List<string> { };
        /// <summary>
        /// Получает или задает список адресатов
        /// </summary>
        public List<string> Adressee
        {
            get { return adressee; }
            set { adressee = value; }
        }
        /// <summary>
        /// Заголовок сообщения
        /// </summary>
        string caption = string.Empty;
        /// <summary>
        /// Получает или задает заголовок сообщения
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        string message = string.Empty;
        /// <summary>
        /// Получает или задает текст сообщения
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        /// <summary>
        /// Письмо
        /// </summary>
        MailMessage mail;
        /// <summary>
        /// Почтовый клиент
        /// </summary>
        SmtpClient client = new SmtpClient();
        #endregion параметры

        #region конструкторы
        /// <summary>
        /// Конструктор с вложениями
        /// </summary>
        /// <param name="adressee">Список адресатов</param>
        /// <param name="caption">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="attachments">Массив названий файлов вложений</param>
        public MailSender(List<string> adressee, string caption, string message, string[] attachments)
        {
            mail = new MailMessage();
            this.adressee = adressee;
            this.caption = caption;
            this.message = message;
            //заполнить вложения
            foreach (string s in attachments)
                mail.Attachments.Add(new Attachment(s));
        }
        /// <summary>
        /// Конструктор с адресатами,заголовком и сообщением
        /// </summary>
        /// <param name="adressee">Адресаты</param>
        /// <param name="caption">Заголовок</param>
        /// <param name="message">Сообщение</param>
        public MailSender(List<string> adressee, string caption, string message)
        {
            mail = new MailMessage();
            this.adressee = adressee;
            this.caption = caption;
            this.message = message;
        }
        /// <summary>
        /// Конструктор с адресатами и сообщением
        /// </summary>
        /// <param name="adressee">адресаты</param>
        /// <param name="message">сообщение</param>
        public MailSender(List<string> adressee, string message)
        {
            mail = new MailMessage();
            this.adressee = adressee;
            this.message = message;
        }
        /// <summary>
        /// Конструктор без входных параметров
        /// </summary>
        public MailSender()
        {
            mail = new MailMessage();
        }
        #endregion конструкторы

        #region перегрузки метода прикрепления вложения
        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="fileName">Файл </param>
        public void AddAttachment(string fileName)
        {
            mail.Attachments.Add(new Attachment(fileName));
        }

        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="contentStream">содержимое вложения</param>
        /// <param name="name">имя</param>
        public void AddAttachment(Stream contentStream, string name)
        {
            mail.Attachments.Add(new Attachment(contentStream, name));
        }

        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="contentStream">содержимое вложения</param>
        /// <param name="contentType">Данные параметра</param>
        public void AddAttachment(Stream contentStream, ContentType contentType)
        {
            mail.Attachments.Add(new Attachment(contentStream, contentType));
        }

        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="fileName">файл</param>
        /// <param name="mediaType">MIME content-header информация данного вложения</param>
        public void AddAttachment(string fileName, string mediaType)
        {
            mail.Attachments.Add(new Attachment(fileName, mediaType));
        }

        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="fileName">Файл</param>
        /// <param name="contentType">Данные имени файла</param>
        public void AddAttachment(string fileName, ContentType contentType)
        {
            mail.Attachments.Add(new Attachment(fileName, contentType));
        }

        /// <summary>
        /// Прикрепить вложение к письму
        /// </summary>
        /// <param name="contentStream">содержимое вложения</param>
        /// <param name="name">имя</param>
        /// <param name="mediaType">MIME content-header информация данного вложения</param>
        public void AddAttachment(Stream contentStream, string name, string mediaType)
        {
            mail.Attachments.Add(new Attachment(contentStream, name, mediaType));
        }
        #endregion

        /// <summary>
        /// Получить коллекцию вложений
        /// </summary>
        /// <returns>Колллекция вложений</returns>
        public AttachmentCollection GetAttachments()
        {
            return mail.Attachments;
        }

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        public void Send()
        {
            mail.From = new MailAddress(from);
            foreach (string mailto in adressee)
            {
                mail.To.Add(new MailAddress(mailto));
            }
            mail.Subject = caption;
            mail.Body = message;
            client.Host = smtpServer;
            client.Port = 25;
            client.EnableSsl = false;
            client.Credentials = new NetworkCredential(from.Split('@')[0], password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
        }

    }
}
