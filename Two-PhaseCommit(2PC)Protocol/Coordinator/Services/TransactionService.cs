using Coordinator.Models;
using Coordinator.Models.Context;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TwoPhaseCommitContext _context;
        private readonly IHttpClientFactory _httpClientFactory;


        public TransactionService(TwoPhaseCommitContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionState.Pending
                }
            });

            await _context.SaveChangesAsync();
            return transactionId;

        }

        public async Task PrepareServicesAsync(Guid transactionId)
        {
             var transactionNodes = await  _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _httpClientFactory.CreateClient("OrderApi").GetAsync("ready"),
                        "Stock.API" => _httpClientFactory.CreateClient("StockApi").GetAsync("ready"),
                        "Payment.API" => _httpClientFactory.CreateClient("PaymentApi").GetAsync("ready"),
                    });
                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;
                }
                catch (Exception)
                {

                    transactionNode.IsReady = Enums.ReadyType.Unready;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
              return (await _context.NodeStates
                .Where(ns=>ns.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns=>ns.IsReady ==Enums.ReadyType.Ready);

        }

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                                    .Where(ns => ns.TransactionId == transactionId)
                                    .Include(ns => ns.Node)
                                    .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await(transactionNode.Node.Name switch
                    {
                        "Order.API" => _httpClientFactory.CreateClient("OrderApi").GetAsync("commit"),
                        "Stock.API" => _httpClientFactory.CreateClient("StockApi").GetAsync("commit"),
                        "Payment.API" => _httpClientFactory.CreateClient("PaymentApi").GetAsync("commit"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        {
            return (await _context.NodeStates
               .Where(ns => ns.TransactionId == transactionId)
               .ToListAsync()).TrueForAll(ns => ns.TransactionState == Enums.TransactionState.Done);

        }

        public async Task RoolbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if (transactionNode.TransactionState == Enums.TransactionState.Done)
                        _ = await(transactionNode.Node.Name switch
                        {
                            "Order.API" => _httpClientFactory.CreateClient("OrderApi").GetAsync("rollback"),
                            "Stock.API" => _httpClientFactory.CreateClient("StockApi").GetAsync("rollback"),
                            "Payment.API" => _httpClientFactory.CreateClient("PaymentApi").GetAsync("rollback"),
                        });

                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
