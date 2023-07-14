using FluentNHibernate.Conventions.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using StackExchange.Redis;
using System.Numerics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IDatabase _redisDatabase;

        public OrderController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _redisDatabase = RedisHelper.RedisCache;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "Orders";
            var cachedOrders = await _redisDatabase.StringGetAsync(cacheKey);

            if (!cachedOrders.IsNullOrEmpty)
            {
                return Ok(JsonConvert.DeserializeObject<List<ECommerceApp.Models.Order>>(cachedOrders));
            }

            using var session = NHibernateHelper.OpenSession();
            var orders = session.Query<ECommerceApp.Models.Order>().ToList();

            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }

            await _redisDatabase.StringSetAsync(cacheKey, JsonConvert.SerializeObject(orders));

            return Ok(orders);
        }



        // POST api/Order
        [HttpPost]
        [HttpPost]
        public IActionResult CreateOrder([FromBody] ECommerceApp.Models.Order order)
        {
            using var session = NHibernateHelper.OpenSession();
            using var transaction = session.BeginTransaction();

            session.Save(order);
            transaction.Commit();

            _redisDatabase.KeyDelete("Orders");

            return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
        }

        // PUT api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] ECommerceApp.Models.Order orderUpdates)
        {
            using var session = NHibernateHelper.OpenSession();
            var order = session.Get<ECommerceApp.Models.Order>(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Name = orderUpdates.Name;

            using var transaction = session.BeginTransaction();
            session.SaveOrUpdate(order);
            transaction.Commit();

            _redisDatabase.KeyDelete("Orders");

            return NoContent();
        }

        // DELETE api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            using var session = NHibernateHelper.OpenSession();
            var order = session.Get<ECommerceApp.Models.Order>(id);

            if (order == null)
            {
                return NotFound();
            }

            using var transaction = session.BeginTransaction();
            session.Delete(order);
            transaction.Commit();

            _redisDatabase.KeyDelete("Orders");

            return NoContent();
        }
    }
    }