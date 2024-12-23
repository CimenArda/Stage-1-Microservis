using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class RabbitMQSettings
    {
        //Stock api OrderCreatedEvente karşılık kullanıcağı kuyruk
        public const string Stock_OrderCreatedEvent = "Stock-orderCreatedEvent-queue";

        //payment api StockReservedEvent e karşılık kullanacagı kuyruk
        public const string Payment_StockReservedEvent = "Payment-stockReservedEvent-queue";

        //Order api Paymentın completed olması durumuna karsılık kullanacağı kuyruk
        public const string Order_PaymentCompletedEvent = "Order-paymentCompletedEvent-queue";

        //Stock api Paymentın failed olması durumuna karsılık kullanacağı kuyruk

        public const string Stock_PaymentFailedEvent = "Stock-paymentFailedEvent-queue";

        // Order api Stokta olumsuz durum olması durumuna karsılık kullanacağı kuyruk
        public const string Order_StockNotReservedEvent = "Order_stockNotReservedEvent-queue";
    }
}
