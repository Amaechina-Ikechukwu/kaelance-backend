using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Notifications;
using Kallum.Models;

namespace Kallum.Mappers
{
    public static class NotificationMappers
    {
        public static GeneralNotification ToCreateNotificationDto(this NotificationDto notificationDto, string bankId)
        {
            return new GeneralNotification
            {
                BankId = bankId,
                DateTime = notificationDto.DateTime,
                SeenNotification = notificationDto.SeenNotification,
                Title = notificationDto.Title,
                Type = notificationDto.Type,
                TypeId = notificationDto.TypeId

            };
        }

        public static NotificationDto ToGetNotificationDto(this GeneralNotification notification)
        {
            return new NotificationDto
            {
                DateTime = notification.DateTime,
                SeenNotification = notification.SeenNotification,
                Title = notification.Title,
                Type = notification.Type,
                TypeId = notification.TypeId
            };
        }
    }
}