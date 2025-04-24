namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface IGenericService<T, ViewModel, CreateVm, UpdateVm>
        where T : class
        where ViewModel : class
        where CreateVm : class
        where UpdateVm : class
    {
        Task<IEnumerable<ViewModel>> GetAllAsync();
        Task<ViewModel?> GetByIdAsync(int id);
        Task<int> AddAsync(CreateVm vm);
        Task<int> UpdateAsync(UpdateVm vm);
        Task<int> DeleteAsync(int id);
    }
}
