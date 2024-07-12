using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Notifications;
using Kallum.Helper;

namespace Kallum.Interfaces
{
    public interface INotificationRepository
    {

        Task<List<NotificationDto>> GetNotification(string username);

    }
}