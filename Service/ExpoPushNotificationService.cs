using Expo.Server.Client;
using Expo.Server.Models;
using Kallum.Data;
using Kallum.DTOS.Notifications;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Service
{
    public class ExpoPushNotificationService
    {
        public readonly ApplicationDBContext _context;
        public readonly UserIdService _userIdService;
        public ExpoPushNotificationService(ApplicationDBContext context, UserIdService userIdService)
        {
            _context = context;
            _userIdService = userIdService;
        }
        public async Task ExpoNotification(string bankId, string message)
        {
            var token = await GetToken(bankId);



            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest
            {
                PushTo = new List<string> { token },
                PushBadgeCount = 1,
                PushBody = message,

                PushTitle = "Kaelance"
            };

            try
            {
                var result = await expoSDKClient.PushSendAsync(pushTicketReq);

                // Handle the response
                Console.WriteLine($"Push notification sent: {result}");
                if (result != null)
                {
                    Console.WriteLine($"Details: {result}");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error sending push notification: {ex.Message}");
            }
        }
        private async Task<string?> GetToken(string bankId)
        {
            try
            {

                var bankInfo = await _userIdService.GetBankAccountInfo(bankId);

                if (bankInfo is null)
                {
                    return null;
                }
                var kallum = await _context.KallumLockData.FirstOrDefaultAsync(x => x.AppUserId == bankInfo.AppUserId);
                if (kallum is null)
                {
                    return null;
                }

                // If a match is found, return the PushNotificationToken, otherwise return null
                return kallum.PushNotificationToken;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving token: {ex.Message}");
                return null;
            }
        }

    }
}
