using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using StackExchange.Redis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IDatabase _redisDatabase;

        public ProductController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _redisDatabase = RedisHelper.RedisCache;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var cacheKey = "Products";
            var cachedProducts = await _redisDatabase.StringGetAsync(cacheKey);

            if (!cachedProducts.IsNullOrEmpty)
            {
                return Ok(JsonConvert.DeserializeObject<List<ECommerceApp.Models.Product>>(cachedProducts));
            }

            using var session = NHibernateHelper.OpenSession();
            var products = session.Query<ECommerceApp.Models.Product>().ToList();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            await _redisDatabase.StringSetAsync(cacheKey, JsonConvert.SerializeObject(products));

            return Ok(products);
        }

        // POST api/<ProductController>
        // POST api/<ProductController>
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ECommerceApp.Models.Product product)
        {
            using var session = NHibernateHelper.OpenSession();
            using var transaction = session.BeginTransaction();

            session.Save(product);
            transaction.Commit();

            _redisDatabase.KeyDelete("Products");

            return CreatedAtAction(nameof(GetProducts), new { id = product.ID }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ECommerceApp.Models.Product productUpdates)
        {
            using var session = NHibernateHelper.OpenSession();
            var product = session.Get<ECommerceApp.Models.Product>(id);

            if (product == null)
            {
                return NotFound();
            }

            product.price = productUpdates.price;
            product.description = productUpdates.description;
            product.category = productUpdates.category;

            using var transaction = session.BeginTransaction();
            session.SaveOrUpdate(product);
            transaction.Commit();

            _redisDatabase.KeyDelete("Products");

            return NoContent();
        }


        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using var session = NHibernateHelper.OpenSession();
            var product = session.Get<ECommerceApp.Models.Product>(id);

            if (product == null)
            {
                return NotFound();
            }

            using var transaction = session.BeginTransaction();
            session.Delete(product);
            transaction.Commit();

            _redisDatabase.KeyDelete("Products");

            return NoContent();
        }

    }
}
