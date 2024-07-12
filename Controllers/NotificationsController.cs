using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.Notifications;
using Kallum.Extensions;
using Kallum.Helper;
using Kallum.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        readonly INotificationRepository _notificationRepository;
        public NotificationsController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                string username = User.GetUsername();
                List<NotificationDto> notifications = await _notificationRepository.GetNotification(username);
                return Ok(notifications);
            }
            catch
            {
                return Conflict(new CircleResponseDto
                {
                    Message = "Error getting notifications"
                });
            }

        }


    }
}