using ShopDAL.Models;
using ShopAPI.DTOs;

namespace ShopAPI.Services.IServices
{
    public interface IComboService
    {
        PagedResult<ComboDto> GetAll(ComboFilterViewmodel filter);
        ComboDto GetById(int id);
        Combo GetComboById(int id);
        ComboDto Create(CreateComboRequest request);
        ComboDto Update(int id, UpdateComboRequest request);
        void Deactivate(int id);
        void Activate(int id);
        bool CheckDuplicateName(string name, int? excludeId = null);
    }
}
