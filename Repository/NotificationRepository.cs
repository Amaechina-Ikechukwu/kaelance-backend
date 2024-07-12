using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.Notifications;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Mappers;
using Kallum.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kallum.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public readonly ServiceComputations _serviceComputations;
        public NotificationRepository(ApplicationDBContext context, UserIdService userIdService, ServiceComputations serviceComputations)
        {
            _context = context;
            _userIdService = userIdService;
            _serviceComputations = serviceComputations;
        }



        public async Task<List<NotificationDto>> GetNotification(string username)
        {
            string bankId = await _userIdService.GetBankAccountNumberWithUsername(username);
            if (bankId is null)
            {
                return null;
            }
            List<NotificationDto> notifications = await _context.Notifications.Where(notification => notification.BankId == bankId).Select(info => info.ToGetNotificationDto()).ToListAsync();
            if (notifications is null)
            {
                return null;
            }
            return notifications;
        }
    }
}