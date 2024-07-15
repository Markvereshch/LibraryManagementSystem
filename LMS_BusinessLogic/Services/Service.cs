using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Interfaces;

namespace LMS_BusinessLogic.Services
{
    public class Service<Model, Entity> : IService<Model>
    where Entity : class, IEntity
    where Model : class
    {
        protected readonly IRepository<Entity> _repository;
        protected readonly IMapper _mapper;
        protected readonly ICaching<Entity> _cache;
        public Service(IRepository<Entity> repository, IMapper mapper, ICaching<Entity> cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<Model> CreateAsync(Model model)
        {
            var entity = _mapper.Map<Entity>(model);
            var createdEntity = await _repository.CreateAsync(entity);
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<Model>(createdEntity);
        }
        public async Task DeleteAsync(Model bookModel)
        {
            var book = _mapper.Map<Entity>(bookModel);
            await _repository.DeleteAsync(book);
            await _cache.InvalidateCacheAsync(book.Id);
            await _cache.InvalidateCachedCollectionAsync();
        }
        public async Task<Model> GetAsync(int id)
        {
            var entity = await _cache.GetCacheAsync(id);
            if (entity == null)
            {
                entity = await _repository.GetAsync(id);
                await _cache.SetCacheAsync(entity);
            }
            return _mapper.Map<Model>(entity);
        }
        public async Task<Model> UpdateAsync(Model book, int id)
        {
            var entityToUpdate = await _cache.GetCacheAsync(id);
            if (entityToUpdate == null)
            {
                entityToUpdate = await _repository.GetAsync(id);
            }
            _mapper.Map(book, entityToUpdate);
            entityToUpdate.Id = id;

            var updatedEntity = await _repository.UpdateAsync(entityToUpdate);

            await _cache.InvalidateCacheAsync(entityToUpdate.Id);
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<Model>(updatedEntity);
        }
        public async Task<(Model?, ValidationResults, string)> ValidateExistingModel(int id)
        {
            if (id <= 0)
            {
                return (null, ValidationResults.BadRequest, $"Invalid ID ({id} is less than 1)");
            }
            var entity = await GetAsync(id);
            if (entity == null)
            {
                return (null, ValidationResults.NotFound, $"Entity with ID={id} cannot be found");
            }
            var model = _mapper.Map<Model>(entity);
            return (model, ValidationResults.OK, "OK");
        }
    }
}
