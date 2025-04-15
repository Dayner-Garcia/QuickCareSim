using AutoMapper;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services;

namespace QuickCareSim.Application.Services
{
    public class GenericService<T, ViewModel, CreateVm, UpdateVm> : IGenericService<T, ViewModel, CreateVm, UpdateVm>
         where T : class
         where ViewModel : class
         where UpdateVm : class
         where CreateVm : class
    {
        private readonly IGenericRepository<T> _genericRepository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<T> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(CreateVm vm)
        {
            var entity = _mapper.Map<T>(vm);
            return await _genericRepository.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _genericRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ViewModel>> GetAllAsync()
        {
            var entities = await _genericRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ViewModel>>(entities);
        }

        public async Task<ViewModel?> GetByIdAsync(int id)
        {
            var entity = await _genericRepository.GetByIdAsync(id);
            return _mapper.Map<ViewModel>(entity);
        }

        public async Task<int> UpdateAsync(UpdateVm vm)
        {
            var entity = _mapper.Map<T>(vm);
            return await _genericRepository.UpdateAsync(entity);
        }
    }
}
