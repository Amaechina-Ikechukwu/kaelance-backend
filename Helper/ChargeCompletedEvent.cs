using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kallum.Helper
{
    public class ChargeCompletedEvent
    {


        public string Event { get; set; }


        public ChargeCompletedData Data { get; set; }
    }

    public class ChargeCompletedData
    {

        public int Id { get; set; }


        public string Tx_ref { get; set; }


        public string Flw_ref { get; set; }


        public string Device_fingerprint { get; set; }


        public decimal Amount { get; set; }


        public string Currency { get; set; }


        public decimal Charged_amount { get; set; }


        public decimal AppFee { get; set; }


        public decimal MerchantFee { get; set; }


        public string Processor_response { get; set; }


        public string Auth_model { get; set; }


        public string Ip { get; set; }


        public string Narration { get; set; }


        public string Status { get; set; }


        public string Payment_type { get; set; }


        public DateTime Created_At { get; set; }


        public int Account_id { get; set; }


        public Customer Customer { get; set; }


        public Card Card { get; set; }
    }

    public class Customer
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public string Phone_number { get; set; }


        public string Email { get; set; }


        public DateTime Created_at { get; set; }
    }

    public class Card
    {

        public string First_6Digits { get; set; }


        public string Last_4Digits { get; set; }


        public string Issuer { get; set; }


        public string Country { get; set; }


        public string Type { get; set; }


        public string Expiry { get; set; }
    }

}