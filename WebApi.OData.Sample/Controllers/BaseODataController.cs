using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using WebApi.OData.Sample.DAL;
using WebApi.OData.Sample.Models;

namespace WebApi.OData.Sample.Controllers
{
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : ModelBase
    {
        protected AppDbContext _dbContext = new AppDbContext();

        [HttpGet]
        [EnableQuery]
        public IQueryable<TEntity> Get()
        {
            return _dbContext.Set<TEntity>();
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<TEntity> Get([FromODataUri] int key)
        {
            IQueryable<TEntity> result = _dbContext.Set<TEntity>().Where(x => x.Id == key);
            return SingleResult.Create(result);
        }

        [HttpPatch]
        public async Task<IHttpActionResult> Patch([FromODataUri]int key, [FromBody]Delta<TEntity> model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbContext.Set<TEntity>().FindAsync(key);
            if(entity == null)
            {
                return NotFound();
            }

            model.Patch(entity);

            UpdateAuditedEntityModifiedInfo(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!EntityExist(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromODataUri] int key, [FromBody]TEntity model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(key != model.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(model).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!EntityExist(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(model);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(TEntity model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UpdateAuditedEntityCreatedInfo(model);

            _dbContext.Set<TEntity>().Add(model);
            await _dbContext.SaveChangesAsync();

            return Created(model);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromODataUri]int key)
        {
            TEntity entity = _dbContext.Set<TEntity>().Find(key);
            if(entity == null)
            {
                return NotFound();
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool EntityExist(int id)
        {
            return _dbContext.Set<TEntity>().Find(id) != null;
        }

        private void UpdateAuditedEntityModifiedInfo(TEntity entity)
        {
            var now = DateTimeOffset.Now;
            var user = GetUserStamp();
            entity.Modified = now;
            entity.ModifiedBy = user;
        }

        private void UpdateAuditedEntityCreatedInfo(TEntity entity)
        {
            var now = DateTimeOffset.Now;
            var user = GetUserStamp();
            entity.Created = now;
            entity.CreatedBy = user;
        }

        private string GetUserStamp()
        {
            return User?.Identity?.Name;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}