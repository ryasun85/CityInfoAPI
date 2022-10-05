using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    //[Route("api/[controller]")] will match to controller name
    public class CitiesController : ControllerBase
    {

        //Step 1 with Json

        //    [HttpGet]
        //    public JsonResult GetCities() 
        //    {
        //        return new JsonResult(CitiesDataStore.Current.Cities);
        //            //new List<object>
        //            //{
        //            //    new{id =1, Name = "New York City" },
        //            //    new{id =2, Name = "Antwerp"}
        //            //});
        //    }

        //Step 2 with DTOs

        //private readonly CitiesDataStore _citiesDataStore;
        //public CitiesController(CitiesDataStore citiesDataStore)
        //{
        //    _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        //}

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]                                                                              //explicit
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery]string? name, string? searchQuery, 
            int pageNumber = 1, int pageSize = 10)
        {
            if(pageNumber > maxCitiesPageSize)
            {
                pageNumber = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber , pageSize);
            //add pagination metadata as header to response
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
            //Without Automapper 
            //var results = new List<CityWithoutPointsOfInterestDto>();

            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name,
            //    });

            //}
            //return Ok(results);
            //return Ok(_citiesDataStore.Cities);


        }
        //[HttpGet("{id}")]
        //public JsonResult GetCity(int id)
        //{
        //    //return new JsonResult(CitiesDataStore.Current.Cities.Where(c => c.Id == id));
        //    return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}

        [HttpGet("{id}")]
        //public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointsOfInterest=false)
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            //Without Automapper
            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);

            //With automapper
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if(includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
