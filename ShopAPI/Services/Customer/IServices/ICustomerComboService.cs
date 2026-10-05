using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerComboService
    {
        PagedResult<ComboDto> Getall(ComboFilterViewmodel comboFilterViewmodel);
        ComboDto GetById(int id);
    }
}
