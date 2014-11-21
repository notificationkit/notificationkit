using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.ServiceBus.Notifications;

namespace NotificationKit.Models
{
    public class RegistrationModel
    {
        public RegistrationModel()
        {
        }

        public RegistrationModel(RegistrationDescription description)
        {
            RegistrationId = description.RegistrationId;
            RegistrationTime = description.ExpirationTime.Value.AddDays(-90);
            ExpirationTime = description.ExpirationTime.Value;
            Tags = description.Tags ?? new HashSet<string>();

            var windowsRegistrationDescription = description as WindowsRegistrationDescription;
            if (windowsRegistrationDescription != null)
            {
                Platform = Platforms.Windows;
                Handle = windowsRegistrationDescription.ChannelUri.AbsoluteUri;
            }

            var mpnsRegistrationDescription = description as MpnsRegistrationDescription;
            if (mpnsRegistrationDescription != null)
            {
                Platform = Platforms.WindowsPhone;
                Handle = mpnsRegistrationDescription.ChannelUri.AbsoluteUri;
            }

            var appleRegistrationDescription = description as AppleRegistrationDescription;
            if (appleRegistrationDescription != null)
            {
                Platform = Platforms.Apple;
                Handle = appleRegistrationDescription.DeviceToken;
            }

            var gcmRegistrationDescription = description as GcmRegistrationDescription;
            if (gcmRegistrationDescription != null)
            {
                Platform = Platforms.Android;
                Handle = gcmRegistrationDescription.GcmRegistrationId;
            }
        }

        public string RegistrationId { get; set; }
        public DateTimeOffset RegistrationTime { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public string Platform { get; set; }
        public ISet<string> Tags { get; set; }
        public string Handle { get; set; }
    }

    public class SendFormModel
    {
        [Required]
        public string Message { get; set; }

        public string TagExpression { get; set; }

        public int Windows { get; set; }

        public int WindowsPhone { get; set; }

        public int Apple { get; set; }

        public int Android { get; set; }
    }

    public class DeleteRegistrationFormModel
    {
        [Required]
        public string RegistrationId { get; set; }
    }

    public class ScheduleFormModel
    {
        [Required]
        public string Message { get; set; }

        public string TagExpression { get; set; }

        [Required]
        public DateTimeOffset? ScheduledOn { get; set; }

        public int Windows { get; set; }

        public int WindowsPhone { get; set; }

        public int Apple { get; set; }

        public int Android { get; set; }
    }

    public class CancelScheduleFormModel
    {
        [Required]
        public string ScheduledNotificationId { get; set; }
    }
}