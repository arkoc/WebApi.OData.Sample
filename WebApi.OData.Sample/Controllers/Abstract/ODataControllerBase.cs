using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using WebApi.OData.Sample.DAL;
using WebApi.OData.Sample.Models.Abstract;

namespace WebApi.OData.Sample.Controllers.Abstract
{
    public abstract class ODataControllerBase<TEntity> : ODataController where TEntity : ModelBase
    {
        public delegate void EntityEventHandler(TEntity entity);

        public event EntityEventHandler CreatingEvent = (x) => { };
        public event EntityEventHandler CreatedEvent = (x) => { };

        public event EntityEventHandler UpdatingEvent = (x) => { };
        public event EntityEventHandler UpdatedEvent = (x) => { };

        public event EntityEventHandler DeletingEvent = (x) => { };
        public event EntityEventHandler DeletedEvent = (x) => { };


        protected AppDbContext _appDbContext;

        public ODataControllerBase(AppDbContext protectableDbContext)
        {
            _appDbContext = protectableDbContext;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<TEntity> Get()
        {
            var result = _appDbContext.Set<TEntity>();
            return result;
        }

        [HttpGet]
        [EnableQuery]
        public virtual SingleResult<TEntity> Get([FromODataUri] int key)
        {
            IQueryable<TEntity> result = _appDbContext.Set<TEntity>().Where(x => x.Id == key);
            var singleResult = SingleResult.Create(result);
            return singleResult;
        }

        [HttpPatch]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri]int key, [FromBody]Delta<TEntity> model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = _appDbContext.Set<TEntity>().Find(key);
            if (entity == null)
            {
                return NotFound();
            }

            model.Patch(entity);

            UpdateModifiedBy(entity);

            try
            {

                UpdatingEvent(entity);

                await _appDbContext.SaveChangesAsync();

                UpdatedEvent(entity);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExist(key))
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
        public virtual async Task<IHttpActionResult> Put([FromODataUri] int key, [FromBody]TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != entity.Id)
            {
                return BadRequest();
            }

            _appDbContext.Entry(entity).State = EntityState.Modified;

            try
            {

                UpdatingEvent(entity);

                await _appDbContext.SaveChangesAsync();

                UpdatedEvent(entity);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExist(key))
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

        [HttpPost]
        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UpdateCreatedBy(entity);

            _appDbContext.Set<TEntity>().Add(entity);

            CreatingEvent(entity);

            await _appDbContext.SaveChangesAsync();

            CreatedEvent(entity);

            return Created(entity);
        }

        [HttpDelete]
        public virtual async Task<IHttpActionResult> Delete([FromODataUri]int key)
        {
            TEntity entity = _appDbContext.Set<TEntity>().Find(key);
            if (entity == null)
            {
                return NotFound();
            }

            _appDbContext.Set<TEntity>().Remove(entity);

            DeletingEvent(entity);

            await _appDbContext.SaveChangesAsync();

            DeletedEvent(entity);

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool EntityExist(int id)
        {
            return _appDbContext.Set<TEntity>().Find(id) != null;
        }

        private void UpdateModifiedBy(TEntity entity)
        {
            var user = GetUserStamp();
            entity.ModifiedBy = user;
        }

        private void UpdateCreatedBy(TEntity entity)
        {
            var user = GetUserStamp();
            entity.CreatedBy = user;
        }

        private string GetUserStamp()
        {
            return User?.Identity?.Name;
        }

    }
}