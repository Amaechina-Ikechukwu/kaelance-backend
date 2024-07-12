using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.DTOS.Notifications
{
    public class NotificationDto
    {
        public string? Title { get; set; }
        public DateTime DateTime { get; set; }

        public string BankId { get; set; }
        public bool SeenNotification { get; set; }
        //Could be Circle Creation or Transaction Notification
        public string? Type { get; set; }

        //Has the circle id or transaction id
        public string? TypeId { get; set; }
    }
}