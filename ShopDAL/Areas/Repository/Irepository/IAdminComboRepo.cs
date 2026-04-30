using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminComboRepo
    {
        IQueryable<Combo> Getall();
        Combo GetById(int id);
        void Add(Combo combo);
        void Update(Combo combo);
        void Deactivate(int id);
        void Activate(int id);
    }
}
