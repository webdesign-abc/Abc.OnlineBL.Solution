using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class PhotographerLogin
    {
        public const string LOGGED_IN = "LoggedIn";
        public const string LOGGED_OUT = "LoggedOut";
        public const string ERROR = "Error";

        public static PhotographerLogin InvalidUsernamePassword = new PhotographerLogin("Invalid username/password.");
        public static PhotographerLogin BackendResponseError = new PhotographerLogin("PhotoSync service threw exception.");
        public static PhotographerLogin NetworkError = new PhotographerLogin("Unable to contact the ABC Server.");
        public static PhotographerLogin MissingIMEI = new PhotographerLogin("IMEI must be supplied.");
        public static PhotographerLogin MissingUsername = new PhotographerLogin("Username must be supplied.");
        public static PhotographerLogin MissingPassword = new PhotographerLogin("Password must be supplied.");
        public static PhotographerLogin LoggedOut = new PhotographerLogin();

        private PhotographerLogin()
        {
            Status = LOGGED_OUT;
            Message = "Logged Out.";
        }

        private PhotographerLogin(string errorMessage)
        {
            Status = ERROR;
            Message = errorMessage;
        }

        public PhotographerLogin(int photographerId, string username, string photographerName, bool isActive, bool isChiefDronePilot)
        {
            Status = LOGGED_IN;
            Message = "Logged In.";
            PhotographerId = photographerId;
            Username = username;
            PhotographerName = photographerName;
            IsActive = isActive;
            IsChiefDronePilot = isChiefDronePilot;
        }

        [DataMember]
        public string Status { get; private set; }

        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public int PhotographerId { get; private set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string PhotographerName { get; private set; }

        [DataMember]
        public bool IsActive { get; private set; }

        [DataMember]
        public bool IsChiefDronePilot { get; private set; }
    }
}
