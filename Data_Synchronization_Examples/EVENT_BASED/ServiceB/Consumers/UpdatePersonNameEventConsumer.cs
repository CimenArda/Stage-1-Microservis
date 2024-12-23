using MassTransit;
using Microsoft.EntityFrameworkCore;
using ServiceB.Context;
using Shared.Events;

namespace ServiceB.Consumers
{
    public class UpdatePersonNameEventConsumer : IConsumer<UpdatedPersonNameEvent>
    {
        private readonly ServiceBContext _dbContext;

        public UpdatePersonNameEventConsumer(ServiceBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<UpdatedPersonNameEvent> context)
        {
            // MSSQL veritabanından ilgili çalışan kaydını bul
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.PersonID == context.Message.PersonId);
            if (employee == null)
            {
                // Eğer çalışan bulunamazsa hata loglama veya başka bir işlem yapılabilir
                return;
            }

            // Çalışanın adını güncelle
            employee.Name = context.Message.NewName;

            // Değişiklikleri kaydet
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();
        }
    }
}
