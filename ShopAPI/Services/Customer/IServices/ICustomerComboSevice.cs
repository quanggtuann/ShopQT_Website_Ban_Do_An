using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerComboSevice
    {
        PagedResult<ComboDto> Getall(ComboFilterViewmodel comboFilterViewmodel);
        ComboDto GetById(int id);
    }
}
