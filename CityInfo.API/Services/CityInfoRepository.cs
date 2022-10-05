using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(o => o.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery,  int pageNumber, int pageSize)
        {
            //No need when paging is implemented 
            //if (string.IsNullOrWhiteSpace(name)
            //    && string.IsNullOrWhiteSpace(searchQuery))
            //{ 
            //    return await GetCitiesAsync(); 
            //}

            var collection =_context.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(w => w.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(w => w.Name.Contains(searchQuery)
                || (w.Description != null && w.Description.Contains(searchQuery)));
            }
            //db call
            var totalItemCount = await collection.CountAsync();

            //construct pagination metadata
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

           var collectionToReturn = await collection.OrderBy(o => o.Name).Skip(pageSize * (pageNumber -1))
                .Take(pageSize).ToListAsync();

            return (collectionToReturn, paginationMetadata);
            //name = name.Trim();

            //return await _context.Cities.Where(c => c.Name == name).OrderBy(o => o.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(i => i.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityID)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityID);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(w => w.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }
        public async Task<bool> SaveChangesAsync() 
        {
            return await _context.SaveChangesAsync() >=0;
        }
    }
}
