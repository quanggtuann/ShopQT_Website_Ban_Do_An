using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IComboRepo
    {
        IQueryable<Combo> GetAllCombos();
        Combo? GetById(int id);
    }
}
