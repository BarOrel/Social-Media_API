using Microsoft.EntityFrameworkCore;

namespace SocialMedia_API.Data.Repository.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private SocialMediaDbContext context;
      
        public GenericRepository(SocialMediaDbContext _context)
        {
            context = _context;
            
        }



       
        public IEnumerable<T> GetAll()
        {
            
        }

        public T GetById(object id)
        {
            
        }

        public void Insert(T obj)
        {
            
        }

        public void Update(T obj)
        {
           
        }

        public void Delete(object id)
        {
            
        }

        public void Save()
        {
            context.SaveChanges();
        }

    }
}
